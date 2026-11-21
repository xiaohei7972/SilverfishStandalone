using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 雷鸣刺喉龙
// Crackling Razormaw
// 战吼：进化一个友方野兽。
// Battlecry: Adapt a friendly_Beast.
internal static class Sim_UNG_915
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
