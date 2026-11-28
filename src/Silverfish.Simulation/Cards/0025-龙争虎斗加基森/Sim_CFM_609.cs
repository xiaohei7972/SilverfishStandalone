namespace Silverfish.Simulation.Cards;

// 邪兽人噬魂魔
// Fel Orc Soulfiend
// 在你的回合开始时，对本随从造成2点 伤害。
// At the start of your turn, deal 2 damage to this_minion.
[CardSim("CFM_609")]
internal static class Sim_CFM_609
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.Source, 2)])
            {
                ControllerTurnOnly = true,
            },
        ],
    };
}
