# ModularGodot 扩展系统 (Extensions System)

这是 ModularGodot 框架的扩展系统，允许开发者以模块化的方式添加功能，同时保持核心架构的完整性。

## 🎯 设计原则

- **架构完整性**：严格遵循现有的DDD三层架构
- **松耦合**：扩展之间通过接口和事件通信
- **生命周期管理**：完整的扩展生命周期管理
- **依赖注入**：与Autofac容器深度集成
- **扩展点**：支持扩展间的交互和监听

## 📁 目录结构

```
Extensions/
├── AudioSystem/                    # 音频系统扩展
│   ├── Services/
│   │   ├── Abstractions/
│   │   └── AudioManagerService.cs
│   ├── Repositories/
│   │   ├── Abstractions/
│   │   └── AudioManagerRepo.cs
│   ├── Commands/
│   ├── CommandHandlers/
│   ├── Examples/
│   ├── AudioSystemExtension.cs     # 扩展入口
│   ├── AudioSystem.csproj          # 项目文件
│   └── README.md                   # 扩展文档
└── README.md                       # 本文件
```

## 🚀 快速开始

### 1. 使用现有扩展

```csharp
public partial class Main : Node
{
    public override void _Ready()
    {
        // 注册音频系统扩展
        ExtendedContexts.Instance.RegisterExtension<AudioSystemExtension>();
        
        // 获取扩展提供的服务
        var audioService = ExtendedContexts.Instance.GetService<IAudioManagerService>();
        
        // 使用服务
        audioService.PlaySound("res://audio/click.wav");
    }
}
```

### 2. 创建自定义扩展

#### 步骤1：创建扩展项目

```xml
<!-- MyExtension.csproj -->
<Project Sdk="Godot.NET.Sdk/4.4.1">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>Extensions.MyExtension</RootNamespace>
  </PropertyGroup>

  <!-- 引用Core框架 -->
  <ItemGroup>
    <ProjectReference Include="..\..\Core\TO.Commons\TO.Commons.csproj" />
    <ProjectReference Include="..\..\Core\TO.Contexts\TO.Contexts.csproj" />
    <!-- 其他必要的引用 -->
  </ItemGroup>

  <!-- 必要的NuGet包 -->
  <ItemGroup>
    <PackageReference Include="Autofac" Version="8.3.0" />
    <PackageReference Include="MediatR" Version="12.5.0" />
  </ItemGroup>
</Project>
```

#### 步骤2：创建扩展类

```csharp
using TO.Commons.Abstractions;
using Autofac;
using System.Reflection;

namespace Extensions.MyExtension;

public class MyExtension : FrameworkExtensionBase
{
    public override string Name => "My Custom Extension";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "我的自定义扩展";
    public override ExtensionPriority Priority => ExtensionPriority.Normal;
    
    public override void ConfigureServices(ContainerBuilder builder)
    {
        // 自动注册服务
        var assembly = Assembly.GetExecutingAssembly();
        
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => t.Name.EndsWith("Service") && !t.IsAbstract)
            .AsImplementedInterfaces()
            .SingleInstance();
            
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => t.Name.EndsWith("Repo") && !t.IsAbstract)
            .AsImplementedInterfaces()
            .SingleInstance();
    }
    
    public override void Initialize(IExtensionContext context)
    {
        base.Initialize(context);
        
        // 注册扩展点
        RegisterExtensionPoint<IMyExtensionPoint>(new MyExtensionPoint());
    }
    
    public override void OnStarted(IContainer container)
    {
        base.OnStarted(container);
        
        // 扩展启动逻辑
        var myService = container.Resolve<IMyService>();
        myService.Initialize();
    }
}
```

#### 步骤3：创建服务和仓储

```csharp
// Services/Abstractions/IMyService.cs
public interface IMyService
{
    void Initialize();
    void DoSomething();
}

// Services/MyService.cs
public class MyService : BaseService, IMyService
{
    public void Initialize()
    {
        GD.Print("My service initialized");
    }
    
    public void DoSomething()
    {
        GD.Print("Doing something...");
    }
}
```

## 🔌 扩展点系统

扩展点允许扩展之间进行通信和交互：

### 定义扩展点

```csharp
public interface IMyExtensionPoint
{
    void OnSomethingHappened(string data);
}

public class MyExtensionPoint : IMyExtensionPoint
{
    public void OnSomethingHappened(string data)
    {
        GD.Print($"Something happened: {data}");
    }
}
```

### 注册和使用扩展点

```csharp
// 在扩展中注册扩展点
public override void Initialize(IExtensionContext context)
{
    RegisterExtensionPoint<IMyExtensionPoint>(new MyExtensionPoint());
}

// 在其他地方使用扩展点
var extensionPoints = ExtensionManager.Instance.GetExtensionPoints<IMyExtensionPoint>();
foreach (var point in extensionPoints)
{
    point.OnSomethingHappened("Hello from extension!");
}
```

## 📋 扩展生命周期

1. **注册阶段**：`RegisterExtension<T>()`
2. **配置阶段**：`ConfigureServices(ContainerBuilder builder)`
3. **初始化阶段**：`Initialize(IExtensionContext context)`
4. **启动阶段**：`OnStarted(IContainer container)`
5. **运行阶段**：扩展正常工作
6. **停止阶段**：`OnStopping(IContainer container)`
7. **释放阶段**：`Dispose()`

## 🛠️ 最佳实践

### 1. 项目组织
- 每个扩展一个独立的csproj项目
- 按功能模块组织代码（Services、Repositories、Commands等）
- 提供清晰的README文档

### 2. 依赖管理
- 只引用必要的Core项目
- 避免扩展之间的直接依赖
- 通过扩展点进行扩展间通信

### 3. 服务注册
- 使用自动注册减少样板代码
- 遵循命名约定（Service、Repo结尾）
- 正确设置服务生命周期

### 4. 错误处理
- 在扩展方法中添加适当的异常处理
- 使用日志记录重要事件
- 不要让扩展错误影响核心系统

### 5. 性能考虑
- 避免在扩展点中执行耗时操作
- 合理使用缓存
- 注意内存泄漏问题

## 📚 现有扩展

### AudioSystem 扩展
- **功能**：完整的音频管理系统
- **特性**：背景音乐、音效、语音、音量控制、静音
- **文档**：[AudioSystem/README.md](AudioSystem/README.md)

## 🔧 开发工具

### 扩展模板
可以创建扩展模板来快速生成新扩展的基础结构：

```bash
# 未来可以添加的工具
dotnet new extension -n MyExtension -o Extensions/MyExtension
```

### 调试支持
- 扩展加载日志
- 服务注册验证
- 生命周期事件跟踪

## 🚨 注意事项

1. **扩展顺序**：扩展按优先级加载，高优先级先加载
2. **循环依赖**：避免扩展之间的循环依赖
3. **版本兼容**：确保扩展与Core框架版本兼容
4. **资源清理**：在Dispose方法中正确清理资源
5. **线程安全**：注意多线程环境下的线程安全问题

## 🎯 未来计划

- [ ] 扩展热加载支持
- [ ] 扩展配置系统
- [ ] 扩展依赖管理
- [ ] 扩展市场和分发
- [ ] 可视化扩展管理器
- [ ] 扩展性能监控

---

通过这个扩展系统，ModularGodot 框架实现了"稳定的核心 + 灵活的扩展"的架构模式，既保证了核心功能的稳定性，又提供了强大的扩展能力。