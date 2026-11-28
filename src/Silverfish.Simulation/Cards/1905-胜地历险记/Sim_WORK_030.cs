using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰冠堡垒宣传单
// Icecrown Brochure
// 对一个随从造成$3点伤害并冻结 其相邻随从。 （每回合翻面。）
// Deal $3 damage to a minion and Freeze its neighbors. (Flips each turn.)
internal static class Sim_WORK_030
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
