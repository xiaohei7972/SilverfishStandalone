using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 宁静旷林
// Tranquil Clearing
// 使一个随从获得+2生命值和嘲讽。直到你的下个回合结束，该随从会陷入沉睡。
// Give a minion +2 Health and Taunt. It falls asleep until the end of your next turn.
internal static class Sim_MEND_044
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)]);
    // </legacy-play-requirements>

}
