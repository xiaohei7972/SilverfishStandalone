using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 寒冰屏障
// Ice Block
// 在本回合中，你的英雄免疫。
// Your hero is Immune this turn.
internal static class Sim_EX1_295o
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_295o", null, EnchantmentDuration.CurrentTurn) { GrantedTag = GAME_TAG.IMMUNE };
}
