namespace Silverfish.Simulation.Cards;

// 虚空碾压者
// Void Crusher
// 激励：随机消灭每个玩家的一个随从。
// Inspire: Destroy a random minion for each player.
[CardSim("AT_023")]
internal static class Sim_AT_023
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new DestroyRandomEnemyMinionEffect(), new DestroyRandomFriendlyMinionEffect()]),
        ],
    };
}
