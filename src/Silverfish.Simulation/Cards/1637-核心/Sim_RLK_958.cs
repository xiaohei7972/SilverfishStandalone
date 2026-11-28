using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 骷髅帮手
// Skeletal Sidekick
// 战吼：使一个友方亡灵获得+2攻击力。
// Battlecry: Give a friendly Undead +2 Attack.
internal static class Sim_RLK_958
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.UNDEAD), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
