using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 银色神官帕尔崔丝
// Confessor Paletress
// 战吼，激励：随机召唤一个传说随从。
// Battlecry and Inspire: Summon a random Legendary minion.
[CardSim("AT_018")]
internal static class Sim_AT_018
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonMatchingMinionEffect(new CardFilter { Kind = EntityKind.Minion, IsLegendary = true }),
        ],
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new SummonMatchingMinionEffect(new CardFilter { Kind = EntityKind.Minion, IsLegendary = true })]),
        ],
    };
}
