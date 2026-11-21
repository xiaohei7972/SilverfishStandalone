namespace Silverfish.Simulation.Cards;

// 酸性沼泽软泥怪
// Acidic Swamp Ooze
// 战吼： 摧毁对手的武器。
// Battlecry: Destroy your opponent's weapon.
[CardSim("EX1_066")]
internal static class Sim_EX1_066
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyEnemyWeaponEffect()]
    };
}
