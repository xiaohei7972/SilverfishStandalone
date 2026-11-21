using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 荒野的复仇
// Revenge of the Wild
// 召唤在 本回合中死亡的友方野兽。
// Summon your Beasts that died this turn.
internal static class Sim_TRL_566
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
