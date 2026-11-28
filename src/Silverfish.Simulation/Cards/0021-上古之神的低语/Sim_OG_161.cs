using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 腐化先知
// Corrupted Seer
// 战吼：对所有非鱼人随从造成2点伤害。
// Battlecry: Deal 2 damage to all non-Murloc minions.
[CardSim("OG_161")]
internal static class Sim_OG_161
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.AllMinions, 2) { ExcludeMinionType = MinionType.Murloc },
        ],
    };
}
