namespace Silverfish.Simulation.Cards;

// 尤格-萨隆的仆从
// Servant of Yogg-Saron
// 战吼：随机施放一个法力值消耗大于或等于（5）点的法术（目标随机而定）。
// Battlecry: Cast a random _spell that costs (5) or MORE_ _(targets chosen randomly).
[CardSim("OG_087")]
internal static class Sim_OG_087
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CastRandomSpellEffect(MinimumPrintedCost: 5),
        ],
    };
}
