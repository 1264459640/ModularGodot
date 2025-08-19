# 批量创建扩展的PowerShell脚本
# 用于将Core中的系统模块转换为扩展

param(
    [string]$ExtensionsPath = "..\Extensions"
)

# 定义要创建的扩展列表
$extensions = @(
    @{
        Name = "UISystem"
        Description = "Provides complete UI management functionality, including UI display, hiding, layer management, lifecycle management and animation system"
        Priority = "High"
        Systems = @("UIManagerService", "UILayerService", "UILifecycleService", "UiAnimationService")
    },
    @{
        Name = "SceneSystem"
        Description = "Provides scene management functionality, including scene switching, transition effects and scene lifecycle management"
        Priority = "High"
        Systems = @("SceneManagerService")
    },
    @{
        Name = "ResourceSystem"
        Description = "Provides resource management functionality, including resource loading, caching, memory monitoring and reference tracking"
        Priority = "High"
        Systems = @("ResourceLoaderRepo", "CacheManager", "MemoryMonitor")
    },
    @{
        Name = "EventBusSystem"
        Description = "Provides event bus functionality, supporting publish-subscribe pattern event communication"
        Priority = "High"
        Systems = @("EventBusRepo")
    },
    @{
        Name = "LogSystem"
        Description = "Provides logging functionality, supporting multi-level logging and formatted output"
        Priority = "High"
        Systems = @("LoggerRepo")
    },
    @{
        Name = "SequenceSystem"
        Description = "Provides sequence management functionality, supporting timeline and action sequences"
        Priority = "Medium"
        Systems = @("SequenceManagerService")
    },
    @{
        Name = "SerializationSystem"
        Description = "Provides serialization functionality, supporting game data saving and loading"
        Priority = "Medium"
        Systems = @("SaveManagerService")
    },
    @{
        Name = "GameAbilitySystem"
        Description = "Provides game ability system, including attribute management, effect system and skill system"
        Priority = "Medium"
        Systems = @("AttributeManagerService", "AbilityEffectService")
    },
    @{
        Name = "ReadTableSystem"
        Description = "Provides data table reading functionality, supporting attribute and effect data caching and querying"
        Priority = "Low"
        Systems = @("AttributeDatabaseReadService", "GameplayEffectDatabaseReadService")
    }
)

# 创建扩展目录
function Create-ExtensionDirectory {
    param([string]$ExtensionName)
    
    $extensionPath = Join-Path $ExtensionsPath $ExtensionName
    if (-not (Test-Path $extensionPath)) {
        New-Item -ItemType Directory -Path $extensionPath -Force | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $extensionPath "Services") -Force | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $extensionPath "Services\Abstractions") -Force | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $extensionPath "Repositories") -Force | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $extensionPath "Repositories\Abstractions") -Force | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $extensionPath "Commands") -Force | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $extensionPath "CommandHandlers") -Force | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $extensionPath "Examples") -Force | Out-Null
    }
    
    return $extensionPath
}

# 创建项目文件
function Create-ProjectFile {
    param([string]$ExtensionPath, [string]$ExtensionName)
    
    $projectContent = @"
<Project Sdk="Godot.NET.Sdk/4.4.1">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>Extensions.$ExtensionName</RootNamespace>
  </PropertyGroup>

  <!-- 引用Core框架 -->
  <ItemGroup>
    <ProjectReference Include="..\..\Core\TO.Commons\TO.Commons.csproj" />
    <ProjectReference Include="..\..\Core\TO.Contexts\TO.Contexts.csproj" />
    <ProjectReference Include="..\..\Core\TO.Services.Abstractions\TO.Services.Abstractions.csproj" />
    <ProjectReference Include="..\..\Core\TO.Repositories.Abstractions\TO.Repositories.Abstractions.csproj" />
    <ProjectReference Include="..\..\Core\TO.Services\TO.Services.csproj" />
    <ProjectReference Include="..\..\Core\TO.Repositories\TO.Repositories.csproj" />
    <ProjectReference Include="..\..\Core\TO.Data\TO.Data.csproj" />
    <ProjectReference Include="..\..\Core\TO.Nodes.Abstractions\TO.Nodes.Abstractions.csproj" />
    <ProjectReference Include="..\..\Core\TO.Events\TO.Events.csproj" />
  </ItemGroup>

  <!-- 必要的NuGet包 -->
  <ItemGroup>
    <PackageReference Include="Autofac" Version="8.3.0" />
    <PackageReference Include="MediatR" Version="12.5.0" />
    <PackageReference Include="GDTask" Version="1.4.1" />
    <PackageReference Include="R3" Version="1.3.0" />
  </ItemGroup>

</Project>
"@
    
    $projectFile = Join-Path $ExtensionPath "$ExtensionName.csproj"
    Set-Content -Path $projectFile -Value $projectContent -Encoding UTF8
}

