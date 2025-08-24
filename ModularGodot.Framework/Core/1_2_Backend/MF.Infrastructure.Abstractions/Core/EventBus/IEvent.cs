namespace MF.Infrastructure.Abstractions.EventBus;

/// <summary>
/// 事件基接口
/// </summary>
public interface IEvent
{
    /// <summary>
    /// 事件ID
    /// </summary>
    string EventId { get; }
    
    /// <summary>
    /// 事件时间戳
    /// </summary>
    DateTime Timestamp { get; }
    
    /// <summary>
    /// 事件源
    /// </summary>
    string Source { get; }
    
    /// <summary>
    /// 关联ID
    /// </summary>
    string CorrelationId { get; }
}

/// <summary>
/// 事件基类
/// </summary>
public abstract class EventBase : IEvent
{
    public string EventId { get; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public virtual string Source { get; protected set; } = "Unknown";
    public string CorrelationId { get; set; } = string.Empty;
    
    protected EventBase(string? source = null)
    {
        Source = source ?? GetType().Name;
    }
}