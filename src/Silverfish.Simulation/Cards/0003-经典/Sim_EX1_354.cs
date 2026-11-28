using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 圣疗术
// Lay on Hands
// 恢复#8点生命值，抽三张牌。
// Restore #8 Health. Draw_3 cards.
[CardSim("EX1_354")]
internal static class Sim_EX1_354
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new HealEffect(HealTarget.Selected, 8), new DrawEffect(3)]
    };
}
