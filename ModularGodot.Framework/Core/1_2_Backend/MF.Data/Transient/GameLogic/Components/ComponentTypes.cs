namespace MF.Data.Transient.GameLogic.Components;

/// <summary>
/// 组件类型常量
/// </summary>
public static class ComponentTypes
{
    // 核心组件类型 (1-99)
    public const int Position = 1;
    public const int Movement = 2;
    public const int Health = 3;
    public const int Combat = 4;
    public const int AI = 5;
    public const int Render = 6;
    public const int Animation = 7;
    public const int Audio = 8;
    public const int Input = 9;
    public const int UI = 10;
    public const int Physics = 11;
    public const int Collision = 12;
    public const int Transform = 13;
    public const int Inventory = 14;
    public const int Stats = 15;
    public const int Buff = 16;
    public const int Skill = 17;
    public const int Quest = 18;
    public const int Dialog = 19;
    public const int Trigger = 20;
    
    // 游戏逻辑组件类型 (100-199)
    public const int Player = 100;
    public const int Enemy = 101;
    public const int NPC = 102;
    public const int Item = 103;
    public const int Weapon = 104;
    public const int Armor = 105;
    public const int Consumable = 106;
    public const int Projectile = 107;
    public const int Pickup = 108;
    public const int Door = 109;
    public const int Chest = 110;
    public const int Switch = 111;
    public const int Platform = 112;
    public const int Trap = 113;
    public const int Spawner = 114;
    public const int Checkpoint = 115;
    public const int Portal = 116;
    public const int Vehicle = 117;
    public const int Building = 118;
    public const int Resource = 119;
    
    // 系统组件类型 (200-299)
    public const int Network = 200;
    public const int Save = 201;
    public const int Debug = 202;
    public const int Performance = 203;
    public const int Analytics = 204;
    public const int Localization = 205;
    public const int Achievement = 206;
    public const int Social = 207;
    public const int Economy = 208;
    public const int Weather = 209;
    public const int Time = 210;
    public const int Scene = 211;
    public const int Camera = 212;
    public const int Light = 213;
    public const int Particle = 214;
    public const int PostProcess = 215;
    public const int Shader = 216;
    public const int Material = 217;
    public const int Texture = 218;
    public const int Mesh = 219;
    
    // 扩展组件类型从1000开始
    public const int CustomStart = 1000;
    
    /// <summary>
    /// 组件类型信息
    /// </summary>
    public static readonly Dictionary<int, ComponentTypeInfo> TypeInfos = new()
    {
        // 核心组件
        { Position, new ComponentTypeInfo("Position", "位置组件", ComponentCategory.Core, typeof(PositionComponent)) },
        { Movement, new ComponentTypeInfo("Movement", "移动组件", ComponentCategory.Core, typeof(MovementComponent)) },
        { Health, new ComponentTypeInfo("Health", "生命值组件", ComponentCategory.Core, typeof(HealthComponent)) },
        { Combat, new ComponentTypeInfo("Combat", "战斗组件", ComponentCategory.Core, typeof(CombatComponent)) },
        { AI, new ComponentTypeInfo("AI", "AI组件", ComponentCategory.Core, typeof(AIComponent)) },
        { Render, new ComponentTypeInfo("Render", "渲染组件", ComponentCategory.Core) },
        { Animation, new ComponentTypeInfo("Animation", "动画组件", ComponentCategory.Core) },
        { Audio, new ComponentTypeInfo("Audio", "音频组件", ComponentCategory.Core) },
        { Input, new ComponentTypeInfo("Input", "输入组件", ComponentCategory.Core) },
        { UI, new ComponentTypeInfo("UI", "UI组件", ComponentCategory.Core) },
        { Physics, new ComponentTypeInfo("Physics", "物理组件", ComponentCategory.Core) },
        { Collision, new ComponentTypeInfo("Collision", "碰撞组件", ComponentCategory.Core) },
        { Transform, new ComponentTypeInfo("Transform", "变换组件", ComponentCategory.Core) },
        { Inventory, new ComponentTypeInfo("Inventory", "库存组件", ComponentCategory.Core) },
        { Stats, new ComponentTypeInfo("Stats", "属性组件", ComponentCategory.Core) },
        
        // 游戏逻辑组件
        { Player, new ComponentTypeInfo("Player", "玩家组件", ComponentCategory.Gameplay) },
        { Enemy, new ComponentTypeInfo("Enemy", "敌人组件", ComponentCategory.Gameplay) },
        { NPC, new ComponentTypeInfo("NPC", "NPC组件", ComponentCategory.Gameplay) },
        { Item, new ComponentTypeInfo("Item", "物品组件", ComponentCategory.Gameplay) },
        { Weapon, new ComponentTypeInfo("Weapon", "武器组件", ComponentCategory.Gameplay) },
        { Armor, new ComponentTypeInfo("Armor", "护甲组件", ComponentCategory.Gameplay) },
        
        // 系统组件
        { Network, new ComponentTypeInfo("Network", "网络组件", ComponentCategory.System) },
        { Save, new ComponentTypeInfo("Save", "存档组件", ComponentCategory.System) },
        { Debug, new ComponentTypeInfo("Debug", "调试组件", ComponentCategory.System) },
        { Performance, new ComponentTypeInfo("Performance", "性能组件", ComponentCategory.System) },
        { Analytics, new ComponentTypeInfo("Analytics", "分析组件", ComponentCategory.System) }
    };
    
