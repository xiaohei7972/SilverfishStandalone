using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 潜踪掠食
// Ambush Predators
// 召唤一只1/1并具有潜行和剧毒的喷毒龙。延系：重复一次。
// Summon a 1/1 Spitter with Stealth and Poisonous. Kindred: Do it again.
internal static class Sim_TLC_519
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
