using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 白骨投手
// Bone Flinger
// 战吼： 如果在你的上回合之后有友方亡灵死亡，造成2点伤害。
// Battlecry: If a friendly Undead died after your last turn, deal 2 damage.
internal static class Sim_RLK_123
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
