namespace Silverfish.Simulation.Cards.Enchantments;

// 真言术：耀
// Power Word: Glory
// 当本随从攻击时，为将其强化的英雄恢复#4点生命值。
// When this attacks, restore #4 Health to the hero of the player who buffed it.
internal static class Sim_AT_013e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_013e", null)
        {
            OnAttack = [new HealFriendlyHeroEffect(4)],
        };
}
