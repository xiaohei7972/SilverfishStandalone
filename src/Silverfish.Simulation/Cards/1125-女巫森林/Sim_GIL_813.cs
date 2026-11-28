using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 鲜活梦魇
// Vivid Nightmare
// 选择一个友方随从，召唤一个该随从的复制，且剩余生命值为1点。
// Choose a friendly minion. Summon a copy of it with 1 Health remaining.
internal static class Sim_GIL_813
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
