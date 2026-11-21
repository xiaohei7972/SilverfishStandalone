using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 数量压制
// Overwhelm
// 对一个随从造成$2点伤害。你每控制一只野兽，便多造成一点伤害。
// Deal $2 damage to a minion. Deal one more damage for each Beast you control.
internal static class Sim_SCH_604
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
