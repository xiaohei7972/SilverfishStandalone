using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 龙息术
// Dragon's Breath
// 造成$4点伤害。在本回合中每有一个随从死亡，本牌的法力值消耗便减少（1）点。
// Deal $4 damage. Costs (1) less for each minion that died this turn.
[CardSim("BRM_003")]
internal static class Sim_BRM_003
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.MinionsDiedThisTurn,
        OnPlay =
        [
            new DamageEffect(DamageTarget.Selected, 4)
        ]
    };
}
