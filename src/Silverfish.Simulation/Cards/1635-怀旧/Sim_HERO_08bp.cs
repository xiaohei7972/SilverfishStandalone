namespace Silverfish.Simulation.Cards;

// 火焰冲击
// Fireblast
// 英雄技能 造成$1点伤害。
// Hero Power Deal $1 damage.
[CardSim("HERO_08bp")]
internal static class Sim_HERO_08bp
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Character,
        OnPlay = [new DamageEffect(DamageTarget.Selected, 1)]
    };
}
