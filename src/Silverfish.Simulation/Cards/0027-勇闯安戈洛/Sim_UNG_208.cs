using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 岩石哨兵
// Stone Sentinel
// 战吼：如果你在上个回合使用过元素牌，则召唤两个2/3并具有嘲讽的元素。
// Battlecry: If you played an Elemental last turn, summon two 2/3 Elementals with Taunt.
internal static class Sim_UNG_208
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE)],
        []);
    // </legacy-play-requirements>

}
