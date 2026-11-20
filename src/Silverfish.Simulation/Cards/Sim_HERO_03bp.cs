namespace Silverfish.Simulation.Cards;

[CardSim("HERO_03bp")]
internal static class Sim_HERO_03bp
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EquipWeaponEffect("CS2_082")]
    };
}
