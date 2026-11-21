using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影使徒
// Herald of Shadows
// 战吼：如果你在本牌在你手中时施放过暗影法术，从一个随从处偷取2点生命值。
// Battlecry: If you've cast a Shadow spell while holding this, steal 2 Health from a minion.
internal static class Sim_TID_717
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
