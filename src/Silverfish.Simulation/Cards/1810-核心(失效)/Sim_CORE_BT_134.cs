using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 沼泽射线
// Bogbeam
// 对一个随从造成$3点伤害。如果你拥有至少七个法力水晶，则法力值消耗为（0）点。
// Deal $3 damage to_a minion. Costs (0) if you have at least 7 Mana Crystals.
internal static class Sim_CORE_BT_134
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
