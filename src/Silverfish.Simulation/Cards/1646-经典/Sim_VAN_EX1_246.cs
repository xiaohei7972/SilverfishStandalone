using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 妖术
// Hex
// 使一个随从变形成为一只0/1并具有嘲讽的青蛙。
// Transform a minion into a 0/1 Frog with Taunt.
internal static class Sim_VAN_EX1_246
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
