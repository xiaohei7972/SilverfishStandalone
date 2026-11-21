using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迪菲亚清道夫
// Defias Cleaner
// 战吼：沉默一个具有亡语的随从。
// Battlecry: Silence a minion with Deathrattle.
internal static class Sim_CFM_855
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
