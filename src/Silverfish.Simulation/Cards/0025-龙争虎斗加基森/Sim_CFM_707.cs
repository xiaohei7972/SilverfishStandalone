using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 青玉闪电
// Jade Lightning
// 造成$3点伤害，召唤一个{0}的青玉魔像。@造成$3点伤害，召唤一个青玉魔像。
// Deal $3 damage. Summon a{1} {0} Jade Golem.@Deal $3 damage. Summon a Jade Golem.
[CardSim("CFM_707")]
internal static class Sim_CFM_707
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Selected, 3), new SummonJadeGolemEffect()],
    };
}
