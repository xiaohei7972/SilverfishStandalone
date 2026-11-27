namespace Silverfish.Simulation.Cards;

// 斯塔拉格
// Stalagg
// 亡语：如果费尔根也在本局对战中死亡，召唤塔迪乌斯。
// Deathrattle: If Feugen also died this game, summon Thaddius.
[CardSim("FP1_014")]
internal static class Sim_FP1_014
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new ConditionalEffect(
                new MinionDiedThisGameCondition("FP1_015"),
                [new SummonEffect("FP1_014t")])
        ]
    };
}
