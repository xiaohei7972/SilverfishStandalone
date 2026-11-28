using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 立体书
// Pop-Up Book
// 造成$2点伤害。召唤两只0/1并具有嘲讽的青蛙。
// Deal $2 damage. Summon two 0/1 Frogs with Taunt.
internal static class Sim_TOY_508
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
