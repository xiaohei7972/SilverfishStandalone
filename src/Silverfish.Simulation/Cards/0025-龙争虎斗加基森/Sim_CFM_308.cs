namespace Silverfish.Simulation.Cards;

// 遗忘之王库恩
// Kun the Forgotten King
// 抉择：获得10点护甲值；或者复原你的法力水晶。
// Choose One - Gain 10 Armor; or Refresh your Mana Crystals.
[CardSim("CFM_308")]
internal static class Sim_CFM_308
{
    internal static CardRules Rules { get; } = new()
    {
        Choices = [new("armor", [new GainArmorEffect(10)]), new("refresh", [new ChangeManaEffect(ManaOperation.Refresh, int.MaxValue)])]
    };
}
