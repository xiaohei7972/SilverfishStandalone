using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 恶魔之火
// Demonfire
// 对一个随从造成$2点伤害，如果该随从是友方恶魔，则改为使其获得+2/+2。
// Deal $2 damage to a minion. If it’s a friendly Demon, give it +2/+2 instead.
[CardSim("EX1_596")]
internal static class Sim_EX1_596
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
            new ConditionalEffect(
                new CharacterMatchCondition(ConditionCharacter.Selected, TriggerRelation.Friendly, new() { MinionType = MinionType.Demon }),
                [Sim_EX1_596e.Rule.ApplyTo(StatTarget.Selected)])
            {
                Otherwise = [new DamageEffect(DamageTarget.Selected, 2)]
            }
        ]
    };
}
