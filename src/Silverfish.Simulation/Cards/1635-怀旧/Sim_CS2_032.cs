using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 烈焰风暴
// Flamestrike
// 对所有敌方随从造成$5点伤害。
// Deal $5 damage to all enemy minions.
[CardSim("CS2_032")]
internal static class Sim_CS2_032
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.EnemyMinions, 5)]
    };
}
