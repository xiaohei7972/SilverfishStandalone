using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 野性之怒
// Feral Rage
// 抉择：使你的英雄在本回合中获得+4攻击力；或者获得8点护甲值。
// Choose One - Give your hero +4 Attack this turn; or Gain 8 Armor.
[CardSim("OG_047")]
internal static class Sim_OG_047
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("OG_047a", [Sim_OG_047e.Rule.ApplyTo(StatTarget.FriendlyHero)]),
            new("OG_047b", [new GainArmorEffect(8)]),
        ],
    };
}
