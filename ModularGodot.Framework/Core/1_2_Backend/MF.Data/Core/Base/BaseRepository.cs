using System.Linq.Expressions;
using MF.Data.Core.Interfaces;

namespace MF.Data.Core.Base;

/// <summary>
/// 仓储基类
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class BaseRepository<T, TKey> : IRepository<T, TKey> where T : class
{
    protected readonly IDataContext _context;
    
    protected BaseRepository(IDataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public virtual async Task<T?> GetByIdAsync(TKey id)
    {
        if (id == null) return null;
        
        var entity = await FindByIdInternalAsync(id);
        return entity;
    }
    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Task.FromResult(_context.Set<T>().AsEnumerable());
    }
    
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));
        
        return await Task.FromResult(_context.Set<T>().Where(predicate).AsEnumerable());
    }
    
    public virtual async Task<T> AddAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        
        var addedEntity = _context.Add(entity);
        await _context.SaveChangesAsync();
        return addedEntity;
    }
    
    public virtual async Task<T> UpdateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        
        var updatedEntity = _context.Update(entity);
        await _context.SaveChangesAsync();
        return updatedEntity;
    }
    
    public virtual async Task<bool> DeleteAsync(TKey id)
    {
        if (id == null) return false;
        
        var entity = await FindByIdInternalAsync(id);
        if (entity == null) return false;
        
        _context.Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    
    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// 根据ID查找实体的内部实现
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>实体对象</returns>
    protected abstract Task<T?> FindByIdInternalAsync(TKey id);
    
    /// <summary>
    /// 获取查询集合
    /// </summary>
    /// <returns>查询集合</returns>
    protected virtual IQueryable<T> GetQueryable()
    {
        return _context.Set<T>();
    }
    
    /// <summary>
    /// 应用包含导航属性
    /// </summary>
    /// <param name="query">查询</param>
    /// <returns>包含导航属性的查询</returns>
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query)
    {
        return query;
    }
    
    /// <summary>
    /// 应用排序
    /// </summary>
    /// <param name="query">查询</param>
    /// <returns>排序后的查询</returns>
    protected virtual IQueryable<T> ApplyOrdering(IQueryable<T> query)
    {
        return query;
    }
}