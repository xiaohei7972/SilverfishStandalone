using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 麻痹药膏
// Paralytic Poison
// 使你的武器获得+1攻击力和“你的英雄在攻击时免疫。”
// Give your weapon +1 Attack and "Your hero is Immune while attacking."
internal static class Sim_BAR_321
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_WEAPON_EQUIPPED)],
        []);
    // </legacy-play-requirements>

}
