using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 第四封印
// The Fourth Seal
// 召唤一个5/5的恶魔。将“终极封印”置入你的手牌。
// Summon a 5/5 Demon. Add 'The Final Seal' to your hand.
internal static class Sim_LOOT_415t4
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
