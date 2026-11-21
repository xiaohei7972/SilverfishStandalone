using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 鼓动火焰
// Bellowing Flames
// 已锻造 对一个随从造成$5点伤害。然后造成$5点伤害，随机分配到所有敌方随从身上。
// Forged Deal $5 damage to a minion, then deal $5 damage split among all enemy minions.
internal static class Sim_TTN_753t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
