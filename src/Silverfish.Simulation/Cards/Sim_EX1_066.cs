namespace Silverfish.Simulation.Cards;

[CardSim("EX1_066")]
internal static class Sim_EX1_066
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyEnemyWeaponEffect()]
    };
}
