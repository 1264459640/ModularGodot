using Godot;



namespace MF.Infrastructure.Logging;

/// <summary>
/// Godot游戏日志实现
/// </summary>
public class GodotGameLogger : IGameLogger, IDisposable
{
    private static readonly Dictionary<LogLevel, Color> DefaultLogColors = new()
    {
        { LogLevel.Trace, Colors.Gray },
        { LogLevel.Debug, Colors.Cyan },
        { LogLevel.Information, Colors.White },
        { LogLevel.Warning, Colors.Yellow },
        { LogLevel.Error, Colors.Red },
        { LogLevel.Critical, Colors.DarkRed }
    };
    
    private readonly string _categoryName;
    private LogLevel _currentLevel = LogLevel.Information;
    private bool _enableFileLogging = false;
    private string _logFilePath = "user://logs/game.log";
    private FileAccess? _logFile;
    private Dictionary<LogLevel, Color> _logColors = new(DefaultLogColors);
    private readonly object _lock = new();
    private bool _disposed;
    
    public GodotGameLogger(string categoryName)
    {
        _categoryName = categoryName;
        SetupFileLogging();
    }
    
    public void LogDebug(string message, params object[] args)
    {
        Log(LogLevel.Debug, message, args);
    }
    
    public void LogInformation(string message, params object[] args)
    {
        Log(LogLevel.Information, message, args);
    }
    
    public void LogWarning(string message, params object[] args)
    {
        Log(LogLevel.Warning, message, args);
    }
    
    public void LogError(string message, params object[] args)
    {
        Log(LogLevel.Error, message, args);
    }
    
    public void LogError(Exception exception, string message, params object[] args)
    {
        var fullMessage = args.Length > 0 ? string.Format(message, args) : message;
        fullMessage += $"\nException: {exception}";
        Log(LogLevel.Error, fullMessage);
    }
    
    public void LogCritical(string message, params object[] args)
    {
        Log(LogLevel.Critical, message, args);
    }
    
    public void LogCritical(Exception exception, string message, params object[] args)
    {
        var fullMessage = args.Length > 0 ? string.Format(message, args) : message;
        fullMessage += $"\nException: {exception}";
        Log(LogLevel.Critical, fullMessage);
    }
    
    public void SetLevel(LogLevel level)
    {
        _currentLevel = level;
    }
    
    public void EnableFileLogging(bool enable)
    {
        lock (_lock)
        {n            _enableFileLogging = enable;
            if (enable)
            {
                SetupFileLogging();
            }
            else
            {
                _logFile?.Close();
                _logFile = null;
            }
        }
    }
    
    public void SetLogFilePath(string path)
    {
        lock (_lock)
        {
            _logFilePath = path;
            if (_enableFileLogging)
            {
                _logFile?.Close();
                SetupFileLogging();
            }
        }
    }
    
    private void Log(LogLevel level, string message, params object[] args)
    {
        if (level < _currentLevel || _disposed) return;
        
        try
        {n            var formattedMessage = args.Length > 0 ? string.Format(message, args) : message;
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] [{level}] [{_categoryName}] {formattedMessage}";
            
            // 控制台输出
            if (_logColors.TryGetValue(level, out var color))
            {
                GD.PrintRich($"[color={color.ToHtml()}]{logMessage}[/color]");
            }
            else
            {
                GD.Print(logMessage);
            }
            
            // 文件输出
            if (_enableFileLogging)
            {
                WriteToFile(logMessage);
            }
        }
        catch (Exception ex)
        {n            GD.PrintErr($"Logging error: {ex.Message}");
        }
    }
    
    private void SetupFileLogging()
    {
        if (!_enableFileLogging) return;
        
        try
        {n            var logDir = _logFilePath.GetBaseDir();
            if (!DirAccess.DirExistsAbsolute(logDir))
            {
                DirAccess.MakeDirRecursiveAbsolute(logDir);
            }
            
            _logFile = FileAccess.Open(_logFilePath, FileAccess.ModeFlags.Write);
            if (_logFile != null)
            {
                _logFile.SeekEnd();
            }
        }
        catch (Exception ex)
        {n            GD.PrintErr($"Failed to setup file logging: {ex.Message}");
            _enableFileLogging = false;
        }
    }
    
    private void WriteToFile(string message)
    {
        lock (_lock)
        {
            try
            {n                _logFile?.StoreLine(message);
                _logFile?.Flush();
            }
            catch (Exception ex)
            {n                GD.PrintErr($"Failed to write to log file: {ex.Message}");
            }
        }
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        lock (_lock)
        {
            _logFile?.Close();
            _logFile = null;
            _disposed = true;
        }
    }
}

/// <summary>
/// 泛型游戏日志实现
/// </summary>
/// <typeparam name="T">日志类别类型</typeparam>
public class GodotGameLogger<T> : GodotGameLogger, IGameLogger<T>
{
    public GodotGameLogger() : base(typeof(T).Name)
    {
    }
}