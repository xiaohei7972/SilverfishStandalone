using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 末日灾祸
// Bane of Doom
// 对一个角色造成$3点伤害。如果该角色死亡，随机召唤一个恶魔。
// Deal $3 damage to a character. If it dies, summon a random Demon.
[CardSim("EX1_320")]
internal static class Sim_EX1_320
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
            new DamageEffect(DamageTarget.Selected, 3),
            new ConditionalEffect(new CharacterStateCondition(ConditionCharacter.Selected, CharacterDisposition.Dead),
                [new SummonMatchingMinionEffect(new CardFilter { MinionType = MinionType.Demon })])
        ]
    };
}
