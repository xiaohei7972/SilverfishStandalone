using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 艾萨拉的祭仪
// Azsharan Ritual
// 沉默一个随从，并召唤一个它的复制。将一张沉没的祭仪置于你的牌库底。
// Silence a minion and summon a copy of it. Put a 'Sunken Ritual' on the bottom of your deck.
internal static class Sim_TSC_775
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
