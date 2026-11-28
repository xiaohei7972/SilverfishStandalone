using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 战斗怒火
// Battle Rage
// 每有一个受伤的友方角色，便抽一张牌。
// Draw a card for each damaged friendly character.
[CardSim("EX1_392")]
internal static class Sim_EX1_392
{
    internal static PlayReqSet PlayReqs { get; } = new([], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DynamicDrawEffect(DynamicDrawMode.DamagedFriendlyCharacters)]
    };
}
