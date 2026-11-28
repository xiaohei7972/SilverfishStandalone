using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 沟渠潜伏者
// Moat Lurker
// 消灭{0}。
// Destroyed {0}.
internal static class Sim_KAR_041e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("KAR_041e", null)
        {
            GrantedTag = GAME_TAG.DEATHRATTLE,
            OnRemoved = [new(EnchantmentRemovalReason.Died, [new ResummonCapturedMinionEffect()])],
        };
}
