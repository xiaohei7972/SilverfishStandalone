using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 太阳看护者
// Watcher of the Sun
// 战吼：随机获取一张神圣法术牌。锻造：还会为你的英雄恢复#6点生命值。
// Battlecry: Get a random Holy spell. Forge: Also restore #6 Health to your hero.
internal static class Sim_TTN_039
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FORGE)],
        []);
    // </legacy-play-requirements>

}
