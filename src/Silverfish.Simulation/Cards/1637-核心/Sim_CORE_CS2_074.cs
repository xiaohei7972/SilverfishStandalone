using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 致命药膏
// Deadly Poison
// 使你的武器获得+2攻击力。
// Give your weapon +2_Attack.
internal static class Sim_CORE_CS2_074
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_WEAPON_EQUIPPED)],
        []);
    // </legacy-play-requirements>

}
