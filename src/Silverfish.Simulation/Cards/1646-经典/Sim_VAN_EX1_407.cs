using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 绝命乱斗
// Brawl
// 随机选择一个随从，消灭除了该随从外的所有其他随从。
// Destroy all minions except one. (chosen randomly)
internal static class Sim_VAN_EX1_407
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_TOTAL_MINIONS, 2)],
        []);
    // </legacy-play-requirements>

}
