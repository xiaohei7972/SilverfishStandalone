namespace Silverfish.Simulation.Cards;

// 地雷
// Burrowing Mine
// 抽到时施放 你受到10点伤害。
// Casts When Drawn You take 10 damage.
[CardSim("GVG_056t")]
internal static class Sim_GVG_056t
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.FriendlyHero, 10)]
    };
}
