using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 禁忌烈焰
// Forbidden Flame
// 消耗你所有的法力值，对一个随从造成等同于所消耗法力值数量的伤害。
// Spend all your Mana. Deal that much damage to a minion.
internal static class Sim_OG_086
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
