namespace Silverfish.Simulation.Cards;

internal static class Sim_HERO_03bp
{
    internal const string CardId = "HERO_03bp";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EquipWeaponEffect("CS2_082")]
    };
}
