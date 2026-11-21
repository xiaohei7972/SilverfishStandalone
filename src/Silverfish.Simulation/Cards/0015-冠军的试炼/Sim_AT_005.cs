using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 变形术：野猪
// Polymorph: Boar
// 使一个随从变形成为一个4/2并具有冲锋的野猪。
// Transform a minion into a 4/2 Boar with Charge.
internal static class Sim_AT_005
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
