using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑石山宣传单
// Blackrock Brochure
// 对一个随从造成$3点伤害并对其相邻随从造成$1点。（每回合翻面。）
// Deal $3 damage to a minion and $1 to its neighbors. (Flips each turn.)
internal static class Sim_WORK_030t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
