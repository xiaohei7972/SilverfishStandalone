using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 天空之王
// Lord of Skies
// 对另一个随从造成20点伤害。
// Deal 20 damage to another minion.
internal static class Sim_TTN_800t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
