using System.Collections.Concurrent;
using Godot;
using MF.Commons.Core.Enums.Infrastructure;
using MF.Data.Configuration.Resources;
using MF.Infrastructure.Abstractions.Core.Caching;
using MF.Infrastructure.Abstractions.Core.Logging;
using MF.Infrastructure.Abstractions.Core.Monitoring;
using MF.Services.Abstractions.Core.ResourceLoading;


namespace MF.Services.Core.ResourceLoading;

/// <summary>
/// Godot资源加载器实现
/// </summary>
public class GodotResourceLoader : IResourceLoader, IDisposable
{
    private readonly IGameLogger _logger;
    private readonly ResourceLoadingConfig _config;
    private readonly SemaphoreSlim _loadingSemaphore;
    private readonly ConcurrentDictionary<string, WeakReference> _loadedResources = new();
    private readonly ICacheService _cacheService;
    private readonly IPerformanceMonitor? _performanceMonitor;
    private readonly IMemoryMonitor? _memoryMonitor;
    private bool _disposed;
    
    // 内置验证逻辑
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".bmp", ".tga", ".webp", // 图片
        ".ogg", ".wav", ".mp3", // 音频
        ".tscn", ".tres", // Godot资源
        ".gd", ".cs" // 脚本
    };
    
    private static readonly HashSet<Type> SupportedTypes = new()
    {
        typeof(Texture2D),
        typeof(AudioStream),
        typeof(PackedScene),
        typeof(Resource),
        typeof(Script)
    };
    
    public GodotResourceLoader(
        IGameLogger<GodotResourceLoader> logger,
        ICacheService cacheService,
        ResourceLoadingConfig? config = null,
        IPerformanceMonitor? performanceMonitor = null,
        IMemoryMonitor? memoryMonitor = null)
    {
        _logger = logger;
        _cacheService = cacheService;
        _config = config ?? new ResourceLoadingConfig();
        _performanceMonitor = performanceMonitor;
        _memoryMonitor = memoryMonitor;
        _loadingSemaphore = new SemaphoreSlim(_config.MaxConcurrentLoads, _config.MaxConcurrentLoads);
        
        // 设置内存压力处理
        if (_memoryMonitor != null)
        {
            _memoryMonitor.MemoryPressureDetected += OnMemoryPressureDetected;
            _memoryMonitor.StartMonitoring();
        }
        
        _logger.LogInformation("GodotResourceLoader initialized with config: {Config}", _config);
    }
    
    public async Task<T?> LoadAsync<T>(string path, CancellationToken cancellationToken = default) where T : class
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GodotResourceLoader));
        
        // 验证路径
        if (!IsValidPath(path))
        {
            _logger.LogError("Invalid resource path: {Path}", path);
            throw new ArgumentException($"Invalid resource path: {path}", nameof(path));
        }
        
        // 验证类型
        if (!IsSupportedType(typeof(T)))
        {
            _logger.LogError("Unsupported resource type: {Type}", typeof(T).Name);
            throw new ArgumentException($"Unsupported resource type: {typeof(T).Name}", nameof(T));
        }
        
        var startTime = DateTime.UtcNow;
        
        try
        {
            await _loadingSemaphore.WaitAsync(cancellationToken);
            
            _logger.LogDebug("Loading resource: {Path}, Type: {Type}", path, typeof(T).Name);
            
            // 检查是否已经加载过
            if (_loadedResources.TryGetValue(path, out var weakRef) && weakRef.IsAlive)
            {
                if (weakRef.Target is T cachedResource)
                {
                    _logger.LogDebug("Resource found in cache: {Path}", path);
                    return cachedResource;
                }
            }
            
            // 使用Godot的ResourceLoader加载资源
            var resource = await Task.Run(() =>
            {
                try
                {
                    var godotResource = ResourceLoader.Load(path);
                    return godotResource;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Godot ResourceLoader failed for path: {Path}", path);
                    return null;
                }
            }, cancellationToken);
            
            if (resource == null)
            {
                _logger.LogWarning("Resource not found: {Path}", path);
                return null;
            }
            
            if (resource is not T typedResource)
            {
                _logger.LogError("Resource type mismatch. Expected: {ExpectedType}, Actual: {ActualType}, Path: {Path}", 
                    typeof(T).Name, resource.GetType().Name, path);
                return null;
            }
            
            // 缓存资源的弱引用
            _loadedResources.AddOrUpdate(path, new WeakReference(typedResource), (key, oldRef) => new WeakReference(typedResource));
            
            _logger.LogDebug("Resource loaded successfully: {Path}", path);
            return typedResource;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Resource loading cancelled: {Path}", path);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load resource: {Path}", path);
            return null;
        }
        finally
        {
            _loadingSemaphore.Release();
        }
    }
    
    public async Task<T?> LoadAsync<T>(string path, ResourceCacheStrategy cacheStrategy, CancellationToken cancellationToken = default) where T : class
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GodotResourceLoader));

        // 验证路径
        if (!IsValidPath(path))
        {
            _logger.LogError("Invalid resource path: {Path}", path);
            throw new ArgumentException($"Invalid resource path: {path}", nameof(path));
        }
        
        // 验证类型
        if (!IsSupportedType(typeof(T)))
        {
            _logger.LogError("Unsupported resource type: {Type}", typeof(T).Name);
            throw new ArgumentException($"Unsupported resource type: {typeof(T).Name}", nameof(T));
        }

        var startTime = DateTime.UtcNow;
        var tags = new Dictionary<string, string>
        {
            ["resource_type"] = typeof(T).Name,
            ["cache_strategy"] = cacheStrategy.ToString(),
            ["resource_path"] = path
        };
        
        _performanceMonitor?.RecordCounter("resource.load_attempts", 1, tags);
        
        try
        {
            await _loadingSemaphore.WaitAsync(cancellationToken);
            
            _logger.LogDebug("Loading resource with strategy: {Path}, Type: {Type}, Strategy: {Strategy}", 
                path, typeof(T).Name, cacheStrategy);
            
            // 根据缓存策略决定是否检查缓存
            if (ShouldUseCache(cacheStrategy))
            {
                // 先检查ICacheService缓存
                var cachedResource = await _cacheService.GetAsync<T>(path);
                if (cachedResource != null)
                {
                    _performanceMonitor?.RecordCounter("resource.cache_hit", 1, tags);
                    _logger.LogDebug("Resource found in cache: {Path}", path);
                    return cachedResource;
                }
                
                // 检查弱引用缓存
                if (_loadedResources.TryGetValue(path, out var weakRef) && weakRef.IsAlive)
                {
                    if (weakRef.Target is T weakCachedResource)
                    {
                        _performanceMonitor?.RecordCounter("resource.cache_hit", 1, tags);
                        _logger.LogDebug("Resource found in weak reference cache: {Path}", path);
                        return weakCachedResource;
                    }
                }
                
                _performanceMonitor?.RecordCounter("resource.cache_miss", 1, tags);
            }
            
            // 使用Godot的ResourceLoader加载资源
            var resource = await Task.Run(() =>
            {
                try
                {
                    var godotResource = ResourceLoader.Load(path);
                    
                    return godotResource;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Godot ResourceLoader failed for path: {Path}", path);
                    return null;
                }
            }, cancellationToken);
            
            if (resource == null)
            {
                _logger.LogWarning("Resource not found: {Path}", path);
                return null;
            }
            
            if (resource is not T typedResource)
            {
                _logger.LogError("Resource type mismatch. Expected: {ExpectedType}, Actual: {ActualType}, Path: {Path}", 
                    typeof(T).Name, resource.GetType().Name, path);
                return null;
            }
            
            // 根据缓存策略决定如何缓存
            if (ShouldCache(cacheStrategy))
            {
                var expiration = GetCacheExpiration(cacheStrategy);
                await _cacheService.SetAsync(path, typedResource, expiration);
                
                // 对于弱引用策略，也保存到弱引用缓存
                if (cacheStrategy == ResourceCacheStrategy.WeakReference)
                {
                    _loadedResources.AddOrUpdate(path, new WeakReference(typedResource), 
                        (key, oldRef) => new WeakReference(typedResource));
                }
            }
            
            var loadTime = DateTime.UtcNow - startTime;
            _performanceMonitor?.RecordTimer("resource.load", loadTime, tags);
            
            _logger.LogDebug("Resource loaded successfully: {Path}", path);
            return typedResource;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Resource loading cancelled: {Path}", path);
            throw;
        }
        catch (Exception ex)
        {
            _performanceMonitor?.RecordCounter("resource.load_errors", 1, tags);
            _logger.LogError(ex, "Failed to load resource: {Path}", path);
            return null;
        }
        finally
        {
            _loadingSemaphore.Release();
        }
    }
    
    public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GodotResourceLoader));
        
        try
        {
            return await Task.Run(() => ResourceLoader.Exists(path), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking resource existence: {Path}", path);
            return false;
        }
    }
    

    
    public async Task PreloadAsync(string path, CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GodotResourceLoader));
        
        _logger.LogDebug("Preloading resource: {Path}", path);
        
        // 异步预加载，不阻塞当前线程
        _ = Task.Run(async () =>
        {
            try
            {
                await LoadAsync<Resource>(path, cancellationToken);
                _logger.LogDebug("Resource preloaded: {Path}", path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to preload resource: {Path}", path);
            }
        }, cancellationToken);
    }
    
    public async Task UnloadAsync(string path)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GodotResourceLoader));
        
        try
        {
            if (_loadedResources.TryRemove(path, out var weakRef))
            {
                if (weakRef.IsAlive && weakRef.Target is Resource resource)
                {
                    // Godot资源通常由引擎管理，这里只是移除我们的引用
                    _logger.LogDebug("Resource unloaded from cache: {Path}", path);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unloading resource: {Path}", path);
        }
    }
    

    public async Task PreloadBatchAsync(IEnumerable<string> paths, CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GodotResourceLoader));
        
        var pathList = paths.ToList();
        _logger.LogDebug("Batch preloading {Count} resources", pathList.Count);
        
        var tasks = pathList.Select(path => PreloadAsync(path, cancellationToken));
        await Task.WhenAll(tasks);
        
        _logger.LogDebug("Batch preloading completed for {Count} resources", pathList.Count);
    }
    
    public async Task ClearCacheAsync(bool force = false)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GodotResourceLoader));
        
        _logger.LogInformation("Clearing resource cache, force: {Force}", force);
        
        try
        {
            // 清理ICacheService缓存
            await _cacheService.ClearAsync();
            
            // 清理弱引用缓存
            if (force)
            {
                _loadedResources.Clear();
            }
            else
            {
                // 只清理已经被GC回收的弱引用
                var keysToRemove = new List<string>();
                foreach (var kvp in _loadedResources)
                {
                    if (!kvp.Value.IsAlive)
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }
                
                foreach (var key in keysToRemove)
                {
                    _loadedResources.TryRemove(key, out _);
                }
            }
            
            _performanceMonitor?.RecordCounter("resource.cache_cleared", 1, new Dictionary<string, string>
            {
                ["force"] = force.ToString()
            });
            
            _logger.LogInformation("Resource cache cleared successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing resource cache");
            throw;
        }
    }
    
    private bool ShouldUseCache(ResourceCacheStrategy strategy)
    {
        return strategy switch
        {
            ResourceCacheStrategy.NoCache => false,
            ResourceCacheStrategy.Default => _config.EnableResourceCache,
            _ => true
        };
    }
    
    private bool ShouldCache(ResourceCacheStrategy strategy)
    {
        return strategy switch
        {
            ResourceCacheStrategy.NoCache => false,
            ResourceCacheStrategy.Default => _config.EnableResourceCache,
            _ => true
        };
    }
    
    private TimeSpan? GetCacheExpiration(ResourceCacheStrategy strategy)
    {
        return strategy switch
        {
            ResourceCacheStrategy.Temporary => TimeSpan.FromMinutes(10),
            ResourceCacheStrategy.Permanent => null, // 永不过期
            ResourceCacheStrategy.WeakReference => TimeSpan.FromHours(1),
            ResourceCacheStrategy.ForceCache => TimeSpan.FromHours(2),
            ResourceCacheStrategy.Default => TimeSpan.FromHours(1),
            _ => TimeSpan.FromHours(1)
        };
    }
    
    /// <summary>
    /// 验证资源路径是否有效
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns>是否有效</returns>
    private static bool IsValidPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;
        
        // 检查路径格式
        if (!path.StartsWith("res://") && !path.StartsWith("user://"))
            return false;
        
        // 检查扩展名
        var extension = path.GetExtension();
        return SupportedExtensions.Contains(extension);
    }
    
    /// <summary>
    /// 验证资源类型是否支持
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <returns>是否支持</returns>
    private static bool IsSupportedType(Type resourceType)
    {
        return SupportedTypes.Any(supportedType => supportedType.IsAssignableFrom(resourceType));
    }
    
    /// <summary>
    /// 验证资源大小是否在限制范围内
    /// </summary>
    /// <param name="size">资源大小</param>
    /// <returns>是否在限制范围内</returns>
    private static bool IsValidSize(long size)
    {
        // 限制单个资源最大100MB
        return size > 0 && size <= 100 * 1024 * 1024;
    }
    
    private void OnMemoryPressureDetected(long currentUsage)
    {
        _logger.LogWarning("Memory pressure detected: {CurrentUsage} bytes. Starting automatic cache cleanup.", currentUsage);
        
        // 异步执行清理，避免阻塞当前操作
        _ = Task.Run(async () =>
        {
            try
            {
                // 先进行非强制清理
                await ClearCacheAsync(force: false);
                
                // 检查内存压力是否仍然很高
                var newUsage = GC.GetTotalMemory(false);
                if (newUsage > _memoryMonitor?.AutoReleaseThreshold * 0.9) // 仍然接近阈值
                {
                    _logger.LogWarning("Memory pressure still high after cache cleanup. Performing forced cleanup and GC.");
                    
                    // 强制清理缓存
                    await ClearCacheAsync(force: true);
                    
                    // 强制垃圾回收
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    
                    var finalUsage = GC.GetTotalMemory(false);
                    _logger.LogInformation("Memory cleanup completed. Usage reduced from {Before} to {After} bytes.", 
                        currentUsage, finalUsage);
                    
                    _performanceMonitor?.RecordMetric("resource.memory_cleanup", finalUsage, new Dictionary<string, string>
                    {
                        ["before_cleanup"] = currentUsage.ToString(),
                        ["cleanup_type"] = "forced"
                    });
                }
                else
                {
                    _logger.LogInformation("Memory pressure resolved after cache cleanup. Usage: {NewUsage} bytes.", newUsage);
                    
                    _performanceMonitor?.RecordMetric("resource.memory_cleanup", newUsage, new Dictionary<string, string>
                    {
                        ["before_cleanup"] = currentUsage.ToString(),
                        ["cleanup_type"] = "normal"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during memory pressure cleanup");
            }
        });
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _logger.LogInformation("Disposing GodotResourceLoader");
        
        // 清理内存监控事件订阅
        if (_memoryMonitor != null)
        {
            _memoryMonitor.MemoryPressureDetected -= OnMemoryPressureDetected;
        }
        
        _loadingSemaphore.Dispose();
        _loadedResources.Clear();
        _disposed = true;
        
        _logger.LogInformation("GodotResourceLoader disposed");
    }
}