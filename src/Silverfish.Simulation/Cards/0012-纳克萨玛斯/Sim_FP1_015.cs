namespace Silverfish.Simulation.Cards;

// 费尔根
// Feugen
// 亡语：如果斯塔拉格也在本局对战中死亡，召唤塔迪乌斯。
// Deathrattle: If Stalagg also died this game, summon Thaddius.
[CardSim("FP1_015")]
internal static class Sim_FP1_015
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new ConditionalEffect(
                new MinionDiedThisGameCondition("FP1_014"),
                [new SummonEffect("FP1_014t")])
        ]
    };
}
