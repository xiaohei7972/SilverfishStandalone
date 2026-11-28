using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灵魂剥离
// Soul Shear
// 对一个随从造成$3点伤害。将两张灵魂残片洗入你的 牌库。
// Deal $3 damage to a minion. Shuffle 2 Soul Fragments into your deck.
internal static class Sim_SCH_701
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
