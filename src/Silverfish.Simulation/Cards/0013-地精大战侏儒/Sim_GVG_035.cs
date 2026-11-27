using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 玛洛恩
// Malorne
// 亡语：进入休眠状态。在2只友方野兽死亡后复活。
// Deathrattle: Go Dormant. Revive after 2 friendly Beasts die.
[CardSim("GVG_035")]
internal static class Sim_GVG_035
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new ScheduleDormantAwakenEffect("GVG_035", 2, MinionType.Beast)]
    };
}
