using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 脱水
// Dehydrate
// 吸血。对一个随从造成$4点伤害。快枪：法力值消耗为（1）点。
// Lifesteal. Deal $4 damage to a minion. Quickdraw: Costs (1).
internal static class Sim_WW_325
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
