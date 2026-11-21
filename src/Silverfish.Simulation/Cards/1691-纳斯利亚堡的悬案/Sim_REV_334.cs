using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 骄傲罪责
// Burden of Pride
// 召唤三个1/3并具有嘲讽的狱卒。如果你的生命值小于或等于20点，使其获得+1/+1。
// Summon three 1/3 Jailers with Taunt. If you have 20 or less Health, give them +1/+1.
internal static class Sim_REV_334
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
