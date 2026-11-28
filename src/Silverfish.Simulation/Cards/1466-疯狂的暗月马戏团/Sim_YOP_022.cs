using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迷雾行者
// Mistrunner
// 战吼：使一个友方随从获得+3/+3。 过载：（1）
// Battlecry: Give a friendly minion +3/+3. Overload: (1)
internal static class Sim_YOP_022
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
