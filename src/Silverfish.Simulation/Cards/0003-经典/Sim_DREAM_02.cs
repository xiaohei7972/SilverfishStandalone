namespace Silverfish.Simulation.Cards;

// 伊瑟拉苏醒
// Ysera Awakens
// 对除了伊瑟拉之外的所有角色造成$5点伤害。
// Deal $5 damage to all characters except Ysera.
[CardSim("DREAM_02")]
internal static class Sim_DREAM_02
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.AllCharacters, 5) { ExcludeCardId = "EX1_572" }]
    };
}
