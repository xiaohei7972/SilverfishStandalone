using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 气闸破损
// Airlock Breach
// 召唤一个5/5并具有嘲讽的亡灵，并使你的英雄获得+5生命值。消耗5份残骸，重复一次。
// Summon a 5/5 Undead with Taunt and give your hero +5 Health. Spend 5 Corpses to do it again.
internal static class Sim_GDB_113
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
