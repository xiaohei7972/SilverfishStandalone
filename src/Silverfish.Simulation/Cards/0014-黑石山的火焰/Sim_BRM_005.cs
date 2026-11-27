using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 恶魔之怒
// Demonwrath
// 对所有非恶魔随从造成2点伤害。
// Deal 2 damage to all minions except Demons.
[CardSim("BRM_005")]
internal static class Sim_BRM_005
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.AllMinions, 2)
            {
                ExcludeMinionType = MinionType.Demon
            }
        ]
    };
}
