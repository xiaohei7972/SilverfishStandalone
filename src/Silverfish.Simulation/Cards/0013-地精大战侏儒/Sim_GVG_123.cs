using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 煤烟喷吐装置
// Soot Spewer
// 法术伤害+1 战吼：如果你控制着其他机械，随机获取一张火焰法术牌。
// Spell Damage +1 Battlecry: If you control another Mech, get a random Fire spell.
[CardSim("GVG_123")]
internal static class Sim_GVG_123
{
    internal static CardRules Rules { get; } = new()
    {
        SpellDamage = 1,
        OnPlay =
        [
            new ConditionalEffect(
                new ZoneCountCondition(
                    ConditionPlayer.Friendly,
                    GameZone.Play,
                    NumberComparison.AtLeast,
                    2,
                    EntityKind.Minion)
                {
                    CardFilter = new CardFilter { MinionType = MinionType.Mech }
                },
                [new CreateRandomMatchingCardEffect(new CardFilter { Kind = EntityKind.Spell, SpellSchool = SpellSchool.Fire })])
        ]
    };
}
