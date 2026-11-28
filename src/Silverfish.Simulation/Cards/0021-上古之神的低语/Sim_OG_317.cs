using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 黑龙领主死亡之翼
// Deathwing, Dragonlord
// 亡语：将你手牌中所有的龙牌置入战场。
// Deathrattle: Put all Dragons from your hand into the battlefield.
[CardSim("OG_317")]
internal static class Sim_OG_317
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonFromHandEffect(new CardFilter { MinionType = MinionType.Dragon }, Count: 7),
        ],
    };
}
