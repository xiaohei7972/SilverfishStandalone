using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 云雾王子
// Cloud Prince
// 战吼： 如果你控制一个奥秘，则造成6点伤害。
// Battlecry: If you control a Secret, deal 6 damage.
internal static class Sim_ULD_293
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS, 1), new(ErrorType.REQ_TARGET_IF_AVAILABLE, 1), new(ErrorType.REQ_ENEMY_TARGET, 1), new(ErrorType.REQ_NONSELF_TARGET, 1)],
        []);
    // </legacy-play-requirements>

}
