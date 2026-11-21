using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰川突进
// Glacial Advance
// 造成$4点伤害。在本回合中，你的下一个法术法力值消耗减少（2）点。
// Deal $4 damage. Your next spell this turn costs (2) less.
internal static class Sim_RLK_512
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
