using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 食肉魔块
// Carnivorous Cube
// 战吼： 消灭一个友方随从。 亡语：召唤两个被消灭随从的复制。
// Battlecry: Destroy a friendly minion. Deathrattle: Summon 2 copies of it.
internal static class Sim_LOOT_161
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
