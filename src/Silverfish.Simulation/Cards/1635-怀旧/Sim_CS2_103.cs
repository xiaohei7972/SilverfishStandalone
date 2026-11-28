using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冲锋
// Charge
// 使一个友方随从获得+2攻击力和冲锋。
// Give a friendly minion +2 Attack and Charge.
internal static class Sim_CS2_103
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
