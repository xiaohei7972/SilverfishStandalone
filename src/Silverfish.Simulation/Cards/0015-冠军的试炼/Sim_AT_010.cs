using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 暴躁的牧羊人
// Ram Wrangler
// 战吼：如果你控制任何野兽，则随机召唤一个野兽。
// Battlecry: If you have a Beast, summon a random Beast.
[CardSim("AT_010")]
internal static class Sim_AT_010
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Play, NumberComparison.AtLeast, 1, EntityKind.Minion) { CardFilter = new CardFilter { MinionType = MinionType.Beast }, ExcludeSource = true }, [new SummonMatchingMinionEffect(new CardFilter { Kind = EntityKind.Minion, MinionType = MinionType.Beast })]),
        ],
    };
}
