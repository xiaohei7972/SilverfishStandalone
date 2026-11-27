namespace Silverfish.Simulation.Cards;

// 林地树妖
// Grove Tender
// 抉择：使每个玩家获得一个法力水晶；或每个玩家抽一张牌。
// Choose One - Give each player a Mana Crystal; or Each player draws a card.
[CardSim("GVG_032")]
internal static class Sim_GVG_032
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("GVG_032a", [new ChangeManaEffect(ManaOperation.GainFilled, 1, ManaRecipient.BothPlayers)]),
            new("GVG_032b", [new DrawEffect(1, CardRecipient.Controller), new DrawEffect(1, CardRecipient.Opponent)])
        ]
    };
}
