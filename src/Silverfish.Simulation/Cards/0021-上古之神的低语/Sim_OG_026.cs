namespace Silverfish.Simulation.Cards;

// 永恒哨卫
// Eternal Sentinel
// 战吼：将你所有过载的法力水晶解锁。
// Battlecry: Unlock your Overloaded Mana Crystals.
[CardSim("OG_026")]
internal static class Sim_OG_026
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new UnlockOverloadEffect()]
    };
}
