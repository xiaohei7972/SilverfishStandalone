namespace Silverfish.Simulation.Cards;

// 鼓舞
// Invigorate
// 抉择：获得一个空的法力水晶；或者抽一张牌。
// Choose One - Gain an empty Mana Crystal; or Draw a card.
[CardSim("WON_014")]
internal static class Sim_WON_014
{
    internal static CardRules Rules { get; } = new()
    {
        Choices = [new("gainCrystal", [new ChangeManaEffect(ManaOperation.GainEmpty, 1)]), new("draw", [new DrawEffect(1)])]
    };
}
