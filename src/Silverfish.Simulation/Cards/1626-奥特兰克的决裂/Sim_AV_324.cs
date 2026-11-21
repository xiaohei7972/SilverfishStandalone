using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗言术：噬
// Shadow Word: Devour
// 选择一个随从，使其从所有其他随从处各偷取1点生命值。
// Choose a minion. It steals 1 Health from _ALL other minions.
internal static class Sim_AV_324
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
