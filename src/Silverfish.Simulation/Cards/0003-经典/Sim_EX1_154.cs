using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 愤怒
// Wrath
// 抉择： 对一个随从造成$3点伤害；或者造成$1点伤害并抽一张牌。
// Choose One - Deal $3 damage to a minion; or $1 damage and draw a card.
[CardSim("EX1_154")]
internal static class Sim_EX1_154
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("EX1_154a", [new DamageEffect(DamageTarget.Selected, 3)]),
            new("EX1_154b", [new DamageEffect(DamageTarget.Selected, 1), new DrawEffect(1)])
        ]
    };
}