    /// <summary>
    /// 根据类型ID获取组件信息
    /// </summary>
    /// <param name="typeId">类型ID</param>
    /// <returns>组件信息</returns>
    public static ComponentTypeInfo? GetTypeInfo(int typeId)
    {
        return TypeInfos.TryGetValue(typeId, out var info) ? info : null;
    }
    
    /// <summary>
    /// 根据组件类型获取类型ID
    /// </summary>
    /// <param name="componentType">组件类型</param>
    /// <returns>类型ID</returns>
    public static int GetTypeId(Type componentType)
    {
        var info = TypeInfos.Values.FirstOrDefault(i => i.ComponentType == componentType);
        return info?.TypeId ?? -1;
    }
    
    /// <summary>
    /// 根据名称获取类型ID
    /// </summary>
    /// <param name="name">组件名称</param>
    /// <returns>类型ID</returns>
    public static int GetTypeId(string name)
    {
        var info = TypeInfos.Values.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return info?.TypeId ?? -1;
    }
    
    /// <summary>
    /// 获取指定分类的所有组件类型
    /// </summary>
    /// <param name="category">组件分类</param>
    /// <returns>组件类型ID列表</returns>
    public static IEnumerable<int> GetTypesByCategory(ComponentCategory category)
    {
        return TypeInfos.Where(kvp => kvp.Value.Category == category).Select(kvp => kvp.Key);
    }
    
    /// <summary>
    /// 注册自定义组件类型
    /// </summary>
    /// <param name="typeId">类型ID</param>
    /// <param name="info">组件信息</param>
    public static void RegisterCustomType(int typeId, ComponentTypeInfo info)
    {
        if (typeId < CustomStart)
            throw new ArgumentException($"Custom component type ID must be >= {CustomStart}", nameof(typeId));
        
        if (TypeInfos.ContainsKey(typeId))
            throw new ArgumentException($"Component type ID {typeId} is already registered", nameof(typeId));
        
        info.TypeId = typeId;
        TypeInfos[typeId] = info;
    }
}

/// <summary>
/// 组件类型信息
/// </summary>
public class ComponentTypeInfo
{
    /// <summary>
    /// 类型ID
    /// </summary>
    public int TypeId { get; set; }
    
    /// <summary>
    /// 组件名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 组件描述
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// 组件分类
    /// </summary>
    public ComponentCategory Category { get; set; }
    
    /// <summary>
    /// 组件类型
    /// </summary>
    public Type? ComponentType { get; set; }
    
    /// <summary>
    /// 是否可序列化
    /// </summary>
    public bool IsSerializable { get; set; } = true;
    
    /// <summary>
    /// 是否可持久化
    /// </summary>
    public bool IsPersistent { get; set; } = false;
    
    /// <summary>
    /// 组件大小（字节）
    /// </summary>
    public int Size { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ComponentTypeInfo(string name, string description, ComponentCategory category, Type? componentType = null)
    {
        Name = name;
        Description = description;
        Category = category;
        ComponentType = componentType;
        
        if (componentType != null)
        {
            Size = System.Runtime.InteropServices.Marshal.SizeOf(componentType);
        }
    }
}

/// <summary>
/// 组件分类
/// </summary>
public enum ComponentCategory
{
    /// <summary>
    /// 核心组件
    /// </summary>
    Core,
    
    /// <summary>
    /// 游戏逻辑组件
    /// </summary>
    Gameplay,
    
    /// <summary>
    /// 系统组件
    /// </summary>
    System,
    
    /// <summary>
    /// 渲染组件
    /// </summary>
    Rendering,
    
    /// <summary>
    /// 物理组件
    /// </summary>
    Physics,
    
    /// <summary>
    /// 音频组件
    /// </summary>
    Audio,
    
    /// <summary>
    /// 网络组件
    /// </summary>
    Network,
    
    /// <summary>
    /// 自定义组件
    /// </summary>
    Custom
}