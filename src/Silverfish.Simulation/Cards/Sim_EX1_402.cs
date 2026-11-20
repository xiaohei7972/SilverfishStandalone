using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

[CardSim("EX1_402")]
internal static class Sim_EX1_402
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(1)])
        {
            EntityKind = EntityKind.Minion
        }]
    };
}
