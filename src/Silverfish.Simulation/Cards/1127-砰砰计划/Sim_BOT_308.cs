using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 弹簧火箭犬
// Spring Rocket
// 战吼：造成2点伤害。
// Battlecry: Deal 2 damage.
internal static class Sim_BOT_308
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
