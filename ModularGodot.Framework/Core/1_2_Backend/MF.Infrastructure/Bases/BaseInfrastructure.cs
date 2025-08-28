using System;
using System.Threading;

namespace MF.Infrastructure.Bases;

/// <summary>
/// 基础设施层的抽象基类，提供通用的资源管理和生命周期控制功能
/// </summary>
public abstract class BaseInfrastructure : IDisposable
{
    private bool _disposed; // 释放标记
    protected readonly CancellationTokenSource CancellationTokenSource = new();

    // 实现 IDisposable.Dispose()
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // 受保护的虚方法，支持派生类扩展
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        Unsubscribe();
        CancellationTokenSource.Cancel();
        CancellationTokenSource.Dispose();

        _disposed = true;
    }

    /// <summary>
    /// 取消订阅事件，派生类应重写此方法以取消订阅相关事件
    /// </summary>
    protected virtual void Unsubscribe() { }

    // 终结器（析构函数），用于未显式调用 Dispose 时的补救
    ~BaseInfrastructure()
    {
        Dispose(false);
    }
}