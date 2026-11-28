using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 青玉飞镖
// Jade Shuriken
// 造成$3点伤害。 连击：召唤一个{0}的青玉魔像。@造成$3点伤害。 连击：召唤一个青玉魔像。
// Deal $3 damage. Combo: Summon a{1} {0} Jade Golem.@Deal $3 damage. Combo: Summon a Jade Golem.
[CardSim("CFM_690")]
internal static class Sim_CFM_690
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
            new ConditionalEffect(new ComboCondition(), [new SummonJadeGolemEffect()]),
        ],
    };
}
