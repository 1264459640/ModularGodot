namespace MF.Data.Core.Interfaces;

/// <summary>
/// 数据上下文接口
/// </summary>
public interface IDataContext : IDisposable
{
    /// <summary>
    /// 保存所有更改
    /// </summary>
    /// <returns>受影响的行数</returns>
    Task<int> SaveChangesAsync();
    
    /// <summary>
    /// 开始事务
    /// </summary>
    /// <returns>事务对象</returns>
    Task<IDataTransaction> BeginTransactionAsync();
    
    /// <summary>
    /// 获取实体集合
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns>实体集合</returns>
    IQueryable<T> Set<T>() where T : class;
    
    /// <summary>
    /// 添加实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns>添加的实体</returns>
    T Add<T>(T entity) where T : class;
    
    /// <summary>
    /// 更新实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns>更新的实体</returns>
    T Update<T>(T entity) where T : class;
    
    /// <summary>
    /// 删除实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    void Remove<T>(T entity) where T : class;
}

/// <summary>
/// 数据事务接口
/// </summary>
public interface IDataTransaction : IDisposable
{
    /// <summary>
    /// 提交事务
    /// </summary>
    Task CommitAsync();
    
    /// <summary>
    /// 回滚事务
    /// </summary>
    Task RollbackAsync();
}