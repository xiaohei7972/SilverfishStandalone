namespace Silverfish.Simulation.Cards;

// 发条强盗机器人
// Wind-up Burglebot
// 每当本随从攻击随从并存活下来时，抽一张牌。
// Whenever this attacks a minion and survives, draw_a card.
[CardSim("CFM_025")]
internal static class Sim_CFM_025
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterAttack, TriggerRelation.Opponent, [new DrawEffect(1)])
            {
                Focus = TriggerFocus.AttackDefender,
                EntityKind = EntityKind.Minion,
                EntityMatch = TriggerEntityMatch.Any,
                RequireSourceAttacker = true,
                Condition = new CharacterStateCondition(ConditionCharacter.TriggerSubject, CharacterDisposition.Alive),
            },
        ],
    };
}
