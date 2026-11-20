namespace Silverfish.Simulation.Cards;

// 寒冰箭
// Frostbolt
// 对一个角色造成$3点伤害，并使其冻结。
// Deal $3 damage to a_character and Freeze it.
[CardSim("CS2_024")]
internal static class Sim_CS2_024
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Character,
        OnPlay = [new DamageEffect(DamageTarget.Selected, 3), new FreezeEffect(FreezeTarget.Selected)]
    };
}
