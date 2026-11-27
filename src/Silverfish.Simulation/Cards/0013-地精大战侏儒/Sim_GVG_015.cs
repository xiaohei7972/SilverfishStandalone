using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 暗色炸弹
// Darkbomb
// 对一个角色造成$3点伤害。如果该角色死亡，抽一张暗影法术牌。
// Deal $3 damage to a character. If it dies, draw a Shadow spell.
[CardSim("GVG_015")]
internal static class Sim_GVG_015
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.Selected, 3),
            new ConditionalEffect(
                new CharacterStateCondition(ConditionCharacter.Selected, CharacterDisposition.Dead),
                [new DrawMatchingEffect(new CardFilter { Kind = EntityKind.Spell, SpellSchool = SpellSchool.Shadow })])
        ]
    };
}
