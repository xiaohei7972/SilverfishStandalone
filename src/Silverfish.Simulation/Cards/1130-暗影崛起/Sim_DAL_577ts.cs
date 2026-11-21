using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 霜冻射线
// Ray of Frost
// 冻结一个随从。如果该随从已被冻结，则对其造成$2点伤害。
// Freeze a minion. If it's already Frozen, deal $2 damage to it.
internal static class Sim_DAL_577ts
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
