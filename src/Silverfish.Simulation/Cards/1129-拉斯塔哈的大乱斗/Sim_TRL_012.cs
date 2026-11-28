using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 图腾重击
// Totemic Smash
// 造成$2点伤害。 超杀：召唤一个 基础图腾。
// Deal $2 damage. Overkill: Summon a basic Totem.
internal static class Sim_TRL_012
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
