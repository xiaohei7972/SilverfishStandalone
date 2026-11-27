namespace Silverfish.Simulation.Cards;

// 希尔瓦娜斯·风行者
// Sylvanas Windrunner
// 亡语：随机夺取一个敌方随从的控制权。
// Deathrattle: Take control of a random enemy minion.
[CardSim("EX1_016")]
internal static class Sim_EX1_016
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new TakeControlRandomEnemyMinionEffect()]
    };
}
