namespace Silverfish.Simulation.Cards;

// 闪电箭
// Lightning Bolt
// 造成$3点伤害，过载：（1）
// Deal $3 damage. Overload: (1)
[CardSim("EX1_238")]
internal static class Sim_EX1_238
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Character,
        OnPlay = [new DamageEffect(DamageTarget.Selected, 3)]
    };
}
