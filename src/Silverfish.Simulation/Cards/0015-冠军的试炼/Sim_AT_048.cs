using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 治疗波
// Healing Wave
// 恢复#8点生命值。揭示双方牌库里的一张随从牌。如果你的牌法力值 消耗较大，改为恢复#16点生命值。
// Restore #8 Health. Reveal a minion in each deck. If yours costs more, restore #16 instead.
[CardSim("AT_048")]
internal static class Sim_AT_048
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new JoustEffect([new HealEffect(HealTarget.Selected, 16)]) { Otherwise = [new HealEffect(HealTarget.Selected, 8)] },
        ],
    };
}
