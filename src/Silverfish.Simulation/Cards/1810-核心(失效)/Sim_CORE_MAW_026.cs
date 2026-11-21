using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 监禁
// Incarceration
// 选择一个随从。使其休眠3回合。
// Choose a minion. It goes Dormant for 3 turns.
internal static class Sim_CORE_MAW_026
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
