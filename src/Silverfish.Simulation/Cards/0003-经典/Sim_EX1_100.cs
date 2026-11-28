namespace Silverfish.Simulation.Cards;

// 游学者周卓
// Lorewalker Cho
// 每当一个玩家施放一个法术，复制该法术，将其置入另一个玩家的手牌。
// Whenever a player casts a spell, put a copy into the other player’s hand.
[CardSim("EX1_100")]
internal static class Sim_EX1_100
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Any, [new CopyTriggerSpellToOtherPlayerEffect()])]
    };
}
