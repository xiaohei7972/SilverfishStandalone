using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 食肉格块
// Carnivorous Cubicle
// 战吼：消灭一个友方随从。在你的回合结束时，召唤一个它的复制。
// Battlecry: Destroy a friendly minion. Summon a copy of it at the end of your turns.
internal static class Sim_WORK_042
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
