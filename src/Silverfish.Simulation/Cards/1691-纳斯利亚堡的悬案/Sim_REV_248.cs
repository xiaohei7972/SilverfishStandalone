using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 晋升者之赐
// Boon of the Ascended
// 使一个随从获得+2生命值。召唤一个具有该随从属性值和嘲讽的格里恩。
// Give a minion +2 Health. Summon a Kyrian with its stats and Taunt.
internal static class Sim_REV_248
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
