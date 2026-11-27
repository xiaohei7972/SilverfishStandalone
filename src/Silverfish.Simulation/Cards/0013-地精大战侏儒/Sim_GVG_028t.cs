namespace Silverfish.Simulation.Cards;

// 加里维克斯的幸运币
// Gallywix's Coin
// 在本回合中，获得一个法力水晶。（不会触发加里维克斯的效果。）
// Gain 1 Mana Crystal this turn only. (Won't trigger Gallywix.)
[CardSim("GVG_028t")]
internal static class Sim_GVG_028t
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainTemporary, 1)]
    };
}
