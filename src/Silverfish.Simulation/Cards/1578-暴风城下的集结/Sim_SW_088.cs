using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恶魔来袭
// Demonic Assault
// 造成$3点伤害。召唤两个1/3并具有嘲讽的虚空行者。
// Deal $3 damage. Summon two 1/3 Voidwalkers with Taunt.
internal static class Sim_SW_088
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
