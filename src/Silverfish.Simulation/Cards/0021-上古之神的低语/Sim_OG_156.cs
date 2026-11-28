namespace Silverfish.Simulation.Cards;

// 怒鳍猎潮者
// Bilefin Tidehunter
// 战吼：召唤一个1/1并具有嘲讽的软泥怪。
// Battlecry: Summon a 1/1 Ooze with Taunt.
[CardSim("OG_156")]
internal static class Sim_OG_156
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonEffect("OG_156a"),
        ],
    };
}
