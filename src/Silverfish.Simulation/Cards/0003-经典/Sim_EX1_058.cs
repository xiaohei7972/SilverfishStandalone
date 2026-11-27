using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 日怒保卫者
// Sunfury Protector
// 战吼：使相邻的随从获得嘲讽。
// Battlecry: Give adjacent minions Taunt.
[CardSim("EX1_058")]
internal static class Sim_EX1_058
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EnchantStatsEffect(StatTarget.AdjacentFriendlyMinions, null) { GrantedTag = GAME_TAG.TAUNT }]
    };
}
