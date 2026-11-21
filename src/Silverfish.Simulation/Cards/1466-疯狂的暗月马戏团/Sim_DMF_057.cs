using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 月蚀
// Lunar Eclipse
// 对一个随从造成$3点伤害。在本回合中，你施放的下一个法术的法力值消耗减少（2）点。
// Deal $3 damage to a minion. Your next spell this turn costs (2) less.
internal static class Sim_DMF_057
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
