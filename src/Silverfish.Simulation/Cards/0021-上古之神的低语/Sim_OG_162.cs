using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 克苏恩的信徒
// Disciple of C'Thun
// 战吼： 造成2点伤害。使你的克苏恩获得+2/+2（无论它在哪里）。
// Battlecry: Deal 2 damage. Give your C'Thun +2/+2 (wherever it is).
internal static class Sim_OG_162
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
