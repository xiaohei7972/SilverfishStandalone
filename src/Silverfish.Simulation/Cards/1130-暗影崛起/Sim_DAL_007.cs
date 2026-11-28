using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 拉法姆的阴谋
// Rafaam's Scheme
// 召唤@个1/1的小鬼。（每回合都会升级！）
// Summon @ 1/1 |4(Imp, Imps). (Upgrades each turn!)
internal static class Sim_DAL_007
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
