namespace Silverfish.Simulation.Cards;

// 恶魔卫士
// Felguard
// 嘲讽，战吼：摧毁你的一个法力水晶。
// Taunt Battlecry: Destroy one of your Mana Crystals.
[CardSim("EX1_301")]
internal static class Sim_EX1_301
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.Destroy, 1)]
    };
}
