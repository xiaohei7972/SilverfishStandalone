using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 野性狼魂
// Feral Spirit
// 召唤两只2/3并具有嘲讽的幽灵狼。过载：（1）
// Summon two 2/3 Spirit Wolves with Taunt. Overload: (1)
[CardSim("EX1_248")]
internal static class Sim_EX1_248
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonEffect("EX1_tk11"), new SummonEffect("EX1_tk11")]
    };
}
