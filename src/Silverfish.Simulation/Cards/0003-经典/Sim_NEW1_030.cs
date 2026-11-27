namespace Silverfish.Simulation.Cards;

// 死亡之翼
// Deathwing
// 战吼： 消灭所有其他随从，并弃掉你的手牌。
// Battlecry: Destroy all other minions and discard your_hand.
[CardSim("NEW1_030")]
internal static class Sim_NEW1_030
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyAllMinionsEffect(ExcludeSource: true), new DiscardHandEffect()]
    };
}
