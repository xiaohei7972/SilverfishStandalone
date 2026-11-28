namespace Silverfish.Simulation.Cards;

// 命令怒吼
// Commanding Shout
// 在本回合中，你的随从的生命值无法被降到1点以下。抽一张牌。
// Your minions can't be reduced below 1 Health this turn. Draw a card.
[CardSim("NEW1_036")]
internal static class Sim_NEW1_036
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ProtectFriendlyMinionsMinimumHealthEffect(), new DrawEffect(1)]
    };
}
