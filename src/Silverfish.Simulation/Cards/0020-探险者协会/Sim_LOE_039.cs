using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// A3型机械金刚
// Gorillabot A-3
// 战吼：如果你控制着其他机械，&发现一张机械牌。
// Battlecry: If you control another Mech,& Discover a Mech.
[CardSim("LOE_039")]
internal static class Sim_LOE_039
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { Kind = EntityKind.Minion, MinionType = MinionType.Mech }) { Condition = new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Play, NumberComparison.AtLeast, 1, EntityKind.Minion) { CardFilter = new CardFilter { MinionType = MinionType.Mech }, ExcludeSource = true } },
    };
}
