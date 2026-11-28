using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 铁木树皮
// Ironbark
// 使一个随从获得+1/+3和嘲讽。如果你拥有至少七个法力水晶，则法力值消耗为（0）点。
// Give a minion +1/+3 and Taunt. Costs (0) if you have at least 7 Mana Crystals.
internal static class Sim_BT_132
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
