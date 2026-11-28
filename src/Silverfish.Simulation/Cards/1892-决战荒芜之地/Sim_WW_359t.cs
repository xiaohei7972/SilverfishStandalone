using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 荒芜之地监狱
// Badlands Jail
// 使一个随从休眠3回合。
// Make a minion go Dormant for 3 turns.
internal static class Sim_WW_359t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
