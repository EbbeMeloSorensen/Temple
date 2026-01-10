using Craft.Math;

namespace Temple.Application.Interfaces;

/// <summary>
/// En NPC request fås under dialog med en NPC, som vises i et site med et udråbstegn over sig
/// </summary>
public class NPCRequest : Quest
{
    public string ModelId { get; set; }
    public string NPCName { get; set; }
    public Point2D Position { get; set; }
    public double Orientation { get; set; }
    public double Height { get; set; }

}