using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影打击
// Shadow Strike
// 对一个 未受伤的角色造成$5点伤害。
// Deal $5 damage to an undamaged character.
internal static class Sim_OG_176
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_UNDAMAGED_TARGET)],
        []);
    // </legacy-play-requirements>

}
