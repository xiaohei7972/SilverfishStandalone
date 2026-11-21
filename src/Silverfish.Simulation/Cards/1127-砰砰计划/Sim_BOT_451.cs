using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 流电爆裂
// Voltaic Burst
// 召唤两个1/1并具有突袭的“火花”。过载：（1）
// Summon two 1/1 Sparks with Rush. Overload: (1)
internal static class Sim_BOT_451
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
