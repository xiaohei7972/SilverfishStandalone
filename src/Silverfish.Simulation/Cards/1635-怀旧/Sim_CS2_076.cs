namespace Silverfish.Simulation.Cards;

// 刺杀
// Assassinate
// 消灭一个敌方随从。
// Destroy an enemy minion.
[CardSim("CS2_076")]
internal static class Sim_CS2_076
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.EnemyMinion,
        OnPlay = [new DestroyMinionEffect()]
    };
}
