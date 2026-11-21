using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 圣疗术
// Lay on Hands
// 恢复#8点生命值，抽三张牌。
// Restore #8 Health. Draw_3 cards.
internal static class Sim_EX1_354
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
