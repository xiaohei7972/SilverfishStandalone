using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 禁忌治疗
// Forbidden Healing
// 消耗你所有的法力值，恢复等同于所消耗法力值数量两倍的生命值。
// Spend all your Mana. Restore twice that much Health.
[CardSim("OG_198")]
internal static class Sim_OG_198
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
            new SpendRemainingManaEffect(ManaSpendReward.HealSelected),
        ],
    };
}
