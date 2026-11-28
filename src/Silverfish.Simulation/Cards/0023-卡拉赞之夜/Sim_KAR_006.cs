namespace Silverfish.Simulation.Cards;

// 神秘女猎手
// Cloaked Huntress
// 你的奥秘牌法力值消耗为（0）点。
// Your Secrets cost (0).
[CardSim("KAR_006")]
internal static class Sim_KAR_006
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras = [new CostAura(CostAuraRecipient.Friendly, CostAuraTarget.Spell, 0) { SecretOnly = true, SetCost = 0 }],
    };
}
