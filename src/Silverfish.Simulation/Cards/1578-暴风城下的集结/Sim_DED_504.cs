using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪恶船运
// Wicked Shipment
// 可交易 召唤@个1/1的小鬼。（被交易时会升级，增加2个！）
// Tradeable Summon @ 1/1 |4(Imp, Imps). (Upgrades by 2 when Traded!)
internal static class Sim_DED_504
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
