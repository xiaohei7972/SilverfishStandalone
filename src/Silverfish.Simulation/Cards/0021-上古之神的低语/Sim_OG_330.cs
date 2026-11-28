namespace Silverfish.Simulation.Cards;

// 幽暗城商贩
// Undercity Huckster
// 亡语：&随机获取一张（你对手职业的）卡牌。
// Deathrattle:& Get a random card (from your opponent's class).
[CardSim("OG_330")]
internal static class Sim_OG_330
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new CreateRandomOpponentClassSpellEffect() { IncludeAllKinds = true },
        ],
    };
}
