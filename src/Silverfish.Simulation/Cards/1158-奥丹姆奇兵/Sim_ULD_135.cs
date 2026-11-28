using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 隐秘绿洲
// Hidden Oasis
// 抉择：召唤一棵6/6并具有嘲讽的古树；或者恢复#12点生命值。
// Choose One - Summon a 6/6 Ancient with Taunt; or Restore #12 Health.
internal static class Sim_ULD_135
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
