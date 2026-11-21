using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 一桶欢猢
// Barrel of Monkeys
// 召唤一只1/4并具有嘲讽的猴子。（还剩3只猴子！）
// Summon a 1/4 Monkey with Taunt. (3 Monkeys left!)
internal static class Sim_ETC_207
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
