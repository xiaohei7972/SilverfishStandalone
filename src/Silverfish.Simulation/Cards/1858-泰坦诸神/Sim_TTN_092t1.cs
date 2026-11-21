using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 守护秩序
// Maintain Order
// 使你的武器获得“在你的英雄攻击后，抽一张牌。”
// Give your weapon "After your hero attacks, draw a card."
internal static class Sim_TTN_092t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_WEAPON_EQUIPPED)],
        []);
    // </legacy-play-requirements>

}
