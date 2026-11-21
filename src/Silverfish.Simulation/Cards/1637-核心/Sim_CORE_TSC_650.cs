using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 划水好友
// Flipper Friends
// 抉择：召唤一只6/6并具有嘲讽的虎鲸；或者六只1/1并具有突袭的海獭。
// Choose One - Summon a 6/6 Orca with Taunt; or six 1/1 Otters with Rush.
internal static class Sim_CORE_TSC_650
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
