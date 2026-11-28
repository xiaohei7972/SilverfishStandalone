using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 百变泽鲁斯
// Shifter Zerus
// 如果这张牌在你的手牌中，每个回合都会随机变成一张随从牌。
// Each turn this is in your hand, transform it into a random minion.
[CardSim("OG_123")]
internal static class Sim_OG_123
{
    internal static CardRules Rules { get; } = new()
    {
        InHandTriggers =
        [
            new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new TransformHandCardEffect(new CardFilter { Kind = EntityKind.Minion }, Sim_OG_123e.CardId)]),
        ],
    };
}
