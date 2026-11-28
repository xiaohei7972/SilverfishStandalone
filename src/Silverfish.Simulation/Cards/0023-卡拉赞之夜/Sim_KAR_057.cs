using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 象牙骑士
// Ivory Knight
// 战吼：发现一张法术牌。为你的英雄恢复等同于其法力值消耗的生命值。
// Battlecry: Discover a spell. Restore Health to your hero equal to its Cost.
[CardSim("KAR_057")]
internal static class Sim_KAR_057
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { Kind = EntityKind.Spell }) { HealHeroByChosenCost = true },
    };
}
