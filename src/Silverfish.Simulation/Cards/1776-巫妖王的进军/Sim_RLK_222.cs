using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 阿斯塔洛·血誓
// Astalor Bloodsworn
// 战吼：将护卫阿斯塔洛置入你的手牌。法力渴求（@）：造成2点伤害。
// Battlecry: Add Astalor, the Protector to your hand. Manathirst (@): Deal 2 damage.
internal static class Sim_RLK_222
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
