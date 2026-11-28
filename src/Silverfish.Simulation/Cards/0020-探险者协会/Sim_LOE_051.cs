namespace Silverfish.Simulation.Cards;

// 丛林枭兽
// Jungle Moonkin
// 双方玩家拥有 法术伤害+2。
// Both players have Spell Damage +2.
[CardSim("LOE_051")]
internal static class Sim_LOE_051
{
    internal static CardRules Rules { get; } = new()
    {
        BothPlayersSpellDamage = 2,
    };
}
