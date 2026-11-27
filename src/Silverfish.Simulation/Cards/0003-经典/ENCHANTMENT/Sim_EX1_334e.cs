using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 暗影狂乱
// Shadow Madness
// 在本回合中，本随从的控制权发生了变化。
// This minion has switched controllers this turn.
internal static class Sim_EX1_334e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_334e", null, EnchantmentDuration.CurrentTurn)
        {
            GrantedTag = GAME_TAG.CONTROLLER,
            OnRemoved =
            [
                new(EnchantmentRemovalReason.Expired, [new TakeControlEffect(EffectTarget.TriggerSubject, ReturnToSourceOpponent: true)]),
                new(EnchantmentRemovalReason.Silenced, [new TakeControlEffect(EffectTarget.TriggerSubject, ReturnToSourceOpponent: true)])
            ]
        };
}
