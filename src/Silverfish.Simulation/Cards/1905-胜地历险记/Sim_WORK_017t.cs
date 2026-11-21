using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 吉尔尼斯宣传单
// Gilneas Brochure
// 沉默一个随从并使其获得-2/-2。（每回合翻面。）
// Silence a minion and give it -2/-2. (Flips each turn.)
internal static class Sim_WORK_017t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
