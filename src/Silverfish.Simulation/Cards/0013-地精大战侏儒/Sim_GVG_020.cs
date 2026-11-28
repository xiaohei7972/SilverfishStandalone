namespace Silverfish.Simulation.Cards;

// 邪能火炮
// Fel Cannon
// 在你的回合结束时，对一个非机械随从造成2点伤害。
// At the end of your turn, deal 2 damage to a non-Mech minion.
[CardSim("GVG_020")]
internal static class Sim_GVG_020
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnEnded,
                TriggerRelation.Friendly,
                [new RandomDamageEffect(RandomCharacterPool.NonMechMinions, 2, 1)])
        ]
    };
}
