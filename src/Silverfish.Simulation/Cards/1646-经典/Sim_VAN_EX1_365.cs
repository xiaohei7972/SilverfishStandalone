using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣愤怒
// Holy Wrath
// 抽一张牌，并造成等同于其法力值消耗的伤害。
// Draw a card and deal_damage equal to_its Cost.
internal static class Sim_VAN_EX1_365
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
