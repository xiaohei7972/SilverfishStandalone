namespace Silverfish.Simulation.Cards;

// 瑞文戴尔男爵
// Baron Rivendare
// 你的随从的亡语将触发两次。
// Your minions trigger their Deathrattles twice.
[CardSim("FP1_031")]
internal static class Sim_FP1_031
{
    internal static CardRules Rules { get; } = new()
    {
        DoubleFriendlyDeathrattles = true
    };
}
