using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 眼棱
// Eye Beam
// 吸血。 对一个随从造成$3点伤害。流放：法力值消耗为（1）点。
// Lifesteal. Deal $3 damage to a minion. Outcast: This costs (1).
internal static class Sim_BT_801
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
