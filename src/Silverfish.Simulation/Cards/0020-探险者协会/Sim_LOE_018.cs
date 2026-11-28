using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 坑道穴居人
// Tunnel Trogg
// 每当你过载时，每一个被锁的法力水晶会使其获得+1攻击力。
// Whenever you Overload, gain +1 Attack per locked Mana Crystal.
[CardSim("LOE_018")]
internal static class Sim_LOE_018
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterCardPlayed, TriggerRelation.Friendly, [new EnchantByEventAmountEffect(StatTarget.Source, 1, Sim_LOE_018e.CardId)]) { OverloadOnly = true },
        ],
    };
}
