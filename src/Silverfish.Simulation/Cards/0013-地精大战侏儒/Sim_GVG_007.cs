using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 烈焰巨兽
// Flame Leviathan
// 突袭。当你抽到该牌时，对所有除机械外的角色造成 2点伤害。
// Rush When you draw this, deal 2 damage to all characters except Mechs.
[CardSim("GVG_007")]
internal static class Sim_GVG_007
{
    internal static CardRules Rules { get; } = new()
    {
        WhenDrawn =
        [
            new DamageEffect(DamageTarget.AllCharacters, 2)
            {
                ExcludeMinionType = MinionType.Mech
            }
        ]
    };
}
