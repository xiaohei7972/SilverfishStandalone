using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 赤红岩床
// Crimson Expanse
// 选择一个 受伤的随从，召唤一个它的休眠一回合的复制。
// Choose a damaged minion. Summon a copy of it that goes Dormant for one turn.
internal static class Sim_DEEP_019
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_DAMAGED_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