# 创建扩展主类
function Create-ExtensionClass {
    param([string]$ExtensionPath, [hashtable]$Extension)
    
    $extensionContent = @"
using System.Reflection;
using Autofac;
using Godot;
using TO.Commons.Abstractions;
using TO.Commons.Extensions;

namespace Extensions.$($Extension.Name);

/// <summary>
/// $($Extension.Name)扩展
/// </summary>
public class $($Extension.Name)Extension : FrameworkExtensionBase
{
    public override string Name => "$($Extension.Name) Extension";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "$($Extension.Description)";
    public override ExtensionPriority Priority => ExtensionPriority.$($Extension.Priority);
    
    public override void ConfigureServices(ContainerBuilder builder)
    {
        // 自动注册当前程序集中的所有服务
        var assembly = Assembly.GetExecutingAssembly();
        
        // 注册服务
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => t.Name.EndsWith("Service") && !t.IsAbstract)
            .AsImplementedInterfaces()
            .SingleInstance();
            
        // 注册仓储
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => t.Name.EndsWith("Repo") && !t.IsAbstract)
            .AsImplementedInterfaces()
            .SingleInstance();
            
        // 注册命令处理器
        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(MediatR.IRequestHandler<,>))
            .AsImplementedInterfaces();
            
        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(MediatR.IRequestHandler<>))
            .AsImplementedInterfaces();
        
        GD.Print("$($Extension.Name) Extension: Services configured");
    }
    
    public override void Initialize(IExtensionContext context)
    {
        base.Initialize(context);
        
        // 注册扩展点
        RegisterExtensionPoint<I$($Extension.Name)ExtensionPoint>(new $($Extension.Name)ExtensionPoint());
        
        GD.Print("$($Extension.Name) Extension: Initialized");
    }
    
    public override void OnStarted(IContainer container)
    {
        base.OnStarted(container);
        
        GD.Print("$($Extension.Name) Extension: Started");
    }
    
    protected override void OnDisposing()
    {
        GD.Print("$($Extension.Name) Extension: Disposing");
    }
}

/// <summary>
/// $($Extension.Name)扩展点接口
/// </summary>
public interface I$($Extension.Name)ExtensionPoint
{
    void OnSystemInitialized();
    void OnSystemStarted();
    void OnSystemStopped();
}

/// <summary>
/// $($Extension.Name)扩展点实现
/// </summary>
public class $($Extension.Name)ExtensionPoint : I$($Extension.Name)ExtensionPoint
{
    public void OnSystemInitialized()
    {
        GD.Print("$($Extension.Name) system initialized");
    }
    
    public void OnSystemStarted()
    {
        GD.Print("$($Extension.Name) system started");
    }
    
    public void OnSystemStopped()
    {
        GD.Print("$($Extension.Name) system stopped");
    }
}
"@
    
    $extensionFile = Join-Path $ExtensionPath "$($Extension.Name)Extension.cs"
    Set-Content -Path $extensionFile -Value $extensionContent -Encoding UTF8
}

# 创建README文件
function Create-ReadmeFile {
    param([string]$ExtensionPath, [hashtable]$Extension)
    
    $readmeContent = @"
# $($Extension.Name) 扩展

$($Extension.Description)

## 功能特性

- 🔧 **核心功能**：$($Extension.Description)
- 🔌 **扩展点**：支持其他扩展监听系统事件
- 📦 **自动注册**：自动注册服务和命令处理器
- 🎯 **优先级**：$($Extension.Priority)

## 使用方法

### 1. 注册扩展

```csharp
public partial class Main : Node
{
    public override void _Ready()
    {
        // 注册$($Extension.Name)扩展
        ExtendedContexts.Instance.RegisterExtension<$($Extension.Name)Extension>();
        
        // 获取服务（如果有的话）
        // var service = ExtendedContexts.Instance.GetService<IYourService>();
    }
}
```

### 2. 扩展点监听

```csharp
public class My$($Extension.Name)Extension : FrameworkExtensionBase, I$($Extension.Name)ExtensionPoint
{
    public override void Initialize(IExtensionContext context)
    {
        // 注册为扩展点
        RegisterExtensionPoint<I$($Extension.Name)ExtensionPoint>(this);
    }
    
    public void OnSystemInitialized()
    {
        GD.Print("$($Extension.Name) system initialized");
    }
    
    public void OnSystemStarted()
    {
        GD.Print("$($Extension.Name) system started");
    }
    
    public void OnSystemStopped()
    {
        GD.Print("$($Extension.Name) system stopped");
    }
}
```

## 包含的系统

$($Extension.Systems | ForEach-Object { "- $_" } | Out-String)

## 注意事项

1. 确保在使用前注册扩展
2. 扩展按优先级加载
3. 注意扩展间的依赖关系
4. 正确处理资源清理
"@
    
    $readmeFile = Join-Path $ExtensionPath "README.md"
    Set-Content -Path $readmeFile -Value $readmeContent -Encoding UTF8
}

# 主执行逻辑
Write-Host "开始创建扩展..." -ForegroundColor Green

foreach ($extension in $extensions) {
    Write-Host "创建扩展: $($extension.Name)" -ForegroundColor Yellow
    
    # 创建目录结构
    $extensionPath = Create-ExtensionDirectory -ExtensionName $extension.Name
    
    # 创建项目文件
    Create-ProjectFile -ExtensionPath $extensionPath -ExtensionName $extension.Name
    
    # 创建扩展主类
    Create-ExtensionClass -ExtensionPath $extensionPath -Extension $extension
    
    # 创建README
    Create-ReadmeFile -ExtensionPath $extensionPath -Extension $extension
    
    Write-Host "✓ 扩展 $($extension.Name) 创建完成" -ForegroundColor Green
}

Write-Host "所有扩展创建完成！" -ForegroundColor Green
Write-Host "接下来需要手动：" -ForegroundColor Cyan
Write-Host "1. 将Core中对应的服务和仓储代码复制到各扩展中" -ForegroundColor Cyan
Write-Host "2. 更新命名空间和引用" -ForegroundColor Cyan
Write-Host "3. 从Core中删除原有实现" -ForegroundColor Cyan
Write-Host "4. 测试扩展集成" -ForegroundColor Cyan