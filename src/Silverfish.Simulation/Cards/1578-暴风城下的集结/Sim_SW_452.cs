using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 混乱吸取
// Chaos Leech
// 吸血 对一个随从造成$3点伤害。流放：改为 造成$5点。
// Lifesteal. Deal $3 damage to a minion. Outcast: Deal $5 instead.
internal static class Sim_SW_452
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
