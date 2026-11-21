using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 展览馆守卫
// Menagerie Warden
// 战吼：选择一个友方野兽，召唤一个它的复制。
// Battlecry: Choose a friendly Beast. Summon a_copy of it.
internal static class Sim_KAR_065
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
