using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 星辰坠落
// Starfall
// 抉择：对一个随从造成$5点伤害；或者对所有敌方随从造成$2点伤害。
// Choose One - Deal $5 damage to a minion; or $2 damage to all enemy minions.
[CardSim("NEW1_007")]
internal static class Sim_NEW1_007
{
    internal static PlayReqSet PlayReqs { get; } = new([], []);

    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("single", [new DamageEffect(DamageTarget.Selected, 5)])
            {
                PlayReqs = [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]
            },
            new("aoe", [new DamageEffect(DamageTarget.EnemyMinions, 2)])
        ]
    };
}
