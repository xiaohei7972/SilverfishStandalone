using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 熔岩镜像
// Molten Reflection
// 选择一个友方随从，召唤一个该随从的复制。
// Choose a friendly minion. Summon a copy of it.
internal static class Sim_UNG_948
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
