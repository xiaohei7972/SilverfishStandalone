using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大地震击
// Earth Shock
// 沉默一个随从，然后对其造成$1点伤害。
// Silence a minion, then deal $1 damage to it.
internal static class Sim_VAN_EX1_245
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
