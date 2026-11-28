namespace Silverfish.Simulation.Cards;

// 青玉护符
// Jade Idol
// 抉择：召唤一个{0}的青玉魔像；或者将该牌的三张复制洗入你的牌库。@抉择：召唤一个青玉魔像；或者将该牌的三张复制洗入你的 牌库。
// Choose One - Summon a{1} {0} Jade Golem; or Shuffle 3 copies of this card into your deck.@Choose One - Summon a Jade Golem; or Shuffle 3 copies of this card into your deck.
[CardSim("CFM_602")]
internal static class Sim_CFM_602
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("CFM_602a", [new SummonJadeGolemEffect()]),
            new("CFM_602b", [new CreateCardEffect("CFM_602", Count: 3, Placement: CardPlacement.ShuffleIntoDeck)]),
        ],
    };
}
