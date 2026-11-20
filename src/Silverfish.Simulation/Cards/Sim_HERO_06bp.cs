using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

internal static class Sim_HERO_06bp
{
    internal const string CardId = "HERO_06bp";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_CS2_017o.Rule.ApplyTo(StatTarget.FriendlyHero), new GainArmorEffect(1)]
    };
}
