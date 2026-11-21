using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 末日灾祸
// Bane of Doom
// 对一个角色造成$3点伤害。如果该角色死亡，随机召唤一个恶魔。
// Deal $3 damage to a character. If it dies, summon a random Demon.
internal static class Sim_VAN_EX1_320
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
