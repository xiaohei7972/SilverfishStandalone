using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 祝福
// Bless
// 使一个随从获得+2生命值，然后使其攻击力等同于其生命值。
// Give a minion +2 Health, then set its Attack to be equal to its Health.
internal static class Sim_AV_329
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
