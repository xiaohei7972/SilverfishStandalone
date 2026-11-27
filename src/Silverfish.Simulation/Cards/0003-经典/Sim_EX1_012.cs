namespace Silverfish.Simulation.Cards;

// 血法师萨尔诺斯
// Bloodmage Thalnos
// 法术伤害+1，亡语：抽一张牌。
// Spell Damage +1 Deathrattle: Draw a card.
[CardSim("EX1_012")]
internal static class Sim_EX1_012
{
    internal static CardRules Rules { get; } = new()
    {
        SpellDamage = 1,
        Deathrattle = [new DrawEffect(1)]
    };
}
