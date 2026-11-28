using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 艾萨拉的舰船
// Azsharan Vessel
// 召唤两个3/3并具有潜行的海盗。将一张沉没的舰船置于你的牌库底。
// Summon two 3/3 Pirates with Stealth. Put a 'Sunken Vessel' on the bottom of your deck.
internal static class Sim_TSC_912
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
