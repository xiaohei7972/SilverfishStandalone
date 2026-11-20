namespace Silverfish.Simulation.Cards;

// 滋养
// Nourish
// 抉择：获得两个法力水晶；或者抽三张牌。
// Choose One - Gain 2_Mana Crystals; or Draw 3 cards.
[CardSim("EX1_164")]
internal static class Sim_EX1_164
{
    internal static CardRules Rules { get; } = new()
    {
        Choices = [new("gainCrystals", [new ChangeManaEffect(ManaOperation.GainFilled, 2)]), new("draw", [new DrawEffect(3)])]
    };
}
