namespace Silverfish.Simulation.Cards;

// 深渊领主
// Pit Lord
// 战吼：对你的英雄造成5点伤害。
// Battlecry: Deal 5 damage to your hero.
[CardSim("EX1_313")]
internal static class Sim_EX1_313
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.FriendlyHero, 5)]
    };
}
