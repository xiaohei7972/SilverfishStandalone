using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 扰咒术
// Spellbender
// 奥秘：当一个敌方法术以一个随从为目标时，召唤一个1/3的随从并使其成为新的目标。
// Secret: When an enemy casts a spell on a minion, summon a 1/3 as the new target.
[CardSim("tt_010")]
internal static class Sim_tt_010
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.SpellTargetWindow, TriggerRelation.Opponent, [new RedirectSpellTargetEffect("tt_010a")])
            {
                EntityKind = EntityKind.Minion,
                RequiresBoardSpace = true
            }
        ]
    };
}
