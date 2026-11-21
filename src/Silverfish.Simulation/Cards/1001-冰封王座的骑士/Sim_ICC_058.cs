using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冷冻鱼人
// Brrrloc
// 战吼： 冻结一个敌人。
// Battlecry: Freeze an_enemy.
internal static class Sim_ICC_058
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
