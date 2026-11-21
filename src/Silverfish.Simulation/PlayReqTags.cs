using Silverfish.Domain.Client;

namespace Silverfish.Simulation;

/// <summary>Only the printed tags used by play requirements are packed into the built-in card table.</summary>
public static class PlayReqTags
{
    private static readonly int[] TagIds = [(int)GAME_TAG.IMP, (int)GAME_TAG.MARK_OF_EVIL, 2831, 3881,
        (int)GAME_TAG.STARSHIP_PIECE, (int)GAME_TAG.STARSHIP, (int)GAME_TAG.PROTOSS, 3444,
        (int)GAME_TAG.STEALTH, (int)GAME_TAG.TITAN, (int)GAME_TAG.RARITY, (int)GAME_TAG.DEATHRATTLE,
        (int)GAME_TAG.ELUSIVE, (int)GAME_TAG.IMMUNE, (int)GAME_TAG.CANT_BE_TARGETED_BY_OPPONENTS,
        (int)GAME_TAG.CANT_BE_TARGETED_BY_SPELLS, (int)GAME_TAG.CANT_BE_TARGETED_BY_HERO_POWERS,
        (int)GAME_TAG.POISONOUS];

    internal static bool Has(int mask, int tag)
    {
        var index = Array.IndexOf(TagIds, tag);
        return index >= 0 && (mask & (1 << index)) != 0;
    }

    public static int FromMetadata(CardMetadata metadata)
    {
        int mask = 0;
        for (int i = 0; i < TagIds.Length; i++)
        {
            var value = metadata.GetTag(TagIds[i]);
            if (TagIds[i] == (int)GAME_TAG.RARITY ? value == 5 : value > 0)
                mask |= 1 << i;
        }
        return mask;
    }
}
