using MF.Nodes.Abstractions.Bases;

namespace MF.Repositories.Bases;

public class NodeRegister(
)
{
    public bool Register<T>(T node) where T : INode
    {
        return node switch
        {
            _ => throw new ArgumentException($"暂不支持的单例节点：{typeof(T).Name}")
        };
    }


}
