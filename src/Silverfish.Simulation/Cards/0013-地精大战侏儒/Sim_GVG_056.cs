namespace Silverfish.Simulation.Cards;

// 钢铁战蝎
// Iron Juggernaut
// 战吼，亡语：将一张“地雷” 牌洗入你对手的牌库。当抽到“地雷”时，便会受到10点伤害。
// Battlecry and Deathrattle: Shuffle a Mine into your opponent's deck. When drawn, explode for 10 damage!
[CardSim("GVG_056")]
internal static class Sim_GVG_056
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new CreateCardEffect("GVG_056t", CardPlacement.ShuffleIntoDeck, CardRecipient.Opponent)],
        Deathrattle = [new CreateCardEffect("GVG_056t", CardPlacement.ShuffleIntoDeck, CardRecipient.Opponent)]
    };
}
