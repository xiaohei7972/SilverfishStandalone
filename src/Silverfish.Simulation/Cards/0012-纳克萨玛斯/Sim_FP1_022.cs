using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 空灵召唤者
// Voidcaller
// 亡语：随机将一张恶魔牌从你的手牌置入战场。
// Deathrattle: Put a random Demon from your hand into the battlefield.
[CardSim("FP1_022")]
internal static class Sim_FP1_022
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonFromHandEffect(new()
            {
                Kind = EntityKind.Minion,
                MinionType = MinionType.Demon
            })
        ]
    };
}
