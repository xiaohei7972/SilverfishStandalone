using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 虚空幽龙史学家
// Netherspite Historian
// 战吼：如果你的手牌中有龙牌，便发现一张龙牌。
// Battlecry: If you're holding a Dragon, Discover a Dragon.
[CardSim("KAR_062")]
internal static class Sim_KAR_062
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { Kind = EntityKind.Minion, MinionType = MinionType.Dragon })
        {
            Condition = new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Hand, NumberComparison.AtLeast, 1) { CardFilter = new CardFilter { MinionType = MinionType.Dragon } },
        },
    };
}
