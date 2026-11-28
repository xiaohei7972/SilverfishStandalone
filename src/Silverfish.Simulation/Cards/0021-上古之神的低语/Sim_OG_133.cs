namespace Silverfish.Simulation.Cards;

// 恩佐斯
// N'Zoth, the Corruptor
// 战吼：召唤所有你在本局对战中死亡的，并具有亡语的随从。
// Battlecry: Summon your Deathrattle minions that died this game.
[CardSim("OG_133")]
internal static class Sim_OG_133
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ResurrectMatchingMinionsEffect(new CardFilter { HasDeathrattle = true }),
        ],
    };
}
