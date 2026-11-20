namespace Silverfish.Simulation;

/// <summary>将已实现的单卡规则绑定到指定卡牌 ID。</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class CardSimAttribute(string cardId) : Attribute
{
    internal string CardId { get; } = cardId;
}
