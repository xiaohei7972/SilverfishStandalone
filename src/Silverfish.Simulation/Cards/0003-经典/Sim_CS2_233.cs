using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 剑刃乱舞
// Blade Flurry
// 摧毁你的武器，对所有敌方随从 造成等同于其攻击力的伤害。
// Destroy your weapon and deal its damage to all enemy minions.
internal static class Sim_CS2_233
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_WEAPON_EQUIPPED)],
        []);
    // </legacy-play-requirements>

}
