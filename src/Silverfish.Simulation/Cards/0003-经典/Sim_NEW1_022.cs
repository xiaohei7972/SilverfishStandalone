namespace Silverfish.Simulation.Cards;

// 恐怖海盗
// Dread Corsair
// 嘲讽 你的武器每有1点攻击力，本牌的法力值消耗便减少（1）点。
// Taunt Costs (1) less per Attack of_your weapon.
[CardSim("NEW1_022")]
internal static class Sim_NEW1_022
{
    internal static CardRules Rules { get; } = new()
    {
        SelfCostReduction = SelfCostReduction.EquippedWeaponAttack
    };
}
