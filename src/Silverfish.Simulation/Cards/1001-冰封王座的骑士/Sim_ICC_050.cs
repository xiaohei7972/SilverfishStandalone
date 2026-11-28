using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 蛛网
// Webweave
// 召唤两只1/2并具有剧毒的 蜘蛛。
// Summon two 1/2 Poisonous Spiders.
internal static class Sim_ICC_050
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
