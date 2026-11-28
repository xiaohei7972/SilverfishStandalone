namespace Silverfish.Simulation.Cards;

// 瑟玛普拉格
// Mekgineer Thermaplugg
// 每当一个敌方随从死亡，召唤一个 麻风侏儒。
// Whenever an enemy minion dies, summon a Leper Gnome.
[CardSim("GVG_116")]
internal static class Sim_GVG_116
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.MinionDied, TriggerRelation.Opponent, [new SummonEffect("EX1_029")])
        ]
    };
}
