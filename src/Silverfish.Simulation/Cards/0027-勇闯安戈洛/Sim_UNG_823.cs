using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 浸毒武器
// Envenom Weapon
// 使你的武器获得剧毒。
// Give your weapon Poisonous.
internal static class Sim_UNG_823
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_WEAPON_EQUIPPED)],
        []);
    // </legacy-play-requirements>

}
