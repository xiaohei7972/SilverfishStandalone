using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 部落的力量
// Power of the Horde
// 随机召唤一名部落勇士。
// Summon a random Horde Warrior.
internal static class Sim_PRO_001c
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
