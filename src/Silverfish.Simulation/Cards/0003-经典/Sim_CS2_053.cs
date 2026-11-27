using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 视界术
// Far Sight
// 抽一张牌，该牌的法力值消耗减少（3）点。
// Draw a card. That card costs (3) less.
[CardSim("CS2_053")]
internal static class Sim_CS2_053
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_CS2_053e.Rule.ApplyWhenDrawn(1)]
    };
}
