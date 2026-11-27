namespace Silverfish.Simulation.Cards;

// 艾露恩的女祭司
// Priestess of Elune
// 战吼：为你的英雄恢复#4点生命值。
// Battlecry: Restore #4 Health to your hero.
[CardSim("EX1_583")]
internal static class Sim_EX1_583
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new HealEffect(HealTarget.FriendlyHero, 4)]
    };
}
