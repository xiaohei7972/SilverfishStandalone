using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 统帅风范
// Commanding Presence
// 使你的武器获得“在你的英雄攻击后，召唤一个3/3并具有嘲讽的执行者。”
// Give your weapon "After your hero attacks, summon a 3/3 Enforcer with Taunt."
internal static class Sim_TTN_092t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_WEAPON_EQUIPPED)],
        []);
    // </legacy-play-requirements>

}
