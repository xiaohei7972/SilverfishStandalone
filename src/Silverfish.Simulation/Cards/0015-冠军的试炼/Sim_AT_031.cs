using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 窃贼
// Cutpurse
// 每当本随从攻击英雄时，将幸运币置入你的手牌。
// Whenever this minion attacks a hero, add the Coin to your hand.
[CardSim("AT_031")]
internal static class Sim_AT_031
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterAttack, TriggerRelation.Opponent, [new CreateCardEffect("GAME_005", Count: 1)]) { Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Hero, RequireSourceAttacker = true },
        ],
    };
}
