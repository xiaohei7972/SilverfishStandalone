using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 回荡混响
// Reverberations
// 召唤一个随从的复制。复制和本体都会在受到伤害后死亡。
// Summon a copy of a minion. Each one dies after taking damage.
internal static class Sim_JAM_031
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
