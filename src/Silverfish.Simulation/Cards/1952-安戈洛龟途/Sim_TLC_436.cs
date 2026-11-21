using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 重生的翼手龙
// Reanimated Pterrordax
// 突袭。吸血 消耗残骸而非 法力值。
// Rush, Lifesteal Costs Corpses instead of Mana.
internal static class Sim_TLC_436
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_CORPSES, 5)],
        []);
    // </legacy-play-requirements>

}
