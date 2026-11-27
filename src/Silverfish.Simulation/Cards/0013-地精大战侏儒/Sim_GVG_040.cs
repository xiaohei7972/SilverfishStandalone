using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 沙鳞灵魂行者
// Siltfin Spiritwalker
// 每当有其他友方鱼人死亡，便抽一张牌。 过载：（1）
// Whenever another friendly Murloc dies, draw a card. Overload: (1)
[CardSim("GVG_040")]
internal static class Sim_GVG_040
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.MinionDied, TriggerRelation.Friendly, [new DrawEffect(1)])
            {
                EntityMatch = TriggerEntityMatch.Other,
                CardFilter = new CardFilter { MinionType = MinionType.Murloc }
            }
        ]
    };
}
