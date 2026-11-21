using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奥术吐息
// Arcane Breath
// 对一个随从造成$2点伤害。如果你的手牌中有龙牌，便发现一张法术牌。
// Deal $2 damage to a minion. If you're holding a Dragon, Discover a spell.
internal static class Sim_DRG_106
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
