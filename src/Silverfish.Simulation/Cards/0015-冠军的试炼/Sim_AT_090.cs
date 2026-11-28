using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 穆克拉的勇士
// Mukla's Champion
// 激励：使你的其他随从获得+1/+1。
// Inspire: Give your other minions +1/+1.
[CardSim("AT_090")]
internal static class Sim_AT_090
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [Sim_AT_090e.Rule.ApplyTo(StatTarget.OtherFriendlyMinions)]),
        ],
    };
}
