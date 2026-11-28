using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 玛克扎尔王子
// Prince Malchezaar
// 对战开始时：额外将五张传说随从牌置入你的牌库。
// Start of Game: Add 5 extra Legendary minions to your deck.
[CardSim("KAR_096")]
internal static class Sim_KAR_096
{
    internal static CardRules Rules { get; } = new()
    {
        StartOfGame = [new AddUniqueClassMinionsToDeckEffect(new CardFilter { Kind = EntityKind.Minion, IsLegendary = true }, 5)],
    };
}
