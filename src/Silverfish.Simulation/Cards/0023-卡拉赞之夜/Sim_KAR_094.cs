namespace Silverfish.Simulation.Cards;

// 致命餐叉
// Deadly Fork
// 亡语：将一张3/2的武器牌置入你的手牌。
// Deathrattle: Add a 3/2 weapon to your hand.
[CardSim("KAR_094")]
internal static class Sim_KAR_094
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new CreateCardEffect("KAR_094a")],
    };
}
