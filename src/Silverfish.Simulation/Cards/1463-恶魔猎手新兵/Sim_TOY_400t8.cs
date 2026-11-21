using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 眼棱
// Eye Beam
// 吸血。 对一个随从造成$3点伤害。流放：法力值消耗为（0）点。
// Lifesteal. Deal $3 damage to a minion. Outcast: This costs (0).
internal static class Sim_TOY_400t8
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
