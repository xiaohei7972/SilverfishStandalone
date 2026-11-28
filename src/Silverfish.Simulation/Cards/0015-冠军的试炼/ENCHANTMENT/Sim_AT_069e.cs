using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 训练完成
// Training Complete
// 嘲讽
// Taunt
internal static class Sim_AT_069e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_069e", null)
        {
            GrantedTag = GAME_TAG.TAUNT,
        };
}
