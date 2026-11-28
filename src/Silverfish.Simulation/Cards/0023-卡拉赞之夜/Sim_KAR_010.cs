using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 夜魇骑士
// Nightbane Templar
// 战吼：如果你的手牌中有龙牌，便召唤两条1/1的雏龙。
// Battlecry: If you're holding a Dragon, summon two 1/1 Whelps.
[CardSim("KAR_010")]
internal static class Sim_KAR_010
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Hand, NumberComparison.AtLeast, 1) { CardFilter = new CardFilter { MinionType = MinionType.Dragon } },
            [new SummonEffect("KAR_010a", SummonPlacement.RightOfSource), new SummonEffect("KAR_010a", SummonPlacement.RightOfSource)]),
        ],
    };
}
