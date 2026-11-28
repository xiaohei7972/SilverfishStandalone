using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 虚灵勇士萨兰德
// Nexus-Champion Saraad
// 激励：随机将一张法术牌置入你的手牌。
// Inspire: Add a random spell to your hand.
[CardSim("AT_127")]
internal static class Sim_AT_127
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new CreateRandomMatchingCardEffect(new CardFilter { Kind = EntityKind.Spell })]),
        ],
    };
}
