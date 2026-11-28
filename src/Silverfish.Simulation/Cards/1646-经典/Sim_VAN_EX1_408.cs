using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 致死打击
// Mortal Strike
// 造成$4点伤害；如果你的生命值小于或等于12点，则改为造成$6点伤害。
// Deal $4 damage. If you have 12 or less Health, deal $6 instead.
internal static class Sim_VAN_EX1_408
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
