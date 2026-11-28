using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奔雷骏马
// Thunderbringer
// 嘲讽。亡语：从你的牌库中召唤一个元素和一只野兽。
// Taunt Deathrattle: Summon an Elemental and Beast from your deck.
internal static class Sim_WW_440
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 2)],
        []);
    // </legacy-play-requirements>

}
