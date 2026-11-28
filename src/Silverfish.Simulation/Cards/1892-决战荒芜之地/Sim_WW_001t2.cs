using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 喷涌水源
// Water Source
// 恢复#3点生命值。抽一张牌。
// Restore #3 Health. Draw a card.
internal static class Sim_WW_001t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
