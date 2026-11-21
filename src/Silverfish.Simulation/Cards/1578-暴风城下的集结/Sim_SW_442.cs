using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虚空碎片
// Void Shard
// 吸血 造成$4点伤害。
// Lifesteal Deal $4 damage.
internal static class Sim_SW_442
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
