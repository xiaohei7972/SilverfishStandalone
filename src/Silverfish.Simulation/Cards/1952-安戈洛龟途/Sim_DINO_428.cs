using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 巨鳗面具
// Behemoth Mask
// 将一个随从的属性值变为8/10并使其获得吸血。随机迫使一个敌方随从攻击该随从。
// Set a minion's stats to 8/10 and give it Lifesteal. Force a random enemy minion to attack it.
internal static class Sim_DINO_428
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
