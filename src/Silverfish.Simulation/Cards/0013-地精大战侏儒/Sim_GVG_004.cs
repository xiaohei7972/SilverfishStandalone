using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 地精炎术师
// Goblin Blastmage
// 战吼：如果你控制任何机械，则造成6点伤害，随机分配到所有敌人身上。
// Battlecry: If you control a Mech, deal 6 damage randomly split among all enemies.
[CardSim("GVG_004")]
internal static class Sim_GVG_004
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
                    CardFilter = new CardFilter { MinionType = MinionType.Mech }
                },
                [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, 6)])
        ]
    };
}
