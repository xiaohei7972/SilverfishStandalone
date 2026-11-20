using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 寒冰护体
// Ice Barrier
// 奥秘：当你的英雄受到攻击时，获得8点护甲值。
// Secret: When your hero is attacked, gain 8 Armor.
[CardSim("EX1_289")]
internal static class Sim_EX1_289
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new GainArmorEffect(8)])
            {
                Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Hero
            }
        ]
    };
}
