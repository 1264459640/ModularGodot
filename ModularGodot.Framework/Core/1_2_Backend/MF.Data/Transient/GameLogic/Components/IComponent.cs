using MF.Data.Core.Attributes;

namespace MF.Data.Transient.GameLogic.Components;

/// <summary>
/// ECS组件基接口
/// </summary>
public interface IComponent
{
    /// <summary>
    /// 组件类型ID
    /// </summary>
    int ComponentTypeId { get; }
    
    /// <summary>
    /// 组件是否有效
    /// </summary>
    bool IsValid { get; }
    
    /// <summary>
    /// 重置组件到默认状态
    /// </summary>
    void Reset();
    
    /// <summary>
    /// 复制组件数据
    /// </summary>
    IComponent Clone();
    
    /// <summary>
    /// 序列化组件数据
    /// </summary>
    /// <returns>序列化后的数据</returns>
    string Serialize();
    
    /// <summary>
    /// 反序列化组件数据
    /// </summary>
    /// <param name="data">序列化的数据</param>
    void Deserialize(string data);
}

/// <summary>
/// 组件基类
/// </summary>
public abstract class ComponentBase : IComponent
{
    public abstract int ComponentTypeId { get; }
    public virtual bool IsValid => true;
    
    public virtual void Reset() { }
    public abstract IComponent Clone();
    
    public virtual string Serialize()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, GetType());
    }
    
    public virtual void Deserialize(string data)
    {
        if (string.IsNullOrEmpty(data)) return;
        
        try
        {
            var deserialized = System.Text.Json.JsonSerializer.Deserialize(data, GetType());
            if (deserialized != null)
            {
                CopyFrom(deserialized);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to deserialize component {GetType().Name}: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// 从另一个对象复制数据
    /// </summary>
    /// <param name="source">源对象</param>
    protected virtual void CopyFrom(object source)
    {
        var sourceType = source.GetType();
        var targetType = GetType();
        
        if (sourceType != targetType)
            throw new ArgumentException($"Source type {sourceType.Name} does not match target type {targetType.Name}");
        
        var properties = targetType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        
        foreach (var property in properties)
        {
            if (property.CanRead && property.CanWrite)
            {
                var value = property.GetValue(source);
                property.SetValue(this, value);
            }
        }
    }
}

/// <summary>
/// 可更新组件接口
/// </summary>
public interface IUpdatableComponent : IComponent
{
    /// <summary>
    /// 更新组件
    /// </summary>
    /// <param name="deltaTime">时间间隔</param>
    void Update(float deltaTime);
}

/// <summary>
/// 可渲染组件接口
/// </summary>
public interface IRenderableComponent : IComponent
{
    /// <summary>
    /// 是否可见
    /// </summary>
    bool IsVisible { get; set; }
    
    /// <summary>
    /// 渲染层级
    /// </summary>
    int RenderLayer { get; set; }
    
    /// <summary>
    /// 渲染优先级
    /// </summary>
    int RenderPriority { get; set; }
}

/// <summary>
/// 可持久化组件接口
/// </summary>
public interface IPersistentComponent : IComponent
{
    /// <summary>
    /// 是否需要保存
    /// </summary>
    bool IsDirty { get; set; }
    
    /// <summary>
    /// 最后保存时间
    /// </summary>
    DateTime LastSaved { get; set; }
    
    /// <summary>
    /// 保存组件数据
    /// </summary>
    /// <returns>保存任务</returns>
    Task SaveAsync();
    
    /// <summary>
    /// 加载组件数据
    /// </summary>
    /// <returns>加载任务</returns>
    Task LoadAsync();
}

/// <summary>
/// 组件事件参数
/// </summary>
public class ComponentEventArgs : EventArgs
{
    /// <summary>
    /// 实体ID
    /// </summary>
    public uint EntityId { get; set; }
    
    /// <summary>
    /// 组件实例
    /// </summary>
    public IComponent Component { get; set; } = null!;
    
    /// <summary>
    /// 组件类型
    /// </summary>
    public Type ComponentType { get; set; } = null!;
    
    /// <summary>
    /// 事件时间
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}