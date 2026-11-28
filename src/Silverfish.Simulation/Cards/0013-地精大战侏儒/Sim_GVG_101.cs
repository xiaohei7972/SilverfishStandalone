namespace Silverfish.Simulation.Cards;

// 血色净化者
// Scarlet Purifier
// 战吼： 对所有具有亡语的随从造成2点伤害。
// Battlecry: Deal 2 damage to all minions with Deathrattle.
[CardSim("GVG_101")]
internal static class Sim_GVG_101
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.AllMinions, 2)
            {
                TargetCardFilter = new CardFilter { HasDeathrattle = true }
            }
        ]
    };
}
