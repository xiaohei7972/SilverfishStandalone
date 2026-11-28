namespace Silverfish.Simulation.Cards;

// 黑水海盗
// Blackwater Pirate
// 你的武器法力值消耗减少（2）点。
// Your weapons cost (2) less.
[CardSim("OG_322")]
internal static class Sim_OG_322
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras = [new(CostAuraRecipient.Friendly, CostAuraTarget.Weapon, -2)],
    };
}
