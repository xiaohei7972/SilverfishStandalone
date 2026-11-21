using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 燃灯元素
// Lamplighter
// 战吼：造成@点伤害（每有一个你使用过元素牌的连续的回合，伤害都会提升）。
// Battlecry: Deal @ damage (Improved by each turn in a row you've played an Elemental).
internal static class Sim_VAC_442
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
