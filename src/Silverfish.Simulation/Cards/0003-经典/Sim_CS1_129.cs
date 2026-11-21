using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 心灵之火
// Inner Fire
// 使一个随从的攻击力等同于其生命值。
// Change a minion's Attack to be equal to its Health.
internal static class Sim_CS1_129
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
