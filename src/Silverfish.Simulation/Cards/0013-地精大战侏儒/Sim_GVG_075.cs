using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 船载火炮
// Ship's Cannon
// 在你召唤一个海盗后，随机对一个敌人造成2点伤害。
// After you summon a Pirate, deal 2 damage to a random enemy.
[CardSim("GVG_075")]
internal static class Sim_GVG_075
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.AfterMinionSummoned,
                TriggerRelation.Friendly,
                [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 2, 1)])
            {
                CardFilter = new CardFilter { MinionType = MinionType.Pirate }
            }
        ]
    };
}
