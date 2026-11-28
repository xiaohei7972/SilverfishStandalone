namespace Silverfish.Simulation.Cards;

// 受伤的克瓦迪尔
// Injured Kvaldir
// 战吼：对自身造成3点伤害。
// Battlecry: Deal 3 damage to this minion.
[CardSim("AT_105")]
internal static class Sim_AT_105
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.Source, 3),
        ],
    };
}
