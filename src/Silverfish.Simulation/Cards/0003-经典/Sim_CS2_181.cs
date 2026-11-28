namespace Silverfish.Simulation.Cards;

// 负伤剑圣
// Injured Blademaster
// 战吼：对自身造成4点伤害。
// Battlecry: Deal 4 damage to HIMSELF.
[CardSim("CS2_181")]
internal static class Sim_CS2_181
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Source, 4)]
    };
}
