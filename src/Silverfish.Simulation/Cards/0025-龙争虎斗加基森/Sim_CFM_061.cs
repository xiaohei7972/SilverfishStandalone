namespace Silverfish.Simulation.Cards;

// 锦鱼人水语者
// Jinyu Waterspeaker
// 战吼：恢复#6点生命值。过载：（1）
// Battlecry: Restore #6 Health. Overload: (1)
[CardSim("CFM_061")]
internal static class Sim_CFM_061
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new HealEffect(HealTarget.FriendlyHero, 6)],
    };
}
