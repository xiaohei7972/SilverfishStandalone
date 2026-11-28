namespace Silverfish.Simulation.Cards;

// 精英对决
// Enter the Coliseum
// 除了每个玩家攻击力最高的随从之外，消灭所有 其他随从。
// Destroy all minions except each player's highest Attack minion.
[CardSim("AT_078")]
internal static class Sim_AT_078
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DestroyAllButOneMinionEffect(PreserveHighestAttackPerPlayer: true),
        ],
    };
}
