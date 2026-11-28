namespace Silverfish.Simulation.Cards;

// 海象人图腾师
// Tuskarr Totemic
// 战吼：随机召唤一个基础图腾。
// Battlecry: Summon a random basic Totem.
[CardSim("AT_046")]
internal static class Sim_AT_046
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonRandomMinionEffect(["NEW1_009", "CS2_050", "CS2_051", "CS2_052"]),
        ],
    };
}
