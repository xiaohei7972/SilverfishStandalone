using System.Collections.Generic;
using System.IO;
using System.Text;
using Silverfish.Cli;
using Silverfish.Cli.Web;
using Silverfish.GameProbe;
using Xunit;

namespace Silverfish.Tests;

public sealed class GameProbeTests
{
    [Fact]
    public async Task CollectorCapturesAllEntitiesAndCustomTagsAndRedactsHiddenOpponentCards()
    {
        var fakeGame = new FakeGameState();
        var frame = GameStateProbeCollector.Capture(fakeGame, 7, out var fingerprint);
        Assert.NotNull(frame);
        Assert.NotEqual(0, fingerprint);
        Assert.Equal(7, frame!.Revision);

        var json = GameStateProbeCollector.SerializeJson(frame);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var input = new WebConsoleInput();
        var position = await input.LoadAsync(stream, "probe-live-match");

        Assert.Equal("probe-live-match", position.MatchId);
        Assert.Equal(7, position.Sequence);
        Assert.Equal("法师", position.Board.OurHero.ClassName);
        Assert.Single(position.Board.OurHand);
        Assert.Equal("CS2_029", position.Board.OurHand[0].CardId);

        var allEntities = Assert.IsType<ConsoleEntity[]>(position.Board.AllEntities);
        var ourFireball = Assert.Single(allEntities, e => e.Id == 11);
        Assert.False(ourFireball.IsRedacted);
        Assert.Contains(ourFireball.Tags, t => t.Id == 48 && t.Value == 4);
        Assert.Contains(ourFireball.Tags, t => t.Id == 9999 && t.Name == "TAG_9999" && t.Value == 42);
        Assert.Contains(ourFireball.CachedTagsForDormant, t => t.Id == 8888 && t.Name == "TAG_8888" && t.Value == 7);

        var enemyHandCard = Assert.Single(allEntities, e => e.Id == 21);
        Assert.True(enemyHandCard.IsRedacted);
        Assert.Null(enemyHandCard.CardId);
        Assert.Empty(enemyHandCard.Tags);
    }

    [Fact]
    public void CollectorSupportsBlizzardCustomMapNotImplementingIDictionary()
    {
        var fakeGame = new FakeGameState(useBlizzardMap: true);
        var frame = GameStateProbeCollector.Capture(fakeGame, 9, out var fingerprint);
        Assert.NotNull(frame);
        Assert.NotEqual(0, fingerprint);
        Assert.Equal(7, frame!.Entities.Count);
        Assert.Contains(frame.Entities, e => e.Id == 11 && e.Tags.Any(t => t.Id == 9999 && t.Value == 42));
    }

    private sealed class FakeTagMap
    {
        private readonly Dictionary<int, int> m_values = new();
        public FakeTagMap(params (int Key, int Value)[] pairs)
        {
            foreach (var (k, v) in pairs)
                m_values[k] = v;
        }
        public Dictionary<int, int> GetMap() => m_values;
    }

    private class FakeEntity
    {
        private readonly int _id;
        private readonly string _cardId;
        private readonly int _controller;
        private readonly int _zone;
        private readonly int _zonePos;
        private readonly int _cardType;
        private readonly FakeTagMap m_tags;
        private readonly FakeTagMap m_cachedTagsForDormant;

        public FakeEntity(int id, string cardId, int controller, int zone, int zonePos, int cardType, FakeTagMap tags, FakeTagMap? dormant = null)
        {
            _id = id;
            _cardId = cardId;
            _controller = controller;
            _zone = zone;
            _zonePos = zonePos;
            _cardType = cardType;
            m_tags = tags;
            m_cachedTagsForDormant = dormant ?? new FakeTagMap();
        }

        public int GetEntityId() => _id;
        public string GetCardId() => _cardId;
        public int GetControllerId() => _controller;
        public int GetZone() => _zone;
        public int GetZonePosition() => _zonePos;
        public int GetCardType() => _cardType;
        public FakeTagMap GetTags() => m_tags;
        public FakeTagMap CachedTagsForDormant => m_cachedTagsForDormant;
    }

    private sealed class FakePlayer : FakeEntity
    {
        private readonly int _playerId;
        private readonly FakeEntity _hero;

        public FakePlayer(int entityId, int playerId, FakeEntity hero, FakeTagMap tags)
            : base(entityId, "", playerId, 1, 0, 2, tags)
        {
            _playerId = playerId;
            _hero = hero;
        }

        public int GetPlayerId() => _playerId;
        public FakeEntity GetHero() => _hero;
    }

    private sealed class FakeBlizzardMap<TKey, TValue> : System.Collections.IEnumerable where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _inner;
        public FakeBlizzardMap(Dictionary<TKey, TValue> inner) => _inner = inner;
        public IEnumerable<TValue> Values => _inner.Values;
        public System.Collections.IEnumerator GetEnumerator() => _inner.GetEnumerator();
    }

    private sealed class FakeGameState
    {
        private readonly FakePlayer _friendly;
        private readonly FakePlayer _opposing;
        private readonly Dictionary<int, object> _entities = new();
        private readonly bool _useBlizzardMap;

        public FakeGameState(bool useBlizzardMap = false)
        {
            _useBlizzardMap = useBlizzardMap;
            var gameEntity = new FakeEntity(1, "", 0, 1, 0, 1, new FakeTagMap((202, 1), (19, 10)));
            var ourHero = new FakeEntity(4, "HERO_08", 1, 1, 0, 3, new FakeTagMap((202, 3), (45, 30), (199, 4), (50, 1), (49, 1)));
            var enemyHero = new FakeEntity(5, "HERO_01", 2, 1, 0, 3, new FakeTagMap((202, 3), (45, 30), (199, 10), (50, 2), (49, 1)));
            _friendly = new FakePlayer(2, 1, ourHero, new FakeTagMap((202, 2), (23, 1), (26, 6), (50, 1), (49, 1)));
            _opposing = new FakePlayer(3, 2, enemyHero, new FakeTagMap((202, 2), (26, 5), (50, 2), (49, 1)));
            var ourHand = new FakeEntity(
                11,
                "CS2_029",
                1,
                3,
                1,
                5,
                new FakeTagMap((202, 5), (48, 4), (50, 1), (49, 3), (263, 1), (9999, 42)),
                new FakeTagMap((8888, 7)));
            var enemyHand = new FakeEntity(21, "EX1_001", 2, 3, 1, 4, new FakeTagMap((202, 4), (48, 2), (50, 2), (49, 3), (9999, 99)));

            foreach (var entity in new FakeEntity[] { gameEntity, _friendly, _opposing, ourHero, enemyHero, ourHand, enemyHand })
                _entities[entity.GetEntityId()] = entity;
        }

        public FakePlayer GetFriendlySidePlayer() => _friendly;
        public FakePlayer GetOpposingSidePlayer() => _opposing;
        public object GetEntityMap() => _useBlizzardMap ? new FakeBlizzardMap<int, object>(_entities) : _entities;
    }
}
