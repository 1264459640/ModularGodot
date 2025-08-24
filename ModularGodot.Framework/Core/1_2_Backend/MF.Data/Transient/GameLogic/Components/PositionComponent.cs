using System.Numerics;
using System.Text.Json.Serialization;
using MF.Data.Core.Attributes;

namespace MF.Data.Transient.GameLogic.Components;

/// <summary>
/// 位置组件
/// </summary>
[Serializable]
[Component("Position", TypeId = ComponentTypes.Position, Description = "实体的位置、旋转和缩放信息")]
public struct PositionComponent : IComponent
{
    public int ComponentTypeId => ComponentTypes.Position;
    public bool IsValid => true;
    
    /// <summary>
    /// 位置坐标
    /// </summary>
    [JsonPropertyName("position")]
    public Vector2 Position;
    
    /// <summary>
    /// 旋转角度（弧度）
    /// </summary>
    [JsonPropertyName("rotation")]
    public float Rotation;
    
    /// <summary>
    /// 缩放比例
    /// </summary>
    [JsonPropertyName("scale")]
    public Vector2 Scale;
    
    /// <summary>
    /// Z轴坐标（用于层级排序）
    /// </summary>
    [JsonPropertyName("z_index")]
    public float ZIndex;
    
    /// <summary>
    /// 是否为全局坐标
    /// </summary>
    [JsonPropertyName("is_global")]
    public bool IsGlobal;
    
    /// <summary>
    /// 父实体ID（用于层级变换）
    /// </summary>
    [JsonPropertyName("parent_entity")]
    public uint ParentEntity;
    
