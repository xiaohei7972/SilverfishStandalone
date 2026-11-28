using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 恶魔融合
// Demonfuse
// 使一个恶魔获得+3/+3。
// Give a Demon +3/+3.
[CardSim("AT_024")]
internal static class Sim_AT_024
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DEMON), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_AT_024e.Rule.ApplyTo(StatTarget.Selected),
        ],
    };
}
