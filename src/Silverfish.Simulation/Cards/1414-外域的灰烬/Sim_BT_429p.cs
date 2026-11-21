using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恶魔冲击
// Demonic Blast
// 造成$5点伤害。（还可使用两次！）
// Deal $5 damage. (Two uses left!)
internal static class Sim_BT_429p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
