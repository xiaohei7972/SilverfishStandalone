namespace Silverfish.Simulation.Cards;

// 法力过剩
// Excess Mana
// 抽一张牌。（你最多可以拥有 十个法力水晶。）
// Draw a card. (You can only have 10 Mana in your tray.)
[CardSim("CS2_013t")]
internal static class Sim_CS2_013t
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DrawEffect(1)]
    };
}
