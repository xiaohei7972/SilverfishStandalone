namespace Silverfish.Simulation.Cards;

// 哈里森·琼斯
// Harrison Jones
// 战吼：摧毁对手的武器，并抽数量等同于其耐久度的牌。
// Battlecry: Destroy your opponent's weapon and draw cards equal to its Durability.
[CardSim("EX1_558")]
internal static class Sim_EX1_558
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyEnemyWeaponEffect(DrawDurability: true)]
    };
}
