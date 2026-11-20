namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_066
{
    internal const string CardId = "EX1_066";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyEnemyWeaponEffect()]
    };
}
