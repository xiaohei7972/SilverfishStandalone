using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 天神下凡形态
// Avatar Form
// 在本回合中，使一个友方角色获得+2攻击力和“在本角色攻击后，对所有敌人造成2点伤害”。
// Give a friendly character +2 Attack and "After this attacks, deal 2 damage to all enemies" this turn.
internal static class Sim_TIME_209t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
