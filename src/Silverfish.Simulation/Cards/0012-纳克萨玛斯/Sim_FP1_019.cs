namespace Silverfish.Simulation.Cards;

// 剧毒之种
// Poison Seeds
// 消灭所有随从，并召唤等量的2/2树人代替他们。
// Destroy all minions and summon 2/2 Treants to replace them.
[CardSim("FP1_019")]
internal static class Sim_FP1_019
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DestroyAllMinionsEffect(ReplaceWithCardId: "FP1_019t")
        ]
    };
}
