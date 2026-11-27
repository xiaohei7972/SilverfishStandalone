namespace Silverfish.Simulation.Cards;

// 法力怨魂
// Mana Wraith
// 所有随从的法力值消耗增加（1）点。
// ALL minions cost (1) more.
[CardSim("EX1_616")]
internal static class Sim_EX1_616
{
    internal static CardRules Rules { get; } = new()
    {
        CostAuras = [new(CostAuraRecipient.BothPlayers, CostAuraTarget.Minion, 1)]
    };
}
