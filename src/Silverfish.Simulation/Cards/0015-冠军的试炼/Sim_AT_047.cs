using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 德莱尼图腾师
// Draenei Totemcarver
// 战吼：每有一个友方图腾，便获得+1/+1。
// Battlecry: Gain +1/+1 for each friendly Totem.
[CardSim("AT_047")]
internal static class Sim_AT_047
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ScaledEnchantStatsEffect(StatTarget.Source, StatScaleSource.FriendlyMinions, 1, 1, Sim_AT_047e.CardId) { CardFilter = new CardFilter { MinionType = MinionType.Totem } },
        ],
    };
}
