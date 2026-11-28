using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 永恒奴役
// Eternal Servitude
// 发现一个在本局对战中死亡的友方随从，并召唤该随从。
// Discover a friendly minion that died this game. Summon it.
internal static class Sim_ICC_213
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_GAME)],
        []);
    // </legacy-play-requirements>

}
