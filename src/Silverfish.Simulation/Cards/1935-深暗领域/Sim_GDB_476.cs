using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 难以喘息
// Suffocate
// 消灭一个随从。如果你正在构筑 星舰，还会消灭一个随机相邻随从。
// Destroy a minion. If you're building a Starship, also destroy a random neighbor.
internal static class Sim_GDB_476
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
