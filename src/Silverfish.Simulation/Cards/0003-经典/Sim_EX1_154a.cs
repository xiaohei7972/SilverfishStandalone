using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 阳炎之怒
// Solar Wrath
// 对一个随从造成$3点伤害。
// Deal $3 damage to a minion.
internal static class Sim_EX1_154a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
