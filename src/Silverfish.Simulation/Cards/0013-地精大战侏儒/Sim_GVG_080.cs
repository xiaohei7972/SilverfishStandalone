using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 毒牙德鲁伊
// Druid of the Fang
// 战吼：如果你控制任何野兽，将本随从变形成为7/7。
// Battlecry: If you have a Beast, transform this minion into a 7/7.
[CardSim("GVG_080")]
internal static class Sim_GVG_080
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(
                new ZoneCountCondition(
                    ConditionPlayer.Friendly,
                    GameZone.Play,
                    NumberComparison.AtLeast,
                    1,
                    EntityKind.Minion)
                {
                    CardFilter = new CardFilter { MinionType = MinionType.Beast }
                },
                [new TransformMinionEffect("GVG_080t", Self: true)])
        ]
    };
}
