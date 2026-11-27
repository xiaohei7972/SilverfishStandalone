namespace Silverfish.Simulation.Cards;

// 奥术傀儡
// Arcane Golem
// 冲锋，战吼：使你的对手获得一个法力水晶。
// Charge. Battlecry: Give your opponent a Mana Crystal.
[CardSim("EX1_089")]
internal static class Sim_EX1_089
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainFilled, 1, ManaRecipient.Opponent)]
    };
}
