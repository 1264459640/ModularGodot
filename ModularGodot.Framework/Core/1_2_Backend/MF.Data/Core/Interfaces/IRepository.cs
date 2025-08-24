using System.Linq.Expressions;

namespace MF.Data.Core.Interfaces;

/// <summary>
/// 通用仓储接口
/// </summary>
public interface IRepository<T, TKey> where T : class
{
    Task<T?> GetByIdAsync(TKey id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(TKey id);
    Task<int> SaveChangesAsync();
}

/// <summary>
/// ECS组件仓储接口
/// </summary>
public interface IComponentRepository
{
    Task<T?> GetComponentAsync<T>(uint entityId) where T : IComponent;
    Task<bool> HasComponentAsync<T>(uint entityId) where T : IComponent;
    Task AddComponentAsync<T>(uint entityId, T component) where T : IComponent;
    Task RemoveComponentAsync<T>(uint entityId) where T : IComponent;
    Task<IEnumerable<uint>> GetEntitiesWithAsync<T>() where T : IComponent;
    Task<IEnumerable<uint>> GetEntitiesWithAsync(params Type[] componentTypes);
}

/// <summary>
/// 游戏状态仓储接口
/// </summary>
public interface IGameStateRepository
{
    Task<GameSession?> GetCurrentSessionAsync();
    Task SaveSessionAsync(GameSession session);
    Task<PlayerState?> GetPlayerStateAsync(string playerId);
    Task SavePlayerStateAsync(PlayerState playerState);
    Task<SceneState?> GetSceneStateAsync(string sceneName);
    Task SaveSceneStateAsync(SceneState sceneState);
}