using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 元尊之力
// Progenitor's Power
// 造成@点伤害。
// Deal @ damage.
internal static class Sim_TTN_075t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
