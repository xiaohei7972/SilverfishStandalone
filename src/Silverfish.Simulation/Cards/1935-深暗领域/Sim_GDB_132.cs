using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 躁动的愤怒卫士
// Relentless Wrathguard
// 战吼：对一个敌方随从造成2点伤害。如果该随从死亡，发现一张恶魔牌。
// Battlecry: Deal 2 damage to an enemy minion. If it dies, Discover a Demon.
internal static class Sim_GDB_132
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
