using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 分裂软泥怪
// Echoing Ooze
// 战吼：在回合结束时召唤一个本随从的复制。
// Battlecry: Summon an exact copy of this minion at the end of the turn.
[CardSim("FP1_003")]
internal static class Sim_FP1_003
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(
                StatTarget.Source,
                null,
                EnchantmentDuration.CurrentTurn)
            {
                EnchantmentCardId = "FP1_003e",
                OnRemoved =
                [
                    new(
                        EnchantmentRemovalReason.Expired,
                        [new SummonCopyEffect(EffectTarget.TriggerSubject)])
                ]
            }
        ]
    };
}
