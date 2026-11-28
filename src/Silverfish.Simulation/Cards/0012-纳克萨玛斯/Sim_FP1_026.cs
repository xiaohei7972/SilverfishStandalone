namespace Silverfish.Simulation.Cards;

// 阿努巴尔伏击者
// Anub'ar Ambusher
// 亡语：随机将一个友方随从移回你的手牌。
// Deathrattle: Return a random friendly minion to your hand.
[CardSim("FP1_026")]
internal static class Sim_FP1_026
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new ReturnMinionEffect(RandomFriendly: true)
        ]
    };
}
