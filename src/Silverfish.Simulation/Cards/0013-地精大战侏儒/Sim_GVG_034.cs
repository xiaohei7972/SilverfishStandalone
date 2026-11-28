namespace Silverfish.Simulation.Cards;

// 机械熊豹
// Mech-Bear-Cat
// 每当本随从受到伤害，将一张零件牌置入你的手牌。
// Whenever this minion takes damage, add a Spare Part card to your hand.
[CardSim("GVG_034")]
internal static class Sim_GVG_034
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.Damage, TriggerRelation.Friendly, [new CreateRandomCardEffect(SpareParts.All)])
            {
                EntityMatch = TriggerEntityMatch.Self
            }
        ]
    };
}
