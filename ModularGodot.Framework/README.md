# ModularGodot Framework - 开发者框架

## 🎯 概述

ModularGodot Framework 是一个完整的 Godot C# 游戏开发框架，为开发者提供了模块化、可扩展的开发环境。开发者获取此框架后，可以直接在此基础上进行游戏开发，所有后续开发都在这个统一的框架结构下进行。

## 📁 框架结构

```
ModularGodot.Framework/
├── 📁 Core/                           # 🔒 核心框架（只读，不允许修改）
│   ├── ModularGodot.Core.Commons/     # 通用工具库
│   ├── ModularGodot.Core.Abstractions/# 抽象接口定义
│   ├── ModularGodot.Core.Extensions/  # 扩展包管理系统
│   ├── ModularGodot.Core.Events/      # 事件系统
│   ├── ModularGodot.Core.Data/        # 数据模型
│   └── ModularGodot.Core.Contexts/    # 依赖注入上下文
├── 📁 Extensions/                      # 🔧 官方扩展包（可选安装）
│   ├── ModularGodot.Extensions.UI/    # UI管理扩展
│   ├── ModularGodot.Extensions.Audio/ # 音频管理扩展
│   └── ModularGodot.Extensions.Scene/ # 场景管理扩展
├── 📁 Templates/                       # 📋 项目模板
│   ├── BasicGame/                      # ✅ 基础游戏模板
│   ├── UIGame/                         # UI重点游戏模板
│   └── ActionGame/                     # 动作游戏模板
├── 📁 Workspace/                       # 💻 开发者工作区
│   └── [您的游戏项目]                   # 在这里开发您的游戏
├── 📁 Tools/                           # 🛠️ 开发工具
├── 📁 Docs/                            # 📚 文档
└── 📄 ModularGodot.Framework.sln       # 框架解决方案
```

## 🚀 快速开始

### 1. 获取框架

```bash
# 克隆框架到本地
git clone https://github.com/ModularGodot/Framework.git MyGameProject
cd MyGameProject
```

### 2. 创建您的游戏项目

```bash
# 方式1：复制基础模板
cp -r Templates/BasicGame Workspace/MyAwesomeGame
cd Workspace/MyAwesomeGame

# 方式2：使用项目生成器（推荐）
# dotnet run --project Tools/ProjectGenerator -- --template BasicGame --name MyAwesomeGame
```

### 3. 配置项目

```bash
# 重命名项目文件
mv BasicGame.csproj MyAwesomeGame.csproj

# 编辑项目文件，更新RootNamespace
# <RootNamespace>MyAwesomeGame</RootNamespace>
```

### 4. 构建和运行

```bash
# 构建项目
dotnet build

# 在Godot中打开项目
godot project.godot
```

## 🔧 开发规范

### 📋 核心原则

1. **不要修改 Core/ 目录**：核心框架代码是只读的，通过NuGet包管理
2. **所有业务代码在 Workspace/ 下**：您的游戏代码应该放在工作区中
3. **使用扩展包系统**：通过实现 `IModularGodotExtension` 接口来扩展功能
4. **遵循依赖注入**：使用Autofac进行组件管理
5. **统一命名约定**：遵循C#和Godot的命名规范

### 🏗️ 项目结构规范

在 `Workspace/YourGame/` 下组织代码：

```
YourGame/
├── Scripts/
│   ├── Core/                    # 核心游戏逻辑
│   ├── UI/                      # UI脚本
│   ├── Systems/                 # 游戏系统
│   └── Extensions/              # 自定义扩展包
├── Scenes/                      # Godot场景文件
├── Assets/                      # 游戏资源
├── Configs/                     # 配置文件
└── Tests/                       # 单元测试
```

### 🔌 扩展包开发

创建自定义扩展包：

```csharp
using ModularGodot.Core.Abstractions;
using Autofac;

public class MyExtension : ModularGodotExtensionBase
{
    public override string Name => "My Extension";
    public override Version Version => new Version(1, 0, 0);
    
    public override void ConfigureServices(ContainerBuilder builder)
    {
        // 注册服务
        builder.RegisterType<MyService>().As<IMyService>();
    }
    
    public override void Initialize(IContainer container)
    {
        // 初始化逻辑
    }
}
```

在 `GameBootstrap.cs` 中注册：

```csharp
private static void RegisterExtensions()
{
    _extensionManager!.RegisterExtension<MyExtension>();
}
```

## 📚 开发工作流

### 日常开发

1. **编写游戏逻辑**：在 `Scripts/Core/` 下编写核心游戏代码
2. **创建UI**：在 `Scripts/UI/` 下编写UI相关代码
3. **添加系统**：在 `Scripts/Systems/` 下实现游戏系统
4. **测试**：在 `Tests/` 下编写单元测试
5. **构建运行**：使用 `dotnet build` 构建，用Godot运行

### 扩展包开发

1. **创建扩展包**：在 `Scripts/Extensions/` 下创建扩展包类
2. **实现接口**：继承 `ModularGodotExtensionBase`
3. **注册服务**：在 `ConfigureServices` 中注册依赖
4. **初始化**：在 `Initialize` 中执行初始化逻辑
5. **注册扩展包**：在 `GameBootstrap` 中注册

### 版本管理

1. **核心框架更新**：通过NuGet包或Git拉取更新
2. **业务代码版本控制**：使用Git管理 `Workspace/` 下的代码
3. **配置管理**：将配置文件纳入版本控制

## 🛠️ 可用工具

### 项目生成器
```bash
# 创建新项目
dotnet run --project Tools/ProjectGenerator -- --template BasicGame --name MyGame

# 启用扩展包
dotnet run --project Tools/ProjectGenerator -- --enable-extension UI Audio
```

### 扩展包生成器
```bash
# 生成扩展包模板
dotnet run --project Tools/ExtensionGenerator -- --name MyExtension
```

### .csproj 文件生成器
```powershell
# 为新扩展生成 .csproj 文件并自动添加到解决方案
./Tools/CreateCsproj.ps1 -Name MyNewExtension
```

## 📖 文档资源

- **[快速开始指南](Docs/QuickStart.md)** - 5分钟上手
- **[开发者指南](Docs/DeveloperGuide.md)** - 详细开发指导
- **[扩展包开发指南](Docs/ExtensionGuide.md)** - 扩展包开发
- **[API参考](Docs/APIReference/)** - 完整API文档
- **[最佳实践](Docs/BestPractices.md)** - 开发最佳实践
- **[常见问题](Docs/FAQ.md)** - 问题解答

## 🎮 示例项目

查看 `Templates/` 目录中的示例项目：

- **BasicGame** - 最基础的游戏模板
- **UIGame** - UI重点的游戏模板  
- **ActionGame** - 动作游戏模板

## 🤝 社区和支持

- **GitHub Issues** - 报告问题和建议
- **Discord社区** - 实时交流和讨论
- **官方文档** - 完整的使用指南
- **示例代码** - 丰富的代码示例

## 📄 许可证

本框架采用 MIT 许可证，您可以自由使用、修改和分发。

## 🔄 版本信息

- **当前版本**: 1.0.0
- **Godot版本**: 4.4.1+
- **.NET版本**: 9.0+
- **最后更新**: 2025年1月

---

**开始您的ModularGodot游戏开发之旅吧！** 🎮✨

如果您有任何问题或建议，请随时联系我们或提交Issue。我们致力于为Godot C#开发者提供最好的开发体验！