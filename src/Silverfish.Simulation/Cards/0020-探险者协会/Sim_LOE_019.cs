using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 化石迅猛龙
// Unearthed Raptor
// 战吼：选择一个友方随从，获得其亡语的复制。
// Battlecry: Choose a friendly minion. Gain a copy of its_Deathrattle.
internal static class Sim_LOE_019
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
