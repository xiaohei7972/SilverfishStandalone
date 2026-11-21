using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恐鳞追猎者
// Terrorscale Stalker
// 战吼：触发一个友方随从的亡语。
// Battlecry: Trigger a friendly minion's Deathrattle.
internal static class Sim_CORE_UNG_800
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
