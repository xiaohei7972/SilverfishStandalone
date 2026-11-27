namespace Silverfish.Simulation.Cards;

// 大法师安东尼达斯
// Archmage Antonidas
// 每当你施放一个法术，&将一张“火球术”法术牌置入你的手牌。
// Whenever you cast a spell,& add a 'Fireball' spell to_your hand.
[CardSim("EX1_559")]
internal static class Sim_EX1_559
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly, [new CreateCardEffect("CS2_029")])]
    };
}
