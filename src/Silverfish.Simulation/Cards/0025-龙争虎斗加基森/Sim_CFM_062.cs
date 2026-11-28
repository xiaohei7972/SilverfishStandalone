using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 污手街守护者
// Grimestreet Protector
// 嘲讽，战吼：使相邻的随从获得圣盾。
// Taunt Battlecry: Give adjacent _minions Divine Shield.
[CardSim("CFM_062")]
internal static class Sim_CFM_062
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EnchantStatsEffect(StatTarget.AdjacentFriendlyMinions, null) { GrantedTag = GAME_TAG.DIVINE_SHIELD }],
    };
}
