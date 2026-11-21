using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 熔火吐息
// Molten Breath
// 对一个随从造成$5点伤害。如果你的手牌中有龙牌，便获得5点护甲值。
// Deal $5 damage to a minion. If you're holding a Dragon, gain 5 Armor.
internal static class Sim_DRG_500
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
