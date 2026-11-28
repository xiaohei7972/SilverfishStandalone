using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 奥格瑞玛狼骑士
// Orgrimmar Aspirant
// 激励：使你的武器获得+1攻击力。
// Inspire: Give your weapon +1 Attack.
[CardSim("AT_066")]
internal static class Sim_AT_066
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [Sim_AT_066e.Rule.ApplyToWeapon()]),
        ],
    };
}
