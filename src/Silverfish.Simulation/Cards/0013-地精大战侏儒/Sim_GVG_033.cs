namespace Silverfish.Simulation.Cards;

// 生命之树
// Tree of Life
// 为所有角色恢复所有生命值。
// Restore all characters to full Health.
[CardSim("GVG_033")]
internal static class Sim_GVG_033
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new HealEffect(HealTarget.AllCharacters, 999)]
    };
}
