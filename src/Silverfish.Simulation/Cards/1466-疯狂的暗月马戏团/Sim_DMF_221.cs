using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪吼冲击
// Felscream Blast
// 吸血 对一个随从和相邻的随从造成$1点伤害。
// Lifesteal. Deal $1 damage to a minion and its neighbors.
internal static class Sim_DMF_221
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
