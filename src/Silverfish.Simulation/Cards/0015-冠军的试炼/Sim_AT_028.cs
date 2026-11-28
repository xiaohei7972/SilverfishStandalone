using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 影踪骁骑兵
// Shado-Pan Rider
// 连击： 获得+4攻击力。
// Combo: Gain +4 Attack.
[CardSim("AT_028")]
internal static class Sim_AT_028
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ComboCondition(), [Sim_AT_028e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
