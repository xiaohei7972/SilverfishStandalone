using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 寒光先知
// Coldlight Seer
// 战吼：使你的其他鱼人获得+2生命值。
// Battlecry: Give your other Murlocs +2 Health.
[CardSim("EX1_103")]
internal static class Sim_EX1_103
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_103e.Rule.ApplyTo(StatTarget.OtherFriendlyMinions, new CardFilter { MinionType = MinionType.Murloc })]
    };
}
