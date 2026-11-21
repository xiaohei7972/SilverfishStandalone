using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 纯净藤蔓
// Purifying Vines
// 选择一个随从。如果是友方随从，使其获得+2生命值；如果是敌方随从，使其获得-2生命值。
// Choose a minion. If it's friendly, give it +2 Health. If it's an enemy, give it -2 Health.
internal static class Sim_TLC_813
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
