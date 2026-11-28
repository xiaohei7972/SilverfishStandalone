using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 猛击
// Slam
// 对一个随从造成$2点伤害，如果 它依然存活，则抽一张牌。
// Deal $2 damage to a minion. If it survives, draw a card.
[CardSim("EX1_391")]
internal static class Sim_EX1_391
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.Selected, 2),
            new ConditionalEffect(new CharacterStateCondition(ConditionCharacter.Selected, CharacterDisposition.Alive), [new DrawEffect(1)])
        ]
    };
}
