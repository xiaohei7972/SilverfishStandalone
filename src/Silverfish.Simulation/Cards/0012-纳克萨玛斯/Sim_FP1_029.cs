namespace Silverfish.Simulation.Cards;

// 舞动之剑
// Dancing Swords
// 亡语：你的对手抽一张牌。
// Deathrattle: Your opponent draws a card.
[CardSim("FP1_029")]
internal static class Sim_FP1_029
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new DrawEffect(1, CardRecipient.Opponent)
        ]
    };
}
