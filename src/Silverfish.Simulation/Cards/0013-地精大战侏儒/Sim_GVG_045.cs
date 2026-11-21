using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小鬼爆破
// Imp-losion
// 对一个随从造成$2-$4点伤害。每造成1点伤害，便召唤一个1/1的小鬼。
// Deal $2-$4 damage to a minion. Summon a 1/1 Imp for each damage dealt.
internal static class Sim_GVG_045
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
