using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 虚灵巫师
// Ethereal Conjurer
// 战吼： 发现一张法术牌。
// Battlecry: Discover a spell.
[CardSim("LOE_003")]
internal static class Sim_LOE_003
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { Kind = EntityKind.Spell }),
    };
}
