using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 捕熊陷阱
// Bear Trap
// 奥秘：在你的英雄受到攻击后，召唤一个3/3并具有嘲讽的灰熊。
// Secret: After your hero is attacked, summon a 3/3 Bear with Taunt.
[CardSim("AT_060")]
internal static class Sim_AT_060
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterAttack, TriggerRelation.Friendly, [new SummonEffect("CS2_125")]) { RequiresBoardSpace = true, Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Hero },
        ],
        IsSecret = true,
    };
}
