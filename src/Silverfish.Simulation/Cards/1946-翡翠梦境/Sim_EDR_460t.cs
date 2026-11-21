using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 满月祈愿
// Wish of the Full Moon
// 吸血 对一个随从造成$6点伤害。
// Lifesteal Deal $6 damage to a minion.
internal static class Sim_EDR_460t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
