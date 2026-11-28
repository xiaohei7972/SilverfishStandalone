namespace Silverfish.Simulation.Cards;

// 禁忌仪式
// Forbidden Ritual
// 消耗你所有的法力值，召唤相同数量的1/1的触须。
// Spend all your Mana. Summon that many 1/1 Tentacles.
[CardSim("OG_114")]
internal static class Sim_OG_114
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SpendRemainingManaEffect(ManaSpendReward.SummonTokens, "OG_114a"),
        ],
    };
}
