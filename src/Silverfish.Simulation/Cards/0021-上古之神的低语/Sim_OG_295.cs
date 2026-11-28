namespace Silverfish.Simulation.Cards;

// 异教药剂师
// Cult Apothecary
// 战吼：每有一个敌方随从，便为你的英雄恢复#2点生命值。
// Battlecry: For each enemy minion, restore #2 Health to your hero.
[CardSim("OG_295")]
internal static class Sim_OG_295
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new HealPerEnemyMinionEffect(2),
        ],
    };
}
