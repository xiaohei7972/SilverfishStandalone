using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 斜掠
// Rake
// 在本回合中，使你的英雄获得+2攻击力。对一个随从造成等同于你的英雄攻击力的伤害。
// Give your hero +2 Attack this turn. Deal damage equal to your hero's Attack to a minion.
internal static class Sim_NX2_012
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
