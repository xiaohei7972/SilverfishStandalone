namespace Silverfish.Simulation.Cards;

// 星界沟通
// Astral Communion
// 获得十个法力水晶。弃掉 你的手牌。
// Gain 10 Mana Crystals. Discard your hand.
[CardSim("AT_043")]
internal static class Sim_AT_043
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ChangeManaEffect(ManaOperation.GainFilled, 10),
            new DiscardHandEffect(),
        ],
    };
}
