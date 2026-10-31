using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public sealed class HistoryContent
    {
        public string id;
        public string owner;
        public bool sourceKnown;

        public bool Available(string kind)
        {
            // 内置自定义牌体集以空字符串作为注册名；仅历史展示认可该约定，缺字段和模组来源仍保持缺失。
            if (kind == "set" && id == "" && string.IsNullOrEmpty(owner)) return true;
            return ModContentRegistry.IsAvailable(kind, id, sourceKnown ? owner ?? "" : null);
        }
        public static HistoryContent Capture(string kind, string id) => new HistoryContent
        {
            id = id, owner = ModContentRegistry.OwnerOf(kind, id), sourceKnown = ModContentRegistry.Initialized
        };
    }

    [Serializable]
    public sealed class HistorySprite
    {
        public string key;
        public string sheet;
        public string spriteName;
        public int index = -1;
        public string owner;
        public bool Available
        {
            get
            {
                try { return Resolve() != null; }
                catch (Exception) { return false; }
            }
        }

        public static HistorySprite Capture(Sprite sprite, string sheet)
        {
            if (sprite == null) return null;
            var manager = SpriteManager.Instance;
            if (manager != null && manager.singleSpriteMap != null)
                foreach (string key in manager.singleSpriteMap.GetKeys())
                    if (manager.singleSpriteMap.Get(key) == sprite)
                    {
                        string modOwner = (ModManager.Instance?.LoadedMods ?? Array.Empty<Mod>())
                            .Where(m => key.Contains(":" + m.modID + ":")).OrderByDescending(m => m.modID.Length).FirstOrDefault()?.modID;
                        return new HistorySprite { key = key, owner = modOwner ?? TexturePackManager.GetSpriteOwner(sprite) };
                    }
            return new HistorySprite { sheet = sheet, spriteName = sprite.name,
                index = Array.IndexOf(SpriteManager.GetSpritesOrLoad(sheet), sprite), owner = TexturePackManager.GetSpriteOwner(sprite) };
        }

        public void CaptureOwner()
        {
            var sprite = Resolve();
            if (string.IsNullOrEmpty(owner)) owner = TexturePackManager.GetSpriteOwner(sprite);
            if (sprite != null && !string.IsNullOrEmpty(sheet)) spriteName = sprite.name;
        }

        public Sprite Resolve()
        {
            if (!ModContentRegistry.IsModLoaded(owner)) return null;
            if (!string.IsNullOrEmpty(key))
            {
                if (TexturePackManager.TryGetSpriteOverride(key, out var replacement)) return replacement;
                var map = SpriteManager.Instance?.singleSpriteMap;
                if (map != null && map.Contains(key)) return map.Get(key);
                // 本体独立遗物图仅在首次展示时入缓存，重启后直接查看历史也需要主动加载。
                // 只恢复已注册的本体资源，模组图片仍须由原提供者恢复，避免掩盖真正的缺失内容。
                const string nativeArtifactPrefix = "artifact:aotenjo:";
                if (string.IsNullOrEmpty(owner) && key.StartsWith(nativeArtifactPrefix, StringComparison.Ordinal))
                {
                    string id = key.Substring(nativeArtifactPrefix.Length);
                    if (ModContentRegistry.IsAvailable("artifact", id, ""))
                    {
                        // 皮纳塔和堂鼓已改用统一图集，旧战绩仍可能保存独立图片的键。
                        // 直接解析新图，避免旧图进入共享缓存后覆盖下一局的遗物外观；不改写历史快照。
                        if (id == Artifacts.Pinata.Id || id == Artifacts.Tanggu.Id)
                            return SpriteManager.GetSpritesOrLoad("Artifacts")
                                .ElementAtOrDefault(Artifacts.ARTIFACT_SPRITE_ID_MAP[id]);
                        return SpriteManager.GetSingleSpriteOrLoad(key, "Textures/Artifact/" + id);
                    }
                }
                return null;
            }
            if (string.IsNullOrEmpty(sheet)) return null;
            var sprites = SpriteManager.GetSpritesOrLoad(sheet);
            return !string.IsNullOrEmpty(spriteName) ? sprites.FirstOrDefault(s => s != null && s.name == spriteName) :
                index < 0 ? null : sprites.ElementAtOrDefault(index);
        }
    }

    [Serializable]
    public sealed class HistoryArtifact
    {
        public HistoryContent content;
        public HistorySprite image;
        public string nameKey;
    }

    [Serializable]
    public sealed class HistoryTile
    {
        public string typeName;
        public bool TypeAvailable
        {
            get
            {
                if (string.IsNullOrEmpty(typeName)) return true;
                try { return Type.GetType(typeName, false) != null; }
                catch (Exception) { return false; }
            }
        }
        public HistoryContent material;
        public int category;
        public int order;
        public bool faceKnown;
        public string font;
        public HistorySprite body;
        public HistorySprite face;
        public HistorySprite mask;
        public HistorySprite ornament;
        public List<HistorySprite> parts = new List<HistorySprite>();
        public List<bool> temporaryParts = new List<bool>();

        public static int FaceIndex(Tile.Category category, int order, string deck)
        {
            if (category >= Tile.Category.SiJi) return (4 + category - Tile.Category.SiJi) * 9 + order - 1;
            if (category == Tile.Category.Jian && order == 5 && deck == MahjongDeck.BambooDeck.regName) order = 8;
            int row = category == Tile.Category.Wan ? 0 : category == Tile.Category.Suo ? 1 : category == Tile.Category.Bing ? 2 : 3;
            return row * 9 + order - 1;
        }

        public static HistoryTile Capture(Tile tile, Player player)
        {
            var snapshot = new HistoryTile { material = new HistoryContent() };
            if (tile == null) return snapshot;
            snapshot.typeName = tile.GetType().AssemblyQualifiedName;
            snapshot.category = (int)tile.GetCategory();
            snapshot.order = tile.GetOrder();
            snapshot.faceKnown = true;
            snapshot.font = tile.properties?.font?.GetRegName();
            snapshot.material = HistoryContent.Capture("material", tile.properties?.material?.GetRegName());
            snapshot.face = new HistorySprite { sheet = Constants.FILE_TILE_FRONT_SHEET,
                index = FaceIndex(tile.GetCategory(), tile.GetOrder(), player?.deck?.regName) };
            var stats = ProfileManager.GetCurrentPlayerProfile()?.profileStats;
            if (tile.GetCategory() == Tile.Category.Suo && tile.GetOrder() == 1 && player?.deck != null &&
                stats != null && stats.GetWonNumberByDeck(player.deck.regName) > 0)
                snapshot.face = new HistorySprite { sheet = Constants.FILE_BIRD_SHEET,
                    index = player.deck.id + (stats.GetWonNumberByDeck(player.deck.regName, 8) > 0 ? 16 : 0) };
            try
            {
                var material = tile.properties?.material;
                if (material != null)
                {
                    if (material is TileMaterialMechPart mech)
                    {
                        snapshot.body = new HistorySprite { sheet = "TileMaterials", index = TileMaterialMechPart.InstalledTileSpriteId };
                        var parts = mech.GetParts();
                        for (int i = 0; i < parts.Count; i++)
                        {
                            snapshot.parts.Add(HistorySprite.Capture(TileMaterialMechPart.GetPartSprite(parts[i], true), "Tile/TileMaterial/MechParts"));
                            snapshot.temporaryParts.Add(mech.IsPartTemporary(i));
                        }
                    }
                    else if (material.GetRegName() == TileMaterial.PLAIN.GetRegName() && player?.deck != null)
                        snapshot.body = new HistorySprite { sheet = "TileMaterials", index = player.deck.GetBasicMaterialSpriteID() };
                    else snapshot.body = HistorySprite.Capture(material.GetSprite(player), "TileMaterials");
                    snapshot.ornament = new HistorySprite { sheet = "TileMaterials", index = material.GetOrnamentSpriteID(player) };
                }
                snapshot.mask = new HistorySprite { sheet = "TileMask", index = tile.properties?.mask?.GetSpriteID(player) ?? -1 };
                foreach (var image in new[] { snapshot.body, snapshot.face, snapshot.mask, snapshot.ornament }) image?.CaptureOwner();
            }
            catch (Exception error) { Debug.LogWarning("Could not capture historical tile appearance: " + error.Message); }
            return snapshot;
        }
    }

    [Serializable]
    public sealed class RunHistorySnapshot
    {
        public int formatVersion = 1;
        public RunEnvironment environment;
        public HistoryContent deck;
        public HistoryContent materialSet;
        public List<HistoryArtifact> artifacts = new List<HistoryArtifact>();
        public List<HistoryTile> tiles = new List<HistoryTile>();
        public List<HistoryContent> yakus = new List<HistoryContent>();

        public static RunHistorySnapshot Capture(Player player)
        {
            var snapshot = new RunHistorySnapshot
            {
                environment = player.runEnvironment ?? RunEnvironment.Legacy(),
                deck = HistoryContent.Capture("deck", player.deck?.regName),
                materialSet = HistoryContent.Capture("set", player.materialSet?.regName)
            };
            foreach (var artifact in player.GetArtifacts())
            {
                var item = new HistoryArtifact { content = HistoryContent.Capture("artifact", artifact.GetRegName()), nameKey = artifact.GetNameKey() };
                try
                {
                    item.image = HistorySprite.Capture(artifact.GetSprite(player), "Artifacts");
                }
                catch (Exception error) { Debug.LogWarning("Could not capture historical artifact appearance: " + error.Message); }
                snapshot.artifacts.Add(item);
            }
            var hand = player.stats?.bestSettleRecord;
            foreach (var tile in hand?.AllTiles ?? new List<Tile>()) snapshot.tiles.Add(HistoryTile.Capture(tile, player));
            foreach (var yaku in hand?.ActivatedYakuTypes ?? new List<YakuType>())
                snapshot.yakus.Add(HistoryContent.Capture("yaku", yaku.ToString()));
            return snapshot;
        }

        public bool HasMissingContent()
        {
            if (formatVersion != 1) return true;
            return deck == null || !deck.Available("deck") || materialSet == null || !materialSet.Available("set") ||
                (artifacts ?? new List<HistoryArtifact>()).Any(a => a?.content == null || !a.content.Available("artifact") || a.image?.Available != true) ||
                (tiles ?? new List<HistoryTile>()).Any(t => t?.material == null || !t.TypeAvailable || !t.material.Available("material") || t.body?.Available != true) ||
                (yakus ?? new List<HistoryContent>()).Any(y => y == null || !y.Available("yaku"));
        }
    }
}