    public PositionComponent(Vector2 position, float rotation = 0f, Vector2? scale = null, float zIndex = 0f)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale ?? Vector2.One;
        ZIndex = zIndex;
        IsGlobal = false;
        ParentEntity = 0;
    }
    
    public PositionComponent(float x, float y, float rotation = 0f, float scaleX = 1f, float scaleY = 1f, float zIndex = 0f)
        : this(new Vector2(x, y), rotation, new Vector2(scaleX, scaleY), zIndex)
    {
    }
    
    public void Reset()
    {
        Position = Vector2.Zero;
        Rotation = 0f;
        Scale = Vector2.One;
        ZIndex = 0f;
        IsGlobal = false;
        ParentEntity = 0;
    }
    
    public IComponent Clone()
    {
        return new PositionComponent
        {
            Position = Position,
            Rotation = Rotation,
            Scale = Scale,
            ZIndex = ZIndex,
            IsGlobal = IsGlobal,
            ParentEntity = ParentEntity
        };
    }
    
    public string Serialize()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
    
    public void Deserialize(string data)
    {
        if (string.IsNullOrEmpty(data)) return;
        
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<PositionComponent>(data);
        Position = deserialized.Position;
        Rotation = deserialized.Rotation;
        Scale = deserialized.Scale;
        ZIndex = deserialized.ZIndex;
        IsGlobal = deserialized.IsGlobal;
        ParentEntity = deserialized.ParentEntity;
    }
    
    /// <summary>
    /// 获取变换矩阵
    /// </summary>
    /// <returns>变换矩阵</returns>
    public Matrix3x2 GetTransformMatrix()
    {
        var translation = Matrix3x2.CreateTranslation(Position);
        var rotation = Matrix3x2.CreateRotation(Rotation);
        var scale = Matrix3x2.CreateScale(Scale);
        
        return scale * rotation * translation;
    }
    
    /// <summary>
    /// 获取逆变换矩阵
    /// </summary>
    /// <returns>逆变换矩阵</returns>
    public Matrix3x2 GetInverseTransformMatrix()
    {
        var matrix = GetTransformMatrix();
        Matrix3x2.Invert(matrix, out var inverse);
        return inverse;
    }
    
    /// <summary>
    /// 将本地坐标转换为世界坐标
    /// </summary>
    /// <param name="localPoint">本地坐标点</param>
    /// <returns>世界坐标点</returns>
    public Vector2 LocalToWorld(Vector2 localPoint)
    {
        return Vector2.Transform(localPoint, GetTransformMatrix());
    }
    
    /// <summary>
    /// 将世界坐标转换为本地坐标
    /// </summary>
    /// <param name="worldPoint">世界坐标点</param>
    /// <returns>本地坐标点</returns>
    public Vector2 WorldToLocal(Vector2 worldPoint)
    {
        return Vector2.Transform(worldPoint, GetInverseTransformMatrix());
    }
    
    /// <summary>
    /// 移动到指定位置
    /// </summary>
    /// <param name="newPosition">新位置</param>
    public void MoveTo(Vector2 newPosition)
    {
        Position = newPosition;
    }
    
    /// <summary>
    /// 相对移动
    /// </summary>
    /// <param name="offset">偏移量</param>
    public void MoveBy(Vector2 offset)
    {
        Position += offset;
    }
    
    /// <summary>
    /// 旋转到指定角度
    /// </summary>
    /// <param name="newRotation">新角度（弧度）</param>
    public void RotateTo(float newRotation)
    {
        Rotation = newRotation;
    }
    
    /// <summary>
    /// 相对旋转
    /// </summary>
    /// <param name="deltaRotation">旋转增量（弧度）</param>
    public void RotateBy(float deltaRotation)
    {
        Rotation += deltaRotation;
        
        // 保持角度在 [0, 2π) 范围内
        while (Rotation >= MathF.PI * 2) Rotation -= MathF.PI * 2;
        while (Rotation < 0) Rotation += MathF.PI * 2;
    }
    
    /// <summary>
    /// 缩放到指定比例
    /// </summary>
    /// <param name="newScale">新缩放比例</param>
    public void ScaleTo(Vector2 newScale)
    {
        Scale = newScale;
    }
    
    /// <summary>
    /// 相对缩放
    /// </summary>
    /// <param name="scaleMultiplier">缩放倍数</param>
    public void ScaleBy(Vector2 scaleMultiplier)
    {
        Scale *= scaleMultiplier;
    }
    
    /// <summary>
    /// 获取前方向量
    /// </summary>
    /// <returns>前方向量</returns>
    public Vector2 GetForward()
    {
        return new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation));
    }
    
    /// <summary>
    /// 获取右方向量
    /// </summary>
    /// <returns>右方向量</returns>
    public Vector2 GetRight()
    {
        return new Vector2(-MathF.Sin(Rotation), MathF.Cos(Rotation));
    }
    
    /// <summary>
    /// 朝向指定位置
    /// </summary>
    /// <param name="target">目标位置</param>
    public void LookAt(Vector2 target)
    {
        var direction = target - Position;
        if (direction.LengthSquared() > 0)
        {
            Rotation = MathF.Atan2(direction.Y, direction.X);
        }
    }
    
    /// <summary>
    /// 计算到另一个位置的距离
    /// </summary>
    /// <param name="other">另一个位置组件</param>
    /// <returns>距离</returns>
    public float DistanceTo(PositionComponent other)
    {
        return Vector2.Distance(Position, other.Position);
    }
    
    /// <summary>
    /// 计算到指定点的距离
    /// </summary>
    /// <param name="point">目标点</param>
    /// <returns>距离</returns>
    public float DistanceTo(Vector2 point)
    {
        return Vector2.Distance(Position, point);
    }
    
    public override string ToString()
    {
        return $"Position: {Position}, Rotation: {Rotation:F2}, Scale: {Scale}, ZIndex: {ZIndex}";
    }
    
    public override bool Equals(object? obj)
    {
        return obj is PositionComponent other && Equals(other);
    }
    
    public bool Equals(PositionComponent other)
    {
        return Position.Equals(other.Position) &&
               Rotation.Equals(other.Rotation) &&
               Scale.Equals(other.Scale) &&
               ZIndex.Equals(other.ZIndex) &&
               IsGlobal == other.IsGlobal &&
               ParentEntity == other.ParentEntity;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Position, Rotation, Scale, ZIndex, IsGlobal, ParentEntity);
    }
    
    public static bool operator ==(PositionComponent left, PositionComponent right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(PositionComponent left, PositionComponent right)
    {
        return !left.Equals(right);
    }
}