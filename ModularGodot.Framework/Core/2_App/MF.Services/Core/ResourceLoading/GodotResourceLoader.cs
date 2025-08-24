using Godot;
using MF.Infrastructure.Abstractions.ResourceLoading;
using MF.Infrastructure.Abstractions.Logging;
using System.Collections.Concurrent;

namespace MF.Services.ResourceLoading;

/// <summary>
/// Godot资源加载器实现
/// </summary>
public class GodotResourceLoader : IResourceLoader, IDisposable
{
    private readonly IGameLogger<GodotResourceLoader> _logger;
    private readonly ResourceLoadingConfig _config;
    private readonly SemaphoreSlim _loadingSemaphore;
    private readonly ConcurrentDictionary<string, WeakReference> _loadedResources = new();
    private bool _disposed;
    
    // 统计信息
    private long _totalLoadedResources;
    private long _successfulLoads;
    private long _failedLoads;
    private readonly List<double> _loadTimes = new();
    private readonly ConcurrentDictionary<string, long> _resourceTypeStatistics = new();
    
    public GodotResourceLoader(
        IGameLogger<GodotResourceLoader> logger,
        ResourceLoadingConfig? config = null)
    {
        _logger = logger;
        _config = config ?? new ResourceLoadingConfig();
        _loadingSemaphore = new SemaphoreSlim(_config.MaxConcurrentLoads, _config.MaxConcurrentLoads);
        
        _logger.LogInformation("GodotResourceLoader initialized with config: {Config}", _config);
    }
    
