namespace Silverfish.Simulation.Cards;

// 禁忌畸变
// Forbidden Shaping
// 消耗你所有的法力值，随机 召唤一个法力值消耗相同的随从。
// Spend all your Mana. Summon a random minion that costs that much.
[CardSim("OG_101")]
internal static class Sim_OG_101
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SpendRemainingManaEffect(ManaSpendReward.SummonMatchingCost),
        ],
    };
}
