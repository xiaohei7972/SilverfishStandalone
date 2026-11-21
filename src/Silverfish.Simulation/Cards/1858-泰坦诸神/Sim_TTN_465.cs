using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 意志熔炉
// Forge of Wills
// 选择一个友方随从。召唤一个具有其属性值和突袭的 巨人。
// Choose a friendly minion. Summon a Giant with its stats and Rush.
internal static class Sim_TTN_465
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
