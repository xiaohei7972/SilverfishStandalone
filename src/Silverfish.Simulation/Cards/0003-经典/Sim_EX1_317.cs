using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 感知恶魔
// Sense Demons
// 从你的牌库中抽两张恶魔牌。
// Draw 2 Demons from your deck.
[CardSim("EX1_317")]
internal static class Sim_EX1_317
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawMatchingEffect(new() { MinionType = MinionType.Demon }, 2)]
    };
}
