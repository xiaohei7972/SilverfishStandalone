using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 雷霆崖勇士
// Thunder Bluff Valiant
// 战吼，激励：使你的图腾获得+2攻击力。
// Battlecry and Inspire: Give your Totems +2 Attack.
[CardSim("AT_049")]
internal static class Sim_AT_049
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_AT_049e.Rule.ApplyTo(StatTarget.FriendlyMinions, new CardFilter { MinionType = MinionType.Totem }),
        ],
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [Sim_AT_049e.Rule.ApplyTo(StatTarget.FriendlyMinions, new CardFilter { MinionType = MinionType.Totem })]),
        ],
    };
}
