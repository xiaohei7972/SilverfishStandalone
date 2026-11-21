using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 封冻沉眠
// Cryosleep
// 造成$4点伤害并抽一张牌。延系：再抽一张。
// Deal $4 damage and draw a card. Kindred: Draw another.
internal static class Sim_TLC_440
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
