namespace Silverfish.Simulation.Cards;

// 虚灵大盗拉法姆
// Arch-Thief Rafaam
// 战吼：发现一张强大的神器牌。
// Battlecry: Discover a powerful Artifact.
[CardSim("LOE_092")]
internal static class Sim_LOE_092
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice() { Options = ["LOEA16_3", "LOEA16_4", "LOEA16_5"] },
    };
}
