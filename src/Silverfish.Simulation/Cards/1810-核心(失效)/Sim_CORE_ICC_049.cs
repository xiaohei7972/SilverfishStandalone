using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 剧毒箭矢
// Toxic Arrow
// 对一个随从造成$2点伤害，如果它依然存活，则使其获得剧毒。
// Deal $2 damage to a minion. If it survives, give it Poisonous.
internal static class Sim_CORE_ICC_049
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
