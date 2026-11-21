using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 猎头
// Headhunt
// 造成$2点伤害。获取一张4/4并具有一项随机额外效果的乘务员。
// Deal $2 damage. Get a 4/4 Crewmate with a random Bonus Effect.
internal static class Sim_GDB_473
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
