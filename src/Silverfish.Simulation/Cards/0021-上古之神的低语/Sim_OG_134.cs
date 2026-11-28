namespace Silverfish.Simulation.Cards;

// 尤格-萨隆
// Yogg-Saron, Hope's End
// 战吼： 在本局对战中，你每施放过一个法术，便随机施放一个法术（目标随机而定）。
// Battlecry: Cast a random spell for each spell you've cast this game (targets chosen randomly).
[CardSim("OG_134")]
internal static class Sim_OG_134
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CastRandomSpellEffect(ForEachSpellCast: true),
        ],
    };
}
