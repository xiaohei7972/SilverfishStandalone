using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 星际狐人
// Star Vulpera
// 可交易 战吼：消灭一个敌方星舰或星舰组件。
// Tradeable _Battlecry: Destroy an enemy_ Starship or Starship Piece.
internal static class Sim_GDB_340
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MUST_HAVE_TAG, GAME_TAG.STARSHIP_PIECE), new(ErrorType.REQ_TARGET_MUST_HAVE_TAG, GAME_TAG.STARSHIP)],
        []);
    // </legacy-play-requirements>

}
