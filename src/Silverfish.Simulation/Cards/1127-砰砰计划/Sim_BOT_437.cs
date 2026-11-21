using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 地精的把戏
// Goblin Prank
// 使一个友方随从获得+3/+3和突袭，该随从会在回合结束时死亡。
// Give a friendly minion +3/+3 and Rush. It_dies at end of turn.
internal static class Sim_BOT_437
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
