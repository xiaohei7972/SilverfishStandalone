using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 强化机器人
// Enhance-o Mechano
// 战吼：随机使你的其他随从分别获得风怒，嘲讽，或者圣盾效果中的一种。
// Battlecry: Give your other minions Windfury, Taunt, or Divine Shield (at random).
[CardSim("GVG_107")]
internal static class Sim_GVG_107
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new GrantRandomKeywordToOtherFriendlyMinionsEffect(
                [GAME_TAG.WINDFURY, GAME_TAG.TAUNT, GAME_TAG.DIVINE_SHIELD])
        ]
    };
}
