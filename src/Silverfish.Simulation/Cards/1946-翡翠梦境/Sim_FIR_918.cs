using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 新月辉光
// Light of the New Moon
// 使一个随从获得+3/+3。（施放@个法术，即可在使用时将本牌移回你的手牌）
// Give a minion +3/+3. (Cast @ |4(spell, spells) to return this to your hand when played.)
internal static class Sim_FIR_918
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
