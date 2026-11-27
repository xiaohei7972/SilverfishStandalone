using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 撕咬
// Bite
// 使你的英雄获得4点护甲值，并在本回合中获得 +4攻击力。
// Give your hero +4_Attack this turn. Gain 4 Armor.
[CardSim("EX1_570")]
internal static class Sim_EX1_570
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_570e.Rule.ApplyTo(StatTarget.FriendlyHero), new GainArmorEffect(4)]
    };
}
