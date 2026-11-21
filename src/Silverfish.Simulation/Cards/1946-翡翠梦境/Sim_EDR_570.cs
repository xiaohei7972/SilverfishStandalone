using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 凶险梦魇
// Ominous Nightmares
// 抉择：对所有随从造成$1点伤害；或者使一个受伤的随从获得+2/+2。
// Choose One - Deal $1 damage to all minions; or Give a damaged minion +2/+2.
internal static class Sim_EDR_570
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_DAMAGED_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
