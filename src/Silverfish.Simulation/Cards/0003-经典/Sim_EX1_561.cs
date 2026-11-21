using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 阿莱克丝塔萨
// Alexstrasza
// 战吼： 将一方英雄的剩余生命值变为15。
// Battlecry: Set a hero's remaining Health to 15.
internal static class Sim_EX1_561
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_HERO_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
