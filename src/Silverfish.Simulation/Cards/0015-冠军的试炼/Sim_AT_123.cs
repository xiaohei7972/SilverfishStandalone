using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 冰喉
// Chillmaw
// 嘲讽。亡语： 如果你的手牌中有龙牌，则对所有随从造成3点伤害。
// Taunt Deathrattle: If you're holding a Dragon, deal 3 damage to all minions.
[CardSim("AT_123")]
internal static class Sim_AT_123
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Hand, NumberComparison.AtLeast, 1, EntityKind.Minion) { CardFilter = new CardFilter { MinionType = MinionType.Dragon } }, [new DamageEffect(DamageTarget.AllMinions, 3)]),
        ],
    };
}
