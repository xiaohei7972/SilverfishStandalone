using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 恐惧战马
// Dreadsteed
// 亡语： 在回合结束时，召唤一匹恐惧战马。
// Deathrattle: At the end of the turn, summon a Dreadsteed.
[CardSim("AT_019")]
internal static class Sim_AT_019
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonAtTurnEndEffect("AT_019", Sim_AT_019e.CardId),
        ],
    };
}
