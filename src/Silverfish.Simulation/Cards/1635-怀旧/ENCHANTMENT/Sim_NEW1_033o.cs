using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 天空之眼
// Eye In The Sky
// 雷欧克使本随从获得+1攻击力。
// Leokk is granting this minion +1 Attack.
internal static class Sim_NEW1_033o
{
    internal const string CardId = "NEW1_033o";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Add, Attack: 1));
}
