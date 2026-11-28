using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 奥术冲击
// Arcane Blast
// 对一个随从造成$2点伤害。该法术受到的法术伤害增益效果翻倍。
// Deal $2 damage to a minion. This spell gets double bonus from Spell Damage.
[CardSim("AT_004")]
internal static class Sim_AT_004
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
            new DamageEffect(DamageTarget.Selected, 2),
        ],
        SpellDamageBonusMultiplier = 2,
    };
}
