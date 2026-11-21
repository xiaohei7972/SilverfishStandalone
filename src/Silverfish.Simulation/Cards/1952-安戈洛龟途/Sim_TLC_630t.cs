using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 格里什毒刺虫
// Gorishi Stinger
// 造成$2点伤害。召唤一只2/1并具有突袭的异种虫幼体。
// Deal $2 damage. Summon a 2/1 Grub with Rush.
internal static class Sim_TLC_630t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
