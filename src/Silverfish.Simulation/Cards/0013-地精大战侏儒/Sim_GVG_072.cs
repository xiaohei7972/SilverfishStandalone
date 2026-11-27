using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 暗影打击装甲
// Shadowboxer
// 每当一个随从获得治疗，便随机对一个敌人造成1点伤害。
// Whenever a minion is healed, deal 1 damage to a random enemy.
[CardSim("GVG_072")]
internal static class Sim_GVG_072
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.Healed,
                TriggerRelation.Any,
                [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, 1)])
            {
                EntityKind = EntityKind.Minion
            }
        ]
    };
}
