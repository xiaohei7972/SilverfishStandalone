namespace Silverfish.Simulation.Cards;

// 末日预言者
// Doomsayer
// 在你的回合开始时，&消灭所有随从。
// At the start of your turn,& destroy ALL minions.
[CardSim("NEW1_021")]
internal static class Sim_NEW1_021
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new DestroyAllMinionsEffect()])]
    };
}
