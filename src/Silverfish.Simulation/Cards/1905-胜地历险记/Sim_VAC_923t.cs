using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 圣沙泽尔
// Sanc'Azel
// 使一个友方随从获得+@攻击力和突袭。变回随从。
// Give a friendly minion +@ Attack and Rush. Turn back into a minion.
internal static class Sim_VAC_923t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
