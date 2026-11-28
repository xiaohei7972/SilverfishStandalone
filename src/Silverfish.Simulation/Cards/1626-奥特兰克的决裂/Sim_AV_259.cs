using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰霜撕咬
// Frostbite
// 造成$3点伤害。荣誉消灭：你对手的下一个法术法力值消耗增加（2）点。
// Deal $3 damage. Honorable Kill: Your opponent's next spell costs (2) more.
internal static class Sim_AV_259
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
