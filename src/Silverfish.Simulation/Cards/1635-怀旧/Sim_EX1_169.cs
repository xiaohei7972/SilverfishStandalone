namespace Silverfish.Simulation.Cards;

// 激活
// Innervate
// 在本回合中，获得一个 法力水晶。
// Gain 1 Mana Crystal this turn only.
[CardSim("EX1_169")]
internal static class Sim_EX1_169
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainTemporary, 1)]
    };
}
