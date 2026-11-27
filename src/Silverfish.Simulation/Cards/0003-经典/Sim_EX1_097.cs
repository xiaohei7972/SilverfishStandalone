namespace Silverfish.Simulation.Cards;

// 憎恶
// Abomination
// 嘲讽，亡语：对所有角色造成2点伤害。
// Taunt. Deathrattle: Deal 2 damage to ALL characters.
[CardSim("EX1_097")]
internal static class Sim_EX1_097
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new DamageEffect(DamageTarget.AllCharacters, 2)]
    };
}
