using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 吸血药膏
// Leeching Poison
// 在本回合中，你的武器获得 吸血。
// Give your weapon Lifesteal this turn.
internal static class Sim_ICC_221
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_WEAPON_EQUIPPED)],
        []);
    // </legacy-play-requirements>

}
