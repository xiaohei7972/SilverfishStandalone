namespace Silverfish.Simulation.Cards;

// 科赞秘术师
// Kezan Mystic
// 战吼：随机夺取一个敌方奥秘的控制权。
// Battlecry: Take control of a random enemy Secret.
[CardSim("GVG_074")]
internal static class Sim_GVG_074
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new StealRandomEnemySecretEffect()]
    };
}
