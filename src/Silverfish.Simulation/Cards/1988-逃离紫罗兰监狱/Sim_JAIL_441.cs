using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 饮血术
// Drink Blood
// 吸血。对一个随从造成$3点伤害。复原你的英雄技能。
// Lifesteal Deal $3 damage to a minion. Refresh your Hero Power.
internal static class Sim_JAIL_441
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
