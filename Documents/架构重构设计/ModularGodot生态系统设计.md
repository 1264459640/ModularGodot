# ModularGodot 生态系统架构设计

## 🎯 项目目标

将当前的 ModularGodot 项目重构为一个完整的生态系统：
- **核心框架**：提供基础架构和依赖注入能力
- **扩展包系统**：各功能模块独立打包，可按需集成
- **示例项目**：展示框架使用和最佳实践
- **完整文档**：详细的使用指南和API文档

## 📊 当前架构分析结果

### 核心基础设施组件（必须保留在核心框架）

1. **TO.Commons** - 通用工具库
   - 扩展方法、配置管理、枚举定义
   - 依赖：Newtonsoft.Json 13.0.3
   - 作用：所有模块的基础依赖

2. **TO.Contexts** - 依赖注入上下文
   - Autofac 8.3.0 容器管理
   - MediatR 12.5.0 中介者模式
   - 节点生命周期管理
   - 作用：整个框架的核心协调器

3. **TO.Nodes.Abstractions** - 节点抽象层
   - 节点接口定义
   - 依赖注入集成抽象
   - 作用：定义框架的基础契约

4. **TO.Events** - 事件系统
   - 全局事件定义
   - 事件总线基础设施
   - 作用：模块间通信的基础

5. **TO.Data** - 数据模型层
   - 基础数据模型
   - 序列化支持
   - Entity Framework Core 9.0.7
   - 作用：数据访问的基础

### 可拆分的扩展包组件

1. **UI管理扩展包**
   - TO.Services (UI相关部分)
   - TO.Repositories (UI相关部分)
   - UI动画系统
   - UI生命周期管理

2. **音频管理扩展包**
   - 音频播放和管理服务
   - 音量控制系统
   - 音频资源管理

3. **场景管理扩展包**
   - 场景切换服务
   - 过渡效果系统
   - 场景生命周期管理

4. **命令系统扩展包**
   - TO.Commands
   - TO.CommandHandlers
   - CQRS模式实现

5. **游戏能力系统扩展包**
   - GameAbilitySystem
   - 属性系统
   - 效果系统

## 🏗️ 新架构设计

### 核心框架结构 (ModularGodot.Core)

```
ModularGodot.Core/
├── ModularGodot.Core.Commons/          # 通用工具库
├── ModularGodot.Core.Contexts/         # 依赖注入上下文
├── ModularGodot.Core.Abstractions/     # 核心抽象接口
├── ModularGodot.Core.Events/           # 事件系统
├── ModularGodot.Core.Data/             # 数据模型基础
└── ModularGodot.Core.Extensions/       # 扩展包集成机制
```

### 扩展包结构

```
ModularGodot.Extensions.UI/
├── ModularGodot.Extensions.UI.Abstractions/
├── ModularGodot.Extensions.UI.Services/
├── ModularGodot.Extensions.UI.Repositories/
└── ModularGodot.Extensions.UI.Components/

ModularGodot.Extensions.Audio/
├── ModularGodot.Extensions.Audio.Abstractions/
├── ModularGodot.Extensions.Audio.Services/
└── ModularGodot.Extensions.Audio.Components/

ModularGodot.Extensions.Scene/
├── ModularGodot.Extensions.Scene.Abstractions/
├── ModularGodot.Extensions.Scene.Services/
└── ModularGodot.Extensions.Scene.Components/

ModularGodot.Extensions.Commands/
├── ModularGodot.Extensions.Commands.Abstractions/
├── ModularGodot.Extensions.Commands.Handlers/
└── ModularGodot.Extensions.Commands.Core/
```

### 示例项目结构

```
ModularGodot.Examples/
├── BasicExample/                       # 基础使用示例
├── UIExample/                          # UI系统使用示例
├── AudioExample/                       # 音频系统使用示例
├── FullFeaturedExample/                # 完整功能示例
└── CustomExtensionExample/             # 自定义扩展包示例
```

## 🔧 扩展包集成机制设计

### 1. 扩展包接口定义

```csharp
public interface IModularGodotExtension
{
    string Name { get; }
    Version Version { get; }
    IEnumerable<Type> Dependencies { get; }
    void ConfigureServices(ContainerBuilder builder);
    void Initialize(IContainer container);
}
```

### 2. 扩展包注册机制

```csharp
public class ExtensionManager
{
    public void RegisterExtension<T>() where T : IModularGodotExtension, new();
    public void LoadExtensions(ContainerBuilder builder);
    public void InitializeExtensions(IContainer container);
}
```

### 3. 自动发现机制

- 通过反射自动发现程序集中的扩展包
- 支持配置文件指定扩展包
- 支持运行时动态加载扩展包

## 📦 NuGet包发布策略

### 核心包
- **ModularGodot.Core** - 核心框架包
- **ModularGodot.Templates** - 项目模板包

### 扩展包
- **ModularGodot.Extensions.UI** - UI管理扩展
- **ModularGodot.Extensions.Audio** - 音频管理扩展
- **ModularGodot.Extensions.Scene** - 场景管理扩展
- **ModularGodot.Extensions.Commands** - 命令系统扩展
- **ModularGodot.Extensions.GameAbility** - 游戏能力系统扩展

### 工具包
- **ModularGodot.Tools** - 开发工具和代码生成器
- **ModularGodot.Templates** - 项目和文件模板

## 🚀 实施计划

### 阶段1：核心框架提取（高优先级）
1. 创建 ModularGodot.Core 解决方案
2. 提取核心组件到新的项目结构
3. 设计扩展包集成机制
4. 创建基础项目模板

### 阶段2：扩展包拆分（中优先级）
1. 拆分UI管理系统
2. 拆分音频管理系统
3. 拆分场景管理系统
4. 拆分命令系统

### 阶段3：示例和文档（中优先级）
1. 创建基础使用示例
2. 创建各扩展包使用示例
3. 编写完整的API文档
4. 创建快速入门指南

### 阶段4：生态完善（低优先级）
1. 创建开发工具
2. 建立社区贡献机制
3. 性能优化和测试完善
4. 持续集成和自动化发布

## 🎯 成功标准

1. **易用性**：开发者可以通过简单的NuGet包安装快速开始
2. **模块化**：各功能模块可以独立使用，不强制依赖
3. **扩展性**：第三方开发者可以轻松创建自定义扩展包
4. **文档完整**：提供详细的使用指南和API文档
5. **示例丰富**：涵盖各种使用场景的示例项目

## 📋 技术要求

- **.NET 9.0**：统一使用最新的.NET版本
- **Godot 4.4.1**：兼容当前Godot版本
- **NuGet包管理**：支持标准的NuGet包管理
- **语义化版本**：遵循语义化版本规范
- **向后兼容**：保证API的向后兼容性

## 🔄 迁移策略

1. **渐进式迁移**：现有项目可以逐步迁移到新架构
2. **兼容性保证**：提供迁移工具和向导
3. **文档支持**：详细的迁移指南和最佳实践
4. **社区支持**：建立问题反馈和解决机制

这个设计将 ModularGodot 从一个单体项目转变为一个完整的生态系统，为 Godot C# 开发者提供专业级的开发框架和工具链。