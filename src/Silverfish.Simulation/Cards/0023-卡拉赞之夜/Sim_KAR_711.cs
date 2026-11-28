namespace Silverfish.Simulation.Cards;

// 奥术巨人
// Arcane Giant
// 在本局对战中，你每施放一个法术，本牌的法力值消耗便减少（1）点。
// Costs (1) less for each spell you've cast this game.
[CardSim("KAR_711")]
internal static class Sim_KAR_711
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.SpellsCast,
    };
}
