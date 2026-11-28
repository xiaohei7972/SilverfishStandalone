using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 鸟禽守护者
// Avian Watcher
// 战吼：如果你控制一个奥秘，便获得+1/+1和嘲讽。
// Battlecry: If you control a Secret, gain +1/+1 and Taunt.
[CardSim("KAR_037")]
internal static class Sim_KAR_037
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Secret, NumberComparison.AtLeast, 1),
                [Sim_KAR_037t.Rule.ApplyTo(StatTarget.Source)])],
    };
}
