using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 乌鸦神像
// Raven Idol
// 抉择： 发现一张随从牌；或者发现一张法术牌。
// Choose One - Discover a minion; or Discover a spell.
[CardSim("LOE_115")]
internal static class Sim_LOE_115
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("LOE_115a", []) { ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { Kind = EntityKind.Minion }) },
            new("LOE_115b", []) { ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { Kind = EntityKind.Spell }) },
        ],
        CombinedChoice = new("both", []) { ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { Kind = EntityKind.Minion }) { Then = new DiscoverCardChoice(new CardFilter { Kind = EntityKind.Spell }) } },
    };
}
