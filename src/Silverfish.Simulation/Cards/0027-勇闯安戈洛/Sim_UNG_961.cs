using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 适者生存
// Adaptation
// 进化一个友方随从。
// Adapt a friendly minion.
[CardSim("UNG_961")]
internal static class Sim_UNG_961
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new AdaptChoice(AdaptChoice.StandardOptions, AdaptRecipient.Selected)
    };
}
