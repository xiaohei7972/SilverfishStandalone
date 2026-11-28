using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 明光祭司
// Illuminator
// 如果在你的回合结束时，你控制一个奥秘，则为你的英雄恢复4点生命值。
// If you control a Secret at the end of your turn, restore 4 Health to your hero.
[CardSim("GVG_089")]
internal static class Sim_GVG_089
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnEnded,
                TriggerRelation.Friendly,
                [
                    new ConditionalEffect(
                        new ZoneCountCondition(
                            ConditionPlayer.Friendly,
                            GameZone.Secret,
                            NumberComparison.AtLeast,
                            1),
                        [new HealEffect(HealTarget.FriendlyHero, 4)])
                ])
        ]
    };
}
