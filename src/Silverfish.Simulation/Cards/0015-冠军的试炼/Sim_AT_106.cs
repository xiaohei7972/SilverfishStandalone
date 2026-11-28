using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 圣光勇士
// Light's Champion
// 战吼： 沉默一个恶魔。
// Battlecry: Silence a Demon.
[CardSim("AT_106")]
internal static class Sim_AT_106
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DEMON), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SilenceEffect(),
        ],
    };
}
