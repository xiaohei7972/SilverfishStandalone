using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 硬壳横行蟹
// Crusty the Crustacean
// 战吼：消灭一个随从。获得其攻击力和生命值。
// Battlecry: Destroy a minion. Gain its Attack and Health.
internal static class Sim_VAC_464t8
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
