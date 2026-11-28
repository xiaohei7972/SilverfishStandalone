using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 墓穴蜘蛛
// Tomb Spider
// 战吼： 发现一张野兽牌。
// Battlecry: Discover a Beast.
[CardSim("LOE_047")]
internal static class Sim_LOE_047
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { Kind = EntityKind.Minion, MinionType = MinionType.Beast }),
    };
}
