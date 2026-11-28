using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 调皮的噬踝者
// Cheaty Anklebiter
// 吸血 战吼：造成1点伤害。
// Lifesteal Battlecry: Deal 1 damage.
internal static class Sim_TRL_512
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
