using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 雷暴术
// Stormcrack
// 对一个随从造成$4点伤害，过载：（1）
// Deal $4 damage to a minion. Overload: (1)
[CardSim("OG_206")]
internal static class Sim_OG_206
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.Selected, 4),
        ],
    };
}
