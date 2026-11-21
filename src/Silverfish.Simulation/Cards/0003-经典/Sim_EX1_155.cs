using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 自然印记
// Mark of Nature
// 抉择： 使一个随从获得+4攻击力；或者+4生命值和嘲讽。
// Choose One - Give a minion +4 Attack; or +4 Health and Taunt.
internal static class Sim_EX1_155
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
