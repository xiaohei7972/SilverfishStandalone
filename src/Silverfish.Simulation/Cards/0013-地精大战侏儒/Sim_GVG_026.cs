namespace Silverfish.Simulation.Cards;

// 假死
// Feign Death
// 触发所有友方随从的亡语。
// Trigger all Deathrattles on your minions.
[CardSim("GVG_026")]
internal static class Sim_GVG_026
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new TriggerFriendlyDeathrattlesEffect()]
    };
}
