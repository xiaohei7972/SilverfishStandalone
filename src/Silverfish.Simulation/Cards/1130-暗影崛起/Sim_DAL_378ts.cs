using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 猛兽出笼
// Unleash the Beast
// 召唤一只5/5并具有突袭的双足飞龙。
// Summon a 5/5 Wyvern with Rush.
internal static class Sim_DAL_378ts
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
