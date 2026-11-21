using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冷血
// Cold Blood
// 使一个随从获得+2攻击力；连击：改为获得+4攻击力。
// Give a minion +2 Attack. Combo: +4 Attack instead.
internal static class Sim_CS2_073
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
