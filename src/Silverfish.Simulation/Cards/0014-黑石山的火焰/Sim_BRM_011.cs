namespace Silverfish.Simulation.Cards;

// 熔岩震击
// Lava Shock
// 造成$2点伤害。 将你所有过载的法力水晶解锁。
// Deal $2 damage. Unlock your Overloaded Mana Crystals.
[CardSim("BRM_011")]
internal static class Sim_BRM_011
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Character,
        OnPlay = [new DamageEffect(DamageTarget.Selected, 2), new UnlockOverloadEffect()]
    };
}
