namespace MF.Data.Core.Attributes;

/// <summary>
/// 表映射属性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TableAttribute : Attribute
{
    /// <summary>
    /// 表名
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 架构名
    /// </summary>
    public string? Schema { get; set; }
    
    /// <summary>
    /// 表描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 是否为临时表
    /// </summary>
    public bool IsTemporary { get; set; }
    
    public TableAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

/// <summary>
/// 列映射属性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ColumnAttribute : Attribute
{
    /// <summary>
    /// 列名
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 数据类型
    /// </summary>
    public string? DataType { get; set; }
    
    /// <summary>
    /// 是否为主键
    /// </summary>
    public bool IsPrimaryKey { get; set; }
    
    /// <summary>
    /// 是否允许空值
    /// </summary>
    public bool IsNullable { get; set; } = true;
    
    /// <summary>
    /// 是否自动增长
    /// </summary>
    public bool IsIdentity { get; set; }
    
    /// <summary>
    /// 默认值
    /// </summary>
    public object? DefaultValue { get; set; }
    
    /// <summary>
    /// 列描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 列长度
    /// </summary>
    public int Length { get; set; } = -1;
    
    /// <summary>
    /// 精度（用于数值类型）
    /// </summary>
    public int Precision { get; set; } = -1;
    
    /// <summary>
    /// 小数位数（用于数值类型）
    /// </summary>
    public int Scale { get; set; } = -1;
    
    public ColumnAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

/// <summary>
/// 索引属性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class IndexAttribute : Attribute
{
    /// <summary>
    /// 索引名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 是否唯一索引
    /// </summary>
    public bool IsUnique { get; set; }
    
    /// <summary>
    /// 是否聚集索引
    /// </summary>
    public bool IsClustered { get; set; }
    
    /// <summary>
    /// 索引列（多列索引时使用）
    /// </summary>
    public string[]? Columns { get; set; }
    
    /// <summary>
    /// 排序顺序
    /// </summary>
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
    
    public IndexAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

/// <summary>
/// 排序顺序
/// </summary>
public enum SortOrder
{
    /// <summary>
    /// 升序
    /// </summary>
    Ascending,
    
    /// <summary>
    /// 降序
    /// </summary>
    Descending
}

/// <summary>
/// 外键属性
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ForeignKeyAttribute : Attribute
{
    /// <summary>
    /// 引用的表名
    /// </summary>
    public string ReferencedTable { get; }
    
    /// <summary>
    /// 引用的列名
    /// </summary>
    public string ReferencedColumn { get; set; } = "Id";
    
    /// <summary>
    /// 删除时的操作
    /// </summary>
    public ForeignKeyAction OnDelete { get; set; } = ForeignKeyAction.Restrict;
    
    /// <summary>
    /// 更新时的操作
    /// </summary>
    public ForeignKeyAction OnUpdate { get; set; } = ForeignKeyAction.Restrict;
    
    public ForeignKeyAttribute(string referencedTable)
    {
        ReferencedTable = referencedTable ?? throw new ArgumentNullException(nameof(referencedTable));
    }
}

/// <summary>
/// 外键操作
/// </summary>
public enum ForeignKeyAction
{
    /// <summary>
    /// 限制
    /// </summary>
    Restrict,
    
    /// <summary>
    /// 级联
    /// </summary>
    Cascade,
    
    /// <summary>
    /// 设置为空
    /// </summary>
    SetNull,
    
    /// <summary>
    /// 设置为默认值
    /// </summary>
    SetDefault,
    
    /// <summary>
    /// 无操作
    /// </summary>
    NoAction
}