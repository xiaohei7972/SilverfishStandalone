using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 琥珀女祭司
// Amber Priestess
// 嘲讽。战吼：为一个角色恢复等同于本随从生命值的生命值。
// Taunt Battlecry: Restore Health to a character equal to this minion's Health.
internal static class Sim_TIME_431
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
