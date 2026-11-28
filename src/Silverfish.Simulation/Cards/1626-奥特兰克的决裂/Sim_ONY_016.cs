using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 憎恨之翼（等级1）
// Wings of Hate (Rank 1)
// 召唤两只1/1的邪翼蝠。（当你有5点法力值时升级。）
// Summon two 1/1 Felwings. (Upgrades when you have 5 Mana.)
internal static class Sim_ONY_016
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
