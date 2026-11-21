using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 绵羊面具
// Sheep Mask
// 将一个随从的属性值变为1/1并使其获得“亡语：对所有随从造成2点伤害。”
// Set a minion's stats to 1/1 and give it "Deathrattle: Deal 2 damage to all minions."
internal static class Sim_DINO_429
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
