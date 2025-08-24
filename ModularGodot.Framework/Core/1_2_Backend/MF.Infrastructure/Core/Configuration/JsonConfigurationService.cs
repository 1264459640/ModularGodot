using Godot;
using MF.Infrastructure.Abstractions.Configuration;
using MF.Infrastructure.Abstractions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace MF.Infrastructure.Configuration;

/// <summary>
/// JSON配置服务实现
/// </summary>
public class JsonConfigurationService : IConfigurationService, IDisposable
{
    private readonly IGameLogger<JsonConfigurationService> _logger;
    private readonly string _configFilePath;
    private readonly ConcurrentDictionary<string, object> _configuration = new();
    private readonly object _lock = new();
    private bool _disposed;
    private FileSystemWatcher? _fileWatcher;
    
    public event Action<string, object?>? ValueChanged;
    public event Action? ConfigurationReloaded;
    
    public JsonConfigurationService(
        IGameLogger<JsonConfigurationService> logger,
        string? configFilePath = null)
    {
        _logger = logger;
        _configFilePath = configFilePath ?? "user://config/game_config.json";
        
        // 初始化时加载配置
        _ = LoadConfigurationAsync();
        
        // 设置文件监控
        SetupFileWatcher();
        
        _logger.LogInformation("JsonConfigurationService initialized with config file: {ConfigFilePath}", _configFilePath);
    }
    
