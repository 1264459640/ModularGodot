using Microsoft.Extensions.DependencyInjection;
using MF.Services.Abstractions.Core.ResourceManagement;
using MF.Data.Transient.Infrastructure.Monitoring;
using MF.Services.Core.ResourceManagement;

namespace MF.Services.Extensions;

/// <summary>
/// 资源管理系统服务注册扩展
/// </summary>
public static class ResourceManagementServiceExtensions
{
    /// <summary>
    /// 添加资源管理系统服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddResourceManagement(
        this IServiceCollection services,
        Action<ResourceSystemConfig>? configureOptions = null)
    {
        // 注册配置
        var config = new ResourceSystemConfig();
        configureOptions?.Invoke(config);
        services.AddSingleton(config);
        
        // 注册主组件
        services.AddSingleton<ResourceManager>();
        
        // 注册接口
        services.AddSingleton<IResourceCacheService>(provider => provider.GetRequiredService<ResourceManager>());
        services.AddSingleton<IResourceMonitorService>(provider => provider.GetRequiredService<ResourceManager>());
        
        return services;
    }
    
    /// <summary>
    /// 添加资源管理系统服务（使用现有配置实例）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="config">配置实例</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddResourceManagement(
        this IServiceCollection services,
        ResourceSystemConfig config)
    {
        // 注册配置实例
        services.AddSingleton(config);
        
        // 注册主组件
        services.AddSingleton<ResourceManager>();
        
        // 注册接口
        services.AddSingleton<IResourceCacheService>(provider => provider.GetRequiredService<ResourceManager>());
        services.AddSingleton<IResourceMonitorService>(provider => provider.GetRequiredService<ResourceManager>());
        
        return services;
    }
}