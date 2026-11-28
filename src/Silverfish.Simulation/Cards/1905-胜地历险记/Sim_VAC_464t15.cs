using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 侏儒军刀
// Gnomish Army Knife
// 使一个随从获得突袭，风怒，圣盾，吸血，剧毒，嘲讽以及潜行。
// Give a minion Rush, Windfury, Divine Shield, Lifesteal, Poisonous, Taunt, and Stealth.
internal static class Sim_VAC_464t15
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
