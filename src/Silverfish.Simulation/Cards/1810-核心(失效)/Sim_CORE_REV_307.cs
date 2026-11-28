using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 自然死亡
// Natural Causes
// 造成$2点伤害。召唤一个2/2的树人。
// Deal $2 damage. Summon a 2/2 Treant.
internal static class Sim_CORE_REV_307
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
