namespace Silverfish.Simulation.Cards;

// 哀嚎的灵魂
// Wailing Soul
// 战吼：沉默你的其他随从。
// Battlecry: Silence your other minions.
[CardSim("FP1_016")]
internal static class Sim_FP1_016
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SilenceEffect(OtherFriendlyMinions: true)
        ]
    };
}
