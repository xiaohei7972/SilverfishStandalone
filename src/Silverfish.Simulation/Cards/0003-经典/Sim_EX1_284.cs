namespace Silverfish.Simulation.Cards;

// 碧蓝幼龙
// Azure Drake
// 法术伤害+1，战吼：抽一张牌。
// Spell Damage +1 Battlecry: Draw a card.
[CardSim("EX1_284")]
internal static class Sim_EX1_284
{
    internal static CardRules Rules { get; } = new()
    {
        SpellDamage = 1,
        OnPlay = [new DrawEffect(1)]
    };
}
