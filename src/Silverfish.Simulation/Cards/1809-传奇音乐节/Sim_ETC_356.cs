using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 变音和弦
// Altered Chord
// 吸血。对一个随从造成$6点伤害。如果你有过载的法力水晶，本牌的法力值消耗减少（3）点。
// Lifesteal Deal $6 damage to a minion. Costs (3) less if you're Overloaded.
internal static class Sim_ETC_356
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
