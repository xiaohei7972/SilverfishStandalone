using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迅疾挥剑
// Swift Slash
// 使你的武器获得+2攻击力和“你的英雄在攻击时免疫。”
// Give your weapon +2 Attack and "Your hero is Immune while attacking."
internal static class Sim_TTN_092t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_WEAPON_EQUIPPED)],
        []);
    // </legacy-play-requirements>

}
