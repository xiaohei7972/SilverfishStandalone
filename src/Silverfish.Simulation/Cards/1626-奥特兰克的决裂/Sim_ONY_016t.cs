using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 憎恨之翼（等级2）
// Wings of Hate (Rank 2)
// 召唤三只1/1的邪翼蝠。（当你有10点法力值时升级。）
// Summon three 1/1 Felwings. (Upgrades when you have 10 Mana.)
internal static class Sim_ONY_016t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
