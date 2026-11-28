using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 镜像
// Mirror Image
// 召唤两个0/2，并具有嘲讽的随从。
// Summon two 0/2 minions with Taunt.
internal static class Sim_CS2_027
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
