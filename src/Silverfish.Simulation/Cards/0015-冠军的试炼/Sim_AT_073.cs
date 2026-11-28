using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 争强好胜
// Competitive Spirit
// 奥秘：在你的回合开始时，使你的所有随从获得+1/+1。
// Secret: When your turn starts, give your minions +1/+1.
[CardSim("AT_073")]
internal static class Sim_AT_073
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [Sim_AT_073e.Rule.ApplyTo(StatTarget.FriendlyMinions)]) { AllowOnControllerTurn = true, Condition = new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Play, NumberComparison.AtLeast, 1, EntityKind.Minion) },
        ],
        IsSecret = true,
    };
}