    public T GetValue<T>(string key, T defaultValue = default!)
    {
        try
        {
            if (_configuration.TryGetValue(key, out var value))
            {
                if (value is T directValue)
                {
                    return directValue;
                }
                
                // 尝试类型转换
                if (value is JsonElement jsonElement)
                {
                    return JsonSerializer.Deserialize<T>(jsonElement.GetRawText()) ?? defaultValue;
                }
                
                // 尝试直接转换
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    _logger.LogWarning("Failed to convert config value for key: {Key}, using default value", key);
                    return defaultValue;
                }
            }
            
            _logger.LogDebug("Config key not found: {Key}, using default value", key);
            return defaultValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting config value for key: {Key}", key);
            return defaultValue;
        }
    }
    
    public async Task<T> GetValueAsync<T>(string key, T defaultValue = default!)
    {
        return await Task.FromResult(GetValue(key, defaultValue));
    }
    
    public void SetValue<T>(string key, T value)
    {
        try
        {
            var oldValue = _configuration.TryGetValue(key, out var existing) ? existing : null;
            _configuration.AddOrUpdate(key, value!, (k, v) => value!);
            
            _logger.LogDebug("Config value set: {Key} = {Value}", key, value);
            
            // 触发变化事件
            ValueChanged?.Invoke(key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting config value for key: {Key}", key);
            throw;
        }
    }
    
    public async Task SetValueAsync<T>(string key, T value)
    {
        SetValue(key, value);
        await Task.CompletedTask;
    }
    
    public bool HasKey(string key)
    {
        return _configuration.ContainsKey(key);
    }
    
    public void RemoveKey(string key)
    {
        try
        {
            if (_configuration.TryRemove(key, out var oldValue))
            {
                _logger.LogDebug("Config key removed: {Key}", key);
                ValueChanged?.Invoke(key, null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing config key: {Key}", key);
            throw;
        }
    }
    
    public IEnumerable<string> GetAllKeys()
    {
        return _configuration.Keys.ToList();
    }
    
    public Dictionary<string, object> GetSection(string prefix)
    {
        try
        {
            var section = new Dictionary<string, object>();
            var prefixWithDot = prefix.EndsWith(".") ? prefix : prefix + ".";
            
            foreach (var kvp in _configuration)
            {
                if (kvp.Key.StartsWith(prefixWithDot, StringComparison.OrdinalIgnoreCase))
                {
                    var sectionKey = kvp.Key.Substring(prefixWithDot.Length);
                    section[sectionKey] = kvp.Value;
                }
            }
            
            return section;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting config section: {Prefix}", prefix);
            return new Dictionary<string, object>();
        }
    }
    
    public async Task SaveAsync()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(JsonConfigurationService));
        
        try
        {
            lock (_lock)
            {
                // 确保目录存在
                var configDir = _configFilePath.GetBaseDir();
                if (!DirAccess.DirExistsAbsolute(configDir))
                {
                    DirAccess.MakeDirRecursiveAbsolute(configDir);
                }
                
                // 序列化配置
                var configDict = _configuration.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                var json = JsonSerializer.Serialize(configDict, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                // 写入文件
                using var file = FileAccess.Open(_configFilePath, FileAccess.ModeFlags.Write);
                if (file != null)
                {
                    file.StoreString(json);
                    file.Flush();
                }
                else
                {
                    throw new IOException($"Cannot write to config file: {_configFilePath}");
                }
            }
            
            _logger.LogInformation("Configuration saved to: {ConfigFilePath}", _configFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving configuration to: {ConfigFilePath}", _configFilePath);
            throw;
        }
    }
    
    public async Task ReloadAsync()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(JsonConfigurationService));
        
        try
        {
            await LoadConfigurationAsync();
            ConfigurationReloaded?.Invoke();
            _logger.LogInformation("Configuration reloaded from: {ConfigFilePath}", _configFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reloading configuration from: {ConfigFilePath}", _configFilePath);
            throw;
        }
    }
    
    private async Task LoadConfigurationAsync()
    {
        try
        {
            if (!FileAccess.FileExists(_configFilePath))
            {
                _logger.LogInformation("Config file not found, creating default: {ConfigFilePath}", _configFilePath);
                await CreateDefaultConfigurationAsync();
                return;
            }
            
            using var file = FileAccess.Open(_configFilePath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                _logger.LogWarning("Cannot read config file: {ConfigFilePath}", _configFilePath);
                return;
            }
            
            var json = file.GetAsText();
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("Config file is empty: {ConfigFilePath}", _configFilePath);
                return;
            }
            
            var configDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            if (configDict != null)
            {
                _configuration.Clear();
                foreach (var kvp in configDict)
                {
                    _configuration[kvp.Key] = kvp.Value;
                }
            }
            
            _logger.LogDebug("Configuration loaded from: {ConfigFilePath}, Keys: {KeyCount}", _configFilePath, _configuration.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configuration from: {ConfigFilePath}", _configFilePath);
        }
    }
    
    private async Task CreateDefaultConfigurationAsync()
    {
        try
        {
            // 创建默认配置
            var defaultConfig = new Dictionary<string, object>
            {
                ["Game.Language"] = "en",
                ["Game.Difficulty"] = "Normal",
                ["Audio.MasterVolume"] = 1.0f,
                ["Audio.MusicVolume"] = 0.8f,
                ["Audio.SfxVolume"] = 1.0f,
                ["Graphics.Resolution"] = "1920x1080",
                ["Graphics.Fullscreen"] = true,
                ["Graphics.VSync"] = true,
                ["Input.MouseSensitivity"] = 1.0f,
                ["Debug.EnableLogging"] = true,
                ["Debug.LogLevel"] = "Information"
            };
            
            foreach (var kvp in defaultConfig)
            {
                _configuration[kvp.Key] = kvp.Value;
            }
            
            await SaveAsync();
            _logger.LogInformation("Default configuration created: {ConfigFilePath}", _configFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating default configuration: {ConfigFilePath}", _configFilePath);
        }
    }
    
    private void SetupFileWatcher()
    {
        try
        {
            var configDir = _configFilePath.GetBaseDir();
            var configFileName = _configFilePath.GetFile();
            
            if (Directory.Exists(configDir))
            {
                _fileWatcher = new FileSystemWatcher(configDir, configFileName)
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                    EnableRaisingEvents = true
                };
                
                _fileWatcher.Changed += async (sender, e) =>
                {
                    try
                    {
                        // 延迟一点时间，确保文件写入完成
                        await Task.Delay(100);
                        await ReloadAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling config file change");
                    }
                };
                
                _logger.LogDebug("File watcher setup for config file: {ConfigFilePath}", _configFilePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to setup file watcher for config file: {ConfigFilePath}", _configFilePath);
        }
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _logger.LogInformation("Disposing JsonConfigurationService");
        
        _fileWatcher?.Dispose();
        _configuration.Clear();
        _disposed = true;
        
        _logger.LogInformation("JsonConfigurationService disposed");
    }
}