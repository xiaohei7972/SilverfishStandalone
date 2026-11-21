using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 巫术时刻
// Witching Hour
// 随机召唤一个在本局对战中死亡的友方野兽。
// Summon a random friendly Beast that died this game.
internal static class Sim_GIL_571
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
