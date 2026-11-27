using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 冰枪术
// Ice Lance
// 冻结一个角色，如果该角色已被冻结，则改为对其造成$4点伤害。
// Freeze a character. If it was already Frozen, deal $4 damage instead.
[CardSim("CS2_031")]
internal static class Sim_CS2_031
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
            new ConditionalEffect(new CharacterStateCondition(ConditionCharacter.Selected, CharacterDisposition.Frozen),
                [new DamageEffect(DamageTarget.Selected, 4)])
            {
                Otherwise = [new FreezeEffect(FreezeTarget.Selected)]
            }
        ]
    };
}
