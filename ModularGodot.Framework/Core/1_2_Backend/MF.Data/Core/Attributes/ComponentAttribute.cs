namespace MF.Data.Core.Attributes;

/// <summary>
/// 组件标记属性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class ComponentAttribute : Attribute
{
    /// <summary>
    /// 组件名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 组件类型ID
    /// </summary>
    public int TypeId { get; set; } = -1;
    
    /// <summary>
    /// 组件描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 是否可序列化
    /// </summary>
    public bool IsSerializable { get; set; } = true;
    
    /// <summary>
    /// 是否可持久化
    /// </summary>
    public bool IsPersistent { get; set; } = false;
    
    /// <summary>
    /// 组件分组
    /// </summary>
    public string? Group { get; set; }
    
    /// <summary>
    /// 组件优先级
    /// </summary>
    public int Priority { get; set; } = 0;
    
    /// <summary>
    /// 是否为单例组件（每个实体只能有一个）
    /// </summary>
    public bool IsSingleton { get; set; } = true;
    
    /// <summary>
    /// 依赖的组件类型
    /// </summary>
    public Type[]? Dependencies { get; set; }
    
    /// <summary>
    /// 互斥的组件类型
    /// </summary>
    public Type[]? Conflicts { get; set; }
    
    public ComponentAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

/// <summary>
/// 系统标记属性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SystemAttribute : Attribute
{
    /// <summary>
    /// 系统名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 系统描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 系统执行顺序
    /// </summary>
    public int ExecutionOrder { get; set; } = 0;
    
    /// <summary>
    /// 系统分组
    /// </summary>
    public string? Group { get; set; }
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// 是否并行执行
    /// </summary>
    public bool IsParallel { get; set; } = false;
    
    /// <summary>
    /// 处理的组件类型
    /// </summary>
    public Type[]? ComponentTypes { get; set; }
    
    /// <summary>
    /// 系统依赖
    /// </summary>
    public Type[]? Dependencies { get; set; }
    
    /// <summary>
    /// 执行阶段
    /// </summary>
    public SystemPhase Phase { get; set; } = SystemPhase.Update;
    
    public SystemAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

/// <summary>
/// 系统执行阶段
/// </summary>
public enum SystemPhase
{
    /// <summary>
    /// 初始化阶段
    /// </summary>
    Initialize,
    
    /// <summary>
    /// 输入处理阶段
    /// </summary>
    Input,
    
    /// <summary>
    /// 逻辑更新阶段
    /// </summary>
    Update,
    
    /// <summary>
    /// 物理更新阶段
    /// </summary>
    Physics,
    
    /// <summary>
    /// 渲染阶段
    /// </summary>
    Render,
    
    /// <summary>
    /// 后处理阶段
    /// </summary>
    PostProcess,
    
    /// <summary>
    /// 清理阶段
    /// </summary>
    Cleanup
}

/// <summary>
/// 实体标记属性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EntityAttribute : Attribute
{
    /// <summary>
    /// 实体名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 实体描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 实体分组
    /// </summary>
    public string? Group { get; set; }
    
    /// <summary>
    /// 是否可序列化
    /// </summary>
    public bool IsSerializable { get; set; } = true;
    
    /// <summary>
    /// 是否可持久化
    /// </summary>
    public bool IsPersistent { get; set; } = false;
    
    /// <summary>
    /// 默认组件类型
    /// </summary>
    public Type[]? DefaultComponents { get; set; }
    
    /// <summary>
    /// 必需的组件类型
    /// </summary>
    public Type[]? RequiredComponents { get; set; }
    
    public EntityAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}