using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迪菲亚麻风侏儒
// Defias Leper
// 战吼：如果你的手牌中有暗影法术牌，则造成2点伤害。
// Battlecry: If you're holding a Shadow spell, deal 2 damage.
internal static class Sim_DED_513
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
