using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 先祖之魂
// Ancestral Spirit
// 亡语：再次召唤本随从。
// Deathrattle: Resummon this minion.
internal static class Sim_CS2_038e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("CS2_038e", null)
        {
            GrantedTag = GAME_TAG.DEATHRATTLE,
            OnRemoved = [new(EnchantmentRemovalReason.Died, [new ResummonMinionEffect(EffectTarget.TriggerSubject)])]
        };
}
