namespace Silverfish.Simulation.Cards;

// 冰霜新星
// Frost Nova
// 冻结所有敌方随从。
// Freeze all enemy minions.
[CardSim("CS2_026")]
internal static class Sim_CS2_026
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new FreezeEffect(FreezeTarget.EnemyMinions)]
    };
}
