using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 蜂群来袭
// BEEEES!!!
// 选择一个随从。召唤四只1/1的蜜蜂攻击该随从。
// Choose a minion. Summon four 1/1 Bees that attack it.
internal static class Sim_ULD_134
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
