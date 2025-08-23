using Godot;
using MF.Nodes.Abstractions.Bases;

namespace MF.Nodes.Abstractions.Extensions.AudioSystem;

public interface IAudioManager : INode
{
    Node? AudioNodeRoot { get; set; }
}