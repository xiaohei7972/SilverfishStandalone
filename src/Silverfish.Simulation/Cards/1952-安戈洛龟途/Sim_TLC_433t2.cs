using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恐怖之墓
// Terror's Grave
// 造成4点伤害。亡语：再次召唤泰拉克斯，魔骸暴龙。
// Deal 4 damage. Deathrattle: Resummon Tyrax, Bone Terror.
internal static class Sim_TLC_433t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)]);
    // </legacy-play-requirements>

}
