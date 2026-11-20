using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 民兵指挥官
// Militia Commander
// 突袭，战吼：在本回合获得+3攻击力。
// Rush Battlecry: Gain +3_Attack this turn.
[CardSim("GIL_803")]
internal static class Sim_GIL_803
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_GIL_803e.Rule.ApplyTo(StatTarget.Source)]
    };
}
