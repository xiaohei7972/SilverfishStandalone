using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;
namespace Silverfish.Simulation.Cards;

// 新鲜气息
// Fresh Scent
// 使一个野兽获得+2/+2。
// Give a Beast +2/+2.
[CardSim("YOD_005ts")]
internal static class Sim_YOD_005ts
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_YOD_005e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
