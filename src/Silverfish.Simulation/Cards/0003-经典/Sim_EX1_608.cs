namespace Silverfish.Simulation.Cards;

// 巫师学徒
// Sorcerer's Apprentice
// 你的法术的法力值消耗减少（1）点（但不能少于1点）。
// Your spells cost (1) less (but not less than 1).
[CardSim("EX1_608")]
internal static class Sim_EX1_608
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras = [new(CostAuraRecipient.Friendly, CostAuraTarget.Spell, -1) { MinimumCost = 1 }]
    };
}
