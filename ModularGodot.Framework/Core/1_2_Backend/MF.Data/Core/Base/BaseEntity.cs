using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MF.Data.Core.Base;

/// <summary>
/// 实体基类
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 更新时间
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 是否已删除（软删除）
    /// </summary>
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
    
    /// <summary>
    /// 删除时间
    /// </summary>
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// 版本号（用于乐观锁）
    /// </summary>
    [Column("version")]
    [Timestamp]
    public byte[]? Version { get; set; }
}

/// <summary>
/// 带主键的实体基类
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class BaseEntity<TKey> : BaseEntity
{
    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    [Column("id")]
    public virtual TKey Id { get; set; } = default!;
}

/// <summary>
/// 字符串主键实体基类
/// </summary>
public abstract class StringKeyEntity : BaseEntity<string>
{
    protected StringKeyEntity()
    {
        Id = Guid.NewGuid().ToString();
    }
    
    protected StringKeyEntity(string id)
    {
        Id = id;
    }
}

/// <summary>
/// 整数主键实体基类
/// </summary>
public abstract class IntKeyEntity : BaseEntity<int>
{
    // 整数主键通常由数据库自动生成
}

/// <summary>
/// GUID主键实体基类
/// </summary>
public abstract class GuidKeyEntity : BaseEntity<Guid>
{
    protected GuidKeyEntity()
    {
        Id = Guid.NewGuid();
    }
    
    protected GuidKeyEntity(Guid id)
    {
        Id = id;
    }
}

/// <summary>
/// 审计实体基类
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// 创建者ID
    /// </summary>
    [Column("created_by")]
    [MaxLength(50)]
    public string? CreatedBy { get; set; }
    
    /// <summary>
    /// 更新者ID
    /// </summary>
    [Column("updated_by")]
    [MaxLength(50)]
    public string? UpdatedBy { get; set; }
    
    /// <summary>
    /// 删除者ID
    /// </summary>
    [Column("deleted_by")]
    [MaxLength(50)]
    public string? DeletedBy { get; set; }
}

/// <summary>
/// 带审计的主键实体基类
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class AuditableEntity<TKey> : AuditableEntity
{
    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    [Column("id")]
    public virtual TKey Id { get; set; } = default!;
}