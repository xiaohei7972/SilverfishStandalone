namespace Silverfish.Simulation.Cards;

// 妙手空空
// Pilfered Power
// 每控制一个友方随从，便获得一个空的法力水晶。
// Gain an empty Mana Crystal for each friendly minion.
[CardSim("CFM_616")]
internal static class Sim_CFM_616
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new GainManaPerFriendlyMinionEffect()],
    };
}
