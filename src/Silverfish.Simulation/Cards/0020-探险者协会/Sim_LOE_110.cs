namespace Silverfish.Simulation.Cards;

// 远古暗影
// Ancient Shade
// 战吼：将一张“远古诅咒”牌洗入你的牌库。当你抽到该牌，便会受到7点伤害。
// Battlecry: Shuffle an 'Ancient Curse' into your deck that deals 7 damage to you when drawn.
[CardSim("LOE_110")]
internal static class Sim_LOE_110
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateCardEffect("LOE_110t", Count: 1, Placement: CardPlacement.ShuffleIntoDeck),
        ],
    };
}
