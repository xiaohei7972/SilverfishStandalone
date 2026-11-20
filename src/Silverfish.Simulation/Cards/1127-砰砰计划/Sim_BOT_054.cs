namespace Silverfish.Simulation.Cards;

// 生物计划
// Biology Project
// 每个玩家获得两个法力水晶。
// Each player gains 2_Mana Crystals.
[CardSim("BOT_054")]
internal static class Sim_BOT_054
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainFilled, 2, ManaRecipient.BothPlayers)]
    };
}
