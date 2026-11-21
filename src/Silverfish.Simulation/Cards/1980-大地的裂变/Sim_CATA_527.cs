using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奈瑟匹拉，蒙难古灵
// Nespirah, Enthralled
// 造成1点伤害。在你施放一个邪能法术后，重新开启。亡语：召唤奈瑟匹拉，脱困古灵。
// Deal 1 damage. After you cast a Fel spell, reopen. Deathrattle: Summon Nespirah, Unshackled.
internal static class Sim_CATA_527
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY)]);
    // </legacy-play-requirements>

}
