using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 闪电吐息
// Lightning Breath
// 对一个随从造成$4点伤害。如果你的手牌中有龙牌，则同样对其相邻随从造成伤害。
// Deal $4 damage to a minion. If you're holding a Dragon, also damage its neighbors.
internal static class Sim_DRG_219
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
