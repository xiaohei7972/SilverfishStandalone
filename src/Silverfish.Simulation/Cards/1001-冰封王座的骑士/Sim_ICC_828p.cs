using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 合成僵尸兽
// Build-A-Beast
// 制造一个自定义的僵尸兽。
// Craft a custom Zombeast.
internal static class Sim_ICC_828p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_HAND_NOT_FULL)],
        []);
    // </legacy-play-requirements>

}
