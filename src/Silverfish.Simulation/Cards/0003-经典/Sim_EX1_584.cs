using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 年迈的法师
// Ancient Mage
// 战吼：使相邻的随从获得法术伤害+1。
// Battlecry: Give adjacent_minions Spell_Damage +1.
[CardSim("EX1_584")]
internal static class Sim_EX1_584
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_584e.Rule.ApplyTo(StatTarget.AdjacentFriendlyMinions)]
    };
}
