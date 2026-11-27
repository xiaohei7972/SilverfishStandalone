using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 姆若咯啦啊咯啦！
// Mrgglaargl!
// 鱼人领军使其获得+2攻击力。
// +2 Attack from Murloc Warleader.
internal static class Sim_EX1_507e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_507e", new(StatModifierMode.Add, Attack: 2));
}
