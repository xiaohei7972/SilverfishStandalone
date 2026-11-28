namespace Silverfish.Simulation.Cards;

// 剽窃
// Burgle
// 随机获取3张（你对手职业的）卡牌。
// Get 3 random cards (from your opponent's class).
[CardSim("AT_033")]
internal static class Sim_AT_033
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateRandomOpponentClassSpellEffect(3) { IncludeAllKinds = true },
        ],
    };
}
