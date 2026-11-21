using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 安拉斐特之核
// Anraphet's Core
// 召唤一个4/3的魔像。在你的英雄攻击后，复原此技能。
// Summon a 4/3 Golem. After your hero attacks, refresh this.
internal static class Sim_ULD_711p3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
