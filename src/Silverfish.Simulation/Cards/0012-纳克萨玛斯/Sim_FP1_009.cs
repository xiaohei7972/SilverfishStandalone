using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 死亡领主
// Deathlord
// 嘲讽，亡语：你的对手将一个随从从其牌库置入战场。
// Taunt. Deathrattle: Your opponent puts a minion from their deck into the battlefield.
[CardSim("FP1_009")]
internal static class Sim_FP1_009
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new RecruitEffect(1, CardRecipient.Opponent)
        ]
    };
}
