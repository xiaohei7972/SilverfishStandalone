namespace Silverfish.Simulation.Cards;

// 幸运币
// The Coin
// 在本回合中，获得一个 法力水晶。
// Gain 1 Mana Crystal this turn only.
[CardSim("GAME_005")]
internal static class Sim_GAME_005
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainTemporary, 1)]
    };
}
