using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 第三封印
// The Third Seal
// 召唤一个4/4的恶魔。将“第四封印”置入你的手牌。
// Summon a 4/4 Demon. Add 'The Fourth Seal' to your hand.
internal static class Sim_LOOT_415t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
