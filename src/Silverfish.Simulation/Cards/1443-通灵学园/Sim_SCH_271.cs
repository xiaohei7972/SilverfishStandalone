using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 岩浆爆裂
// Molten Blast
// 造成$2点伤害，召唤相同数量的1/1的元素。
// Deal $2 damage. Summon that many 1/1 Elementals.
internal static class Sim_SCH_271
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
