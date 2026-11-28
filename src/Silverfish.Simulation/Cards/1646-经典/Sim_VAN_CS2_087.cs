using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 力量祝福
// Blessing of Might
// 使一个随从获得+3攻击力。
// Give a minion +3_Attack.
internal static class Sim_VAN_CS2_087
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
