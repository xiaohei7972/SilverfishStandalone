using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 南海畸变船长
// Southsea Squidface
// 亡语：使你的武器获得+2攻击力。
// Deathrattle: Give your weapon +2 Attack.
[CardSim("OG_267")]
internal static class Sim_OG_267
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            Sim_OG_267e.Rule.ApplyToWeapon(),
        ],
    };
}
