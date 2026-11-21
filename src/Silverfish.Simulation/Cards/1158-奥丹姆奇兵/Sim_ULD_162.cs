using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 怪盗征募官
// EVIL Recruiter
// 战吼：消灭一个友方跟班，召唤一个5/5的恶魔。
// Battlecry: Destroy a friendly Lackey to summon a 5/5 Demon.
internal static class Sim_ULD_162
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MUST_HAVE_TAG, GAME_TAG.MARK_OF_EVIL)],
        []);
    // </legacy-play-requirements>

}
