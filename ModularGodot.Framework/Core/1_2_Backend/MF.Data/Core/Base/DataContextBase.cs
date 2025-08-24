using MF.Data.Core.Interfaces;

namespace MF.Data.Core.Base;

/// <summary>
/// 数据上下文基类
/// </summary>
public abstract class DataContextBase : IDataContext
{
    private readonly Dictionary<Type, object> _sets = new();
    private readonly List<object> _addedEntities = new();
    private readonly List<object> _updatedEntities = new();
    private readonly List<object> _removedEntities = new();
    private bool _disposed;
    
    public virtual async Task<int> SaveChangesAsync()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(DataContextBase));
        
        var changeCount = 0;
        
        // 处理新增实体
        foreach (var entity in _addedEntities)
        {
            await ProcessAddedEntityAsync(entity);
            changeCount++;
        }
        
        // 处理更新实体
        foreach (var entity in _updatedEntities)
        {
            await ProcessUpdatedEntityAsync(entity);
            changeCount++;
        }
        
        // 处理删除实体
        foreach (var entity in _removedEntities)
        {
            await ProcessRemovedEntityAsync(entity);
            changeCount++;
        }
        
        // 清空变更跟踪
        _addedEntities.Clear();
        _updatedEntities.Clear();
        _removedEntities.Clear();
        
        return changeCount;
    }
    
    public virtual async Task<IDataTransaction> BeginTransactionAsync()
    {
        return new DataTransaction(this);
    }
    
    public virtual IQueryable<T> Set<T>() where T : class
    {
        var type = typeof(T);
        
        if (!_sets.TryGetValue(type, out var set))
        {
            set = CreateSet<T>();
            _sets[type] = set;
        }
        
        return (IQueryable<T>)set;
    }
    
    public virtual T Add<T>(T entity) where T : class
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        
        _addedEntities.Add(entity);
        
        // 如果是BaseEntity，设置创建时间
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.CreatedAt = DateTime.UtcNow;
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }
        
        return entity;
    }
    
    public virtual T Update<T>(T entity) where T : class
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        
        _updatedEntities.Add(entity);
        
        // 如果是BaseEntity，设置更新时间
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }
        
        return entity;
    }
    
    public virtual void Remove<T>(T entity) where T : class
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        
        _removedEntities.Add(entity);
        
        // 如果是BaseEntity，设置删除时间（软删除）
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.IsDeleted = true;
            baseEntity.DeletedAt = DateTime.UtcNow;
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }
    }
    
    /// <summary>
    /// 创建实体集合
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>实体集合</returns>
    protected abstract IQueryable<T> CreateSet<T>() where T : class;
    
    /// <summary>
    /// 处理新增实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>处理任务</returns>
    protected abstract Task ProcessAddedEntityAsync(object entity);
    
    /// <summary>
    /// 处理更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>处理任务</returns>
    protected abstract Task ProcessUpdatedEntityAsync(object entity);
    
    /// <summary>
    /// 处理删除实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>处理任务</returns>
    protected abstract Task ProcessRemovedEntityAsync(object entity);
    
    public virtual void Dispose()
    {
        if (_disposed) return;
        
        _sets.Clear();
        _addedEntities.Clear();
        _updatedEntities.Clear();
        _removedEntities.Clear();
        
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// 数据事务实现
/// </summary>
internal class DataTransaction : IDataTransaction
{
    private readonly DataContextBase _context;
    private bool _disposed;
    private bool _committed;
    
    public DataTransaction(DataContextBase context)
    {
        _context = context;
    }
    
    public async Task CommitAsync()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(DataTransaction));
        
        if (_committed)
            throw new InvalidOperationException("Transaction has already been committed.");
        
        await _context.SaveChangesAsync();
        _committed = true;
    }
    
    public async Task RollbackAsync()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(DataTransaction));
        
        // 在基类实现中，回滚就是不保存更改
        // 具体实现可以重写此方法
        await Task.CompletedTask;
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        if (!_committed)
        {
            // 如果事务没有提交，自动回滚
            _ = RollbackAsync();
        }
        
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}