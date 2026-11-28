using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 血骑士
// Blood Knight
// 战吼：所有随从失去圣盾。每有一个随从失去圣盾，便获得+3/+3。
// Battlecry: All minions lose Divine Shield. Gain +3/+3 for each Shield lost.
[CardSim("EX1_590")]
internal static class Sim_EX1_590
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrainDivineShieldsBuffEffect(3, 3, Sim_EX1_590e.CardId)]
    };
}
