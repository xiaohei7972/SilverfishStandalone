using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 终极封印
// The Final Seal
// 召唤一个6/6的恶魔。将“吞噬者阿扎里”置入你的手牌。
// Summon a 6/6 Demon. Add 'Azari, the Devourer' to your hand.
internal static class Sim_LOOT_415t5
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
