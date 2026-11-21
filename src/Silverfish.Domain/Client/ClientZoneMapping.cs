namespace Silverfish.Domain.Client;

/// <summary>将客户端区域转换成独立模拟器使用的区域。</summary>
public static class ClientZoneMapping
{
    public static GameZone ToGameZone(TAG_ZONE zone, TAG_CARDTYPE cardType) => zone switch
    {
        TAG_ZONE.PLAY when cardType == TAG_CARDTYPE.HERO_POWER => GameZone.HeroPower,
        TAG_ZONE.PLAY => GameZone.Play,
        TAG_ZONE.DECK => GameZone.Deck,
        TAG_ZONE.HAND => GameZone.Hand,
        TAG_ZONE.GRAVEYARD => GameZone.Graveyard,
        TAG_ZONE.REMOVEDFROMGAME => GameZone.Removed,
        TAG_ZONE.SETASIDE => GameZone.SetAside,
        TAG_ZONE.SECRET => GameZone.Secret,
        _ => throw new ArgumentOutOfRangeException(nameof(zone), $"客户端区域「{zone}」无法映射到当前模拟器的区域。")
    };
}
