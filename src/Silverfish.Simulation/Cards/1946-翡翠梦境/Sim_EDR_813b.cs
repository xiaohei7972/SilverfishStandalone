using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 尸虫噬咬
// Bug Bites
// 消耗2份残骸，对一个随从造成$4点伤害。
// Spend 2 Corpses to deal $4 damage to a_minion.
internal static class Sim_EDR_813b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_MINIMUM_CORPSES, 2)],
        []);
    // </legacy-play-requirements>

}