    public async Task<T?> LoadAsync<T>(string path, CancellationToken cancellationToken = default) where T : class
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GodotResourceLoader));
        
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
                Interlocked.Increment(ref _failedLoads);
                _logger.LogWarning("Resource not found: {Path}", path);
                return null;
            }
            
            if (resource is not T typedResource)
            {
                Interlocked.Increment(ref _failedLoads);
                _logger.LogError("Resource type mismatch. Expected: {ExpectedType}, Actual: {ActualType}, Path: {Path}", 
                    typeof(T).Name, resource.GetType().Name, path);
                return null;
            }
            
            // 缓存资源的弱引用
            _loadedResources.AddOrUpdate(path, new WeakReference(typedResource), (key, oldRef) => new WeakReference(typedResource));
            
            // 更新统计信息
            Interlocked.Increment(ref _totalLoadedResources);
            Interlocked.Increment(ref _successfulLoads);
            _resourceTypeStatistics.AddOrUpdate(typeof(T).Name, 1, (key, value) => value + 1);
            
            var loadTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
            lock (_loadTimes)
            {
                _loadTimes.Add(loadTime);
                if (_loadTimes.Count > 1000) // 保持最近1000次的记录
                {
                    _loadTimes.RemoveAt(0);
                }
            }
            
            _logger.LogDebug("Resource loaded successfully: {Path}, LoadTime: {LoadTime}ms", path, loadTime);
            return typedResource;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Resource loading cancelled: {Path}", path);
            throw;
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _failedLoads);
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
    
    public async Task<ResourceMetadata> GetMetadataAsync(string path, CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GodotResourceLoader));
        
        try
        {
            return await Task.Run(() =>
            {
                var metadata = new ResourceMetadata
                {
                    Path = path,
                    Exists = ResourceLoader.Exists(path)
                };
                
                if (metadata.Exists)
                {
                    try
                    {
                        // 尝试获取文件信息
                        var fileAccess = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                        if (fileAccess != null)
                        {
                            metadata.Size = (long)fileAccess.GetLength();
                            metadata.LastModified = DateTime.FromFileTime(fileAccess.GetModifiedTime());
                            fileAccess.Close();
                        }
                        
                        // 尝试确定资源类型
                        var extension = path.GetExtension().ToLower();
                        metadata.ResourceType = extension switch
                        {
                            ".png" or ".jpg" or ".jpeg" or ".bmp" or ".tga" or ".webp" => "Texture2D",
                            ".ogg" or ".wav" or ".mp3" => "AudioStream",
                            ".tscn" => "PackedScene",
                            ".tres" => "Resource",
                            ".gd" or ".cs" => "Script",
                            _ => "Unknown"
                        };
                        
                        metadata.MimeType = extension switch
                        {
                            ".png" => "image/png",
                            ".jpg" or ".jpeg" => "image/jpeg",
                            ".ogg" => "audio/ogg",
                            ".wav" => "audio/wav",
                            ".mp3" => "audio/mpeg",
                            _ => "application/octet-stream"
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to get detailed metadata for: {Path}", path);
                    }
                }
                
                return metadata;
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resource metadata: {Path}", path);
            return new ResourceMetadata { Path = path, Exists = false };
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
    
    public async Task<Dictionary<string, T?>> LoadManyAsync<T>(IEnumerable<string> paths, CancellationToken cancellationToken = default) where T : class
    {
        var tasks = paths.Select(async path =>
        {
            var resource = await LoadAsync<T>(path, cancellationToken);
            return new KeyValuePair<string, T?>(path, resource);
        });
        
        var results = await Task.WhenAll(tasks);
        return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    
    public async Task<ResourceLoaderStatistics> GetStatisticsAsync()
    {
        double averageLoadTime = 0;
        lock (_loadTimes)
        {
            if (_loadTimes.Count > 0)
            {
                averageLoadTime = _loadTimes.Average();
            }
        }
        
        // 清理无效的弱引用
        var validCacheCount = 0;
        var cacheMemoryUsage = 0L;
        
        var keysToRemove = new List<string>();
        foreach (var kvp in _loadedResources)
        {
            if (kvp.Value.IsAlive)
            {
                validCacheCount++;
                // 简单的内存使用估算
                cacheMemoryUsage += 1024; // 假设每个资源平均1KB
            }
            else
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        
        // 移除无效引用
        foreach (var key in keysToRemove)
        {
            _loadedResources.TryRemove(key, out _);
        }
        
        return new ResourceLoaderStatistics
        {
            TotalLoadedResources = _totalLoadedResources,
            SuccessfulLoads = _successfulLoads,
            FailedLoads = _failedLoads,
            AverageLoadTimeMs = averageLoadTime,
            CachedResourceCount = validCacheCount,
            CacheMemoryUsage = cacheMemoryUsage,
            ResourceTypeStatistics = new Dictionary<string, long>(_resourceTypeStatistics)
        };
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _logger.LogInformation("Disposing GodotResourceLoader");
        
        _loadingSemaphore.Dispose();
        _loadedResources.Clear();
        _disposed = true;
        
        _logger.LogInformation("GodotResourceLoader disposed");
    }
}

/// <summary>
/// 资源加载配置
/// </summary>
public class ResourceLoadingConfig
{
    /// <summary>
    /// 最大并发加载数
    /// </summary>
    public int MaxConcurrentLoads { get; set; } = 4;
    
    /// <summary>
    /// 加载超时时间
    /// </summary>
    public TimeSpan LoadTimeout { get; set; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// 是否启用预加载
    /// </summary>
    public bool EnablePreloading { get; set; } = true;
    
    /// <summary>
    /// 预加载路径
    /// </summary>
    public string[] PreloadPaths { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// 是否启用资源缓存
    /// </summary>
    public bool EnableResourceCache { get; set; } = true;
    
    /// <summary>
    /// 缓存清理间隔
    /// </summary>
    public TimeSpan CacheCleanupInterval { get; set; } = TimeSpan.FromMinutes(5);
    
    public override string ToString()
    {
        return $"MaxConcurrentLoads: {MaxConcurrentLoads}, LoadTimeout: {LoadTimeout}, EnablePreloading: {EnablePreloading}";
    }
}

/// <summary>
/// 资源验证器实现
/// </summary>
public class ResourceValidator : IResourceValidator
{
    private readonly HashSet<string> _supportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".bmp", ".tga", ".webp", // 图片
        ".ogg", ".wav", ".mp3", // 音频
        ".tscn", ".tres", // Godot资源
        ".gd", ".cs" // 脚本
    };
    
    private readonly HashSet<Type> _supportedTypes = new()
    {
        typeof(Texture2D),
        typeof(AudioStream),
        typeof(PackedScene),
        typeof(Resource),
        typeof(Script)
    };
    
    public bool IsValidPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;
        
        // 检查路径格式
        if (!path.StartsWith("res://") && !path.StartsWith("user://"))
            return false;
        
        // 检查扩展名
        var extension = path.GetExtension();
        return _supportedExtensions.Contains(extension);
    }
    
    public bool IsSupportedType(Type resourceType)
    {
        return _supportedTypes.Any(supportedType => supportedType.IsAssignableFrom(resourceType));
    }
    
    public bool IsValidSize(long size)
    {
        // 限制单个资源最大100MB
        return size > 0 && size <= 100 * 1024 * 1024;
    }
}