namespace Silverfish.Simulation.Cards;

// 先知维伦
// Prophet Velen
// 使你的法术和英雄技能的伤害和治疗效果翻倍。
// Double the damage and healing of your spells and Hero Power.
[CardSim("EX1_350")]
internal static class Sim_EX1_350
{
    internal static CardRules Rules { get; } = new()
    {
        SpellAndHeroPowerMultiplier = 2
    };
}
