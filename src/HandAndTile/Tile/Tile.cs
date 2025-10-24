using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class Tile : IComparable<Tile>
    {
        [SerializeField] protected Category category;
        [SerializeField] private int order;
        [SerializeReference] public TileProperties properties;
        [SerializeReference] private List<TileTransform> transforms;
        [SerializeField] public double addonFu;

        public Tile(Tile tile)
        {
            category = tile.category;
            order = tile.order;
            properties = new TileProperties(tile.properties);
            addonFu = tile.addonFu;
            transforms = tile.transforms == null ? new List<TileTransform>() : tile.transforms.Select(t => t.Copy()).ToList();
        }

        public Tile(Category category, int order)
        {
            this.category = category;
            this.order = order;
            properties = TileProperties.Plain();
            transforms = new List<TileTransform>();
        }

        public Tile(Category category, int order, TileProperties properties) : this(category, order)
        {
            this.properties = properties;
            transforms = new List<TileTransform>();
        }

        public virtual double GetBaseFu()
        {
            double point = CategoryIsNumbered(category)? Math.Abs(order - 5) + 5 + addonFu : 10 + addonFu;
            return point;
        }

        public Category GetCategory()
        {
            if (transforms.Count == 0)
            {
                return GetBaseCategory();
            }

            Tile rippedTile = new Tile(this);
            rippedTile.transforms.RemoveAt(rippedTile.transforms.Count - 1);
            return transforms[^1].GetTransformedCategory(rippedTile);
        }

        public int GetOrder()
        {
            if (transforms.Count == 0)
            {
                return GetBaseOrder();
            }

            Tile rippedTile = new Tile(this);
            rippedTile.transforms.RemoveAt(rippedTile.transforms.Count - 1);
            return transforms[^1].GetTransformedOrder(rippedTile);
        }

        public virtual bool IsYaoJiu(Player player)
        {
            return player.DetermineYaojiu(this);
        }

        public virtual bool IsHonor(Player player)
        {
            return player.DetermineHonor(this);
        }

        protected Tile ChangeProperties(TileProperties newProperties, Player player)
        {
            player.PreChangedProperties(this, newProperties);
            properties.UnsubcribeFromPlayer(player);
            properties = newProperties;
            newProperties.SubscribeToPlayer(player);
            
            return this;
        }

        public Tile SetMaterial(TileMaterial newMaterial, Player player, bool isCopy = false)
        {
            player.OnChangeMaterial(this, newMaterial, isCopy);
            ChangeProperties(properties.CopyWithMaterial(newMaterial), player);
            return this;
        }

        public Tile SetFont(TileFont newFont, Player player, bool isCopy = false)
        {
            player.OnChangeFont(this, newFont, isCopy);
            ChangeProperties(properties.CopyWithFont(newFont), player);
            return this;
        }

        public Tile SetMask(TileMask newMask, Player player, bool isCopy = false)
        {
            TileMask preMask = properties.mask;
            bool canceled = player.OnChangeMask(this, newMask, isCopy);
            if (canceled)
            {
                return this;
            }
            
            ChangeProperties(properties.CopyWithMask(newMask), player);
            
            if (newMask.GetRegName() != preMask.GetRegName())
            {
                MessageManager.Instance.OnChangeTileMask(this, newMask.GetRegName());
            }
            return this;
        }

        public Tile AddMask(TileMask newMask, Player player, bool isCopy = false)
        {
            //TODO: 支持多个负面效果共存
            return SetMask(newMask, player, isCopy);
        }

        public Tile SetProperties(TileProperties toBecome, Player player, bool isCopy = false)
        {
            player.OnchangeProperties(this, toBecome, isCopy);
            ChangeProperties(toBecome, player);
            return this;
        }

        public Tile(String representation) : this(0, 0)
        {
            char[] arr = representation.ToCharArray();
            order = arr[0] - '0';
            category = GetCategoryFromChar(arr[1], GetOrder());
        }

        public virtual void SubscribeToPlayerEvents(Player player)
        {
            properties.SubscribeToPlayer(player);
        }

        public virtual void UnsubscribeFromPlayer(Player player)
        {
            properties.UnsubcribeFromPlayer(player);
        }

        public override String ToString()
        {
            return GetOrder().ToString() +
                   GetCharFromCategory(GetCategory());
        }

        public virtual int CompareTo(Tile o)
        {
            if (o == null)
            {
                return 0;
            }

            int catDiff = CategoryToInteger(GetCategory()) - CategoryToInteger(o.GetCategory());
            if (catDiff != 0) return catDiff;
            if (GetOrder() - o.GetOrder() != 0) return GetOrder() - o.GetOrder();
            int baseOrderDiff = GetBaseOrder() - o.GetBaseOrder();
            return baseOrderDiff == 0 ? Comparer.Default.Compare(base.GetHashCode(), o.GetHashCode()) : baseOrderDiff;
        }

        public virtual bool CompatWith(Tile cand)
        {
            return IsSameCategory(cand) && IsSameOrder(cand);
        }

        public bool CompatWith(string representation)
        {
            return CompatWith(new Tile(representation));
        }

        public virtual bool IsSameCategory(Tile cand)
        {
            return cand.GetCategory() == GetCategory();
        }

        public virtual bool CompatWithCategory(Category cat)
        {
            return GetCategory() == cat;
        }

        public virtual bool IsSameOrder(Tile cand)
        {
            return cand.GetOrder() == GetOrder();
        }

        public virtual bool Succ(Tile a)
        {
            return CategoryIsNumbered(a.GetCategory()) && CategoryIsNumbered(GetCategory()) &&
                   a.GetOrder() == GetOrder() + 1 && a.GetCategory() == GetCategory();
        }

        public bool IsNumbered()
        {
            return CategoryIsNumbered(GetCategory());
        }

        public bool IsNumbered(int ord)
        {
            return IsNumbered() && GetOrder() == ord;
        }


        public enum Category
        {
            Wan = 0,
            Suo = 1,
            Bing = 2,
            Feng = 3,
            Jian = 4,
            SiJi = 5,
            JunZi = 6,
            SiYi = 7,
            SiYe = 8
        }

        public static bool CategoryIsNumbered(Category tileCategory)
        {
            return tileCategory == Category.Wan || tileCategory == Category.Suo || tileCategory == Category.Bing;
        }

        public static int CategoryToInteger(Category category)
        {
            return category switch
            {
                Category.Wan => 1,
                Category.Bing => 2,
                Category.Suo => 3,
                Category.Feng => 4,
                Category.Jian => 4,
                Category.SiJi => 5,
                Category.JunZi => 6,
                Category.SiYi => 7,
                Category.SiYe => 8,
                _ => 0,
            };
        }

        public static string CategoryToNameKey(Category category)
        {
            return category switch
            {
                Category.Wan => "ui_wan",
                Category.Bing => "ui_bing",
                Category.Suo => "ui_suo",
                Category.Feng => "ui_feng",
                Category.Jian => "ui_jian",
                Category.SiJi => "ui_siji",
                Category.JunZi => "ui_junzi",
                Category.SiYi => "ui_siyi",
                Category.SiYe => "ui_sizhi",
                _ => throw new ArgumentException("INVALID CATEGORY RECEIVED"),
            };
        }

        public static Category GetCategoryFromChar(char c, int ord)
        {
            return c switch
            {
                's' => Category.Suo,
                'm' => Category.Wan,
                'p' => Category.Bing,
                'z' => ord > 4 ? Category.Jian : Category.Feng,
                'f' => Category.SiJi,
                'g' => Category.JunZi,
                'h' => Category.SiYi,
                'i' => Category.SiYe,
                _ => throw new ArgumentException("INVALID CATEGORY RECEIVED: " + c),
            };
        }

        public static char GetCharFromCategory(Category category)
        {
            return category switch
            {
                Category.Suo => 's',
                Category.Wan => 'm',
                Category.Bing => 'p',
                Category.Feng => 'z',
                Category.Jian => 'z',
                Category.SiJi => 'f',
                Category.JunZi => 'g',
                Category.SiYi => 'h',
                Category.SiYe => 'i',
                _ => throw new ArgumentException(),
            };
        }

        public bool IsRotationalSymmetric()
        {
            return new Hand("1234589p245689s5z").tiles.Any(a =>
                a.GetCategory() == GetCategory() && a.GetOrder() == GetOrder());
        }

        public virtual Pair<Category, int> GetLastVisibleDisplay()
        {
            if (transforms.Count == 0)
            {
                return new(GetBaseCategory(), GetBaseOrder());
            }

            if (transforms[^1].ChangeBaseDisplay())
            {
                return new(GetCategory(), GetOrder());
            }

            Tile rippedTile = new Tile(this);
            rippedTile.transforms.RemoveAt(transforms.Count - 1);

            return rippedTile.GetLastVisibleDisplay();
        }

        public bool ContainsBlue(Player player)
        {
            if (transforms != null && transforms.Count > 0 &&
                (transforms[^1].GetNameKey() == new TileTransformMagnet().GetNameKey())) return true;
            return player.DetermineFontCompatibility(this, TileFont.BLUE);
        }

        public bool ContainsRed(Player player)
        {
            if (player.GetArtifacts().Contains(Artifacts.ThreeDGlasses) && Artifacts.ThreeDGlasses.IsActive(player))
            {
                return OriginallyContainsGreen(player);
            }

            return OriginallyContainsRed(player);
        }

        private bool OriginallyContainsRed(Player player)
        {
            Pair<Category, int> display = GetLastVisibleDisplay();
            Category visibleCat = display.elem1;
            int visibleOrder = display.elem2;

            if (transforms != null && transforms.Count > 0 &&
                (IsRedMarked() || transforms[^1].GetNameKey() == new TileTransformMagnet().GetNameKey())) return true;
            if (properties.font.GetRegName() == TileFont.COLORLESS.GetRegName()) return false;
            if (properties.font.GetRegName() == TileFont.BLUE.GetRegName()) return false;
            if (properties.font.GetRegName() == TileFont.RED.GetRegName()) return true;
            if (properties.font.GetRegName() == TileFont.Neon().GetRegName()) return true;
            return new Hand("135679p1579s123456789m7z1234f1234g1234h1234i").tiles.Any(a =>
                a.GetCategory() == visibleCat && a.GetOrder() == visibleOrder);
        }

        private bool OriginallyContainsGreen(Player player)
        {
            Pair<Category, int> display = GetLastVisibleDisplay();
            Category visibleCat = display.elem1;
            int visibleOrder = display.elem2;

            if (properties.font.GetRegName() == TileFont.COLORLESS.GetRegName()) return false;
            if (properties.font.GetRegName() == TileFont.Neon().GetRegName()) return true;
            if (properties.font.GetRegName() == TileFont.RED.GetRegName()) return false;
            if (properties.font.GetRegName() == TileFont.BLUE.GetRegName()) return false;
            return new Hand("123456789s6z123f1234g1234h1234i").tiles.Any(a =>
                a.GetCategory() == visibleCat && a.GetOrder() == visibleOrder);
        }

        private bool IsRedMarked()
        {
            if (transforms.Count == 0)
            {
                return false;
            }

            bool isRedNow = false;

            for (int i = 0; i < transforms.Count; i++)
            {
                if (transforms[i].GetNameKey() == new TileTransformRedMarker().GetNameKey())
                {
                    isRedNow = true;
                }

                if (transforms[i].ChangeBaseDisplay())
                {
                    isRedNow = false;
                }
            }

            return isRedNow;
        }

        public bool ContainsGreen(Player player)
        {
            if (player.GetArtifacts().Contains(Artifacts.ThreeDGlasses) && Artifacts.ThreeDGlasses.IsActive(player))
            {
                return OriginallyContainsRed(player);
            }

            return OriginallyContainsGreen(player);
        }

        public bool FulfillAllGreen(Player player)
        {
            if (new Hand("23468s6z").tiles.Any(a => a.GetCategory() == GetCategory() && a.GetOrder() == GetOrder()))
                return true;
            if (player.GetArtifacts().Contains(Artifacts.ThreeDGlasses) && Artifacts.ThreeDGlasses.IsActive(player))
            {
                if (properties.font.GetRegName() == TileFont.COLORLESS.GetRegName()) return false;
                if (transforms != null && transforms.Count > 0 &&
                    (transforms[^1].GetNameKey() == new TileTransformMagnet().GetNameKey())) return false;
                if (properties.font.GetRegName() == TileFont.RED.GetRegName()) return true;
                return new Hand("7z").tiles.Any(a => a.GetCategory() == GetCategory() && a.GetOrder() == GetOrder());
            }

            return false;
        }

        public bool ContainsNoColor(Player player)
        {
            if (ContainsGreen(player)) return false;
            if (ContainsRed(player)) return false;
            if (player.DetermineFontCompatibility(this, TileFont.BLUE)) return false;
            return true;
        }

        public Tile ModifyOrder(int v, Player player)
        {
            return ModifyCarvedDesign(GetBaseCategory(), v, player);
        }

        public Tile SetOrderForced(int v)
        {
            order = v;
            return this;
        }

        public Tile ModifyCategory(Category category, Player player)
        {
            return ModifyCarvedDesign(category, GetBaseOrder(), player);
        }

        public Tile SetCategoryForced(Category category)
        {
            this.category = category;
            return this;
        }

        public Tile ModifyCarvedDesign(Tile tile, Player player)
        {
            return ModifyCarvedDesign(tile.GetCategory(), tile.GetOrder(), player);
        }

        public Tile ModifyCarvedDesign(Category newCat, int newOrd, Player player)
        {
            bool preModifyRes = player.OnPreModifyCarvedDesign(this, category, newOrd);
            if (!preModifyRes) return this;
            order = newOrd;
            category = newCat;
            player.OnPostModifyCarvedDesign(this, category, newOrd);
            return this;
        }

        public int GetBaseOrder()
        {
            return order;
        }

        public string GetLocalizedName(Func<string, string> loc)
        {
            return loc($"tile_{ToString()}_name") + (IsModified() ? "??" : "");
        }

        public string GetSpriteString()
        {
            return $"<size=64px><sprite name=\"{ToString()}\"></size>";
        }

        public bool IsModified()
        {
            return GetBaseCategory() != GetCategory() || GetBaseOrder() != GetOrder();
        }

        public Category GetBaseCategory()
        {
            return category;
        }


        public void AddTransform(TileTransform tileTransform, Player player)
        {
            bool res = player.OnSetTransform(this, tileTransform);
            if (!res) return;
            transforms.Add(tileTransform);
        }

        public TileTransform GetLastTransform()
        {
            if (transforms.Count == 0)
            {
                return null;
            }

            return transforms[^1];
        }

        public void Suppress(Player player)
        {
            TileMask originalMask = properties.mask;
            SetMask(TileMask.Suppressed(originalMask), player);
        }

        internal void MergeAndSetProperties(TileProperties properties, Player player)
        {
            SetProperties(MergeProperties(properties), player);
        }
        
        internal TileProperties MergeProperties(TileProperties properties)
        {
            TileProperties newProp = TileProperties.Plain();
            if (properties.mask != TileMask.NONE)
            {
                newProp.mask = properties.mask;
            }
            else
            {
                newProp.mask = this.properties.mask;
            }

            if (properties.material != TileMaterial.PLAIN)
            {
                newProp.material = properties.material;
            }
            else
            {
                newProp.material = this.properties.material;
            }

            if (properties.font != TileFont.PLAIN)
            {
                newProp.font = properties.font;
            }
            else
            {
                newProp.font = this.properties.font;
            }

            return newProp;
        }

        public bool CompatWithMaterial(TileMaterial mat, Player player)
        {
            return player.DetermineMaterialCompatibility(this, mat);
        }

        public void AppendToListUnusedEffect(Player player, Permutation perm, List<Effect> effects)
        {
            properties.AppendUnusedEffects(player, perm, effects);
        }

        public void AppendToListOnTileUnusedEffect(Player player, Permutation perm, List<Effect> effects, Tile onTile)
        {
            properties.AppendToListOnTileUnusedEffect(player, perm, effects, this, onTile);
        }

        public void AddTransformForced(TileTransform transform)
        {
            transforms.Add(transform);
        }

        public void ClearTransform(Player player)
        {
            transforms.Clear();
        }

        public List<TileTransform> GetTransforms()
        {
            return new(transforms);
        }

        public virtual void AppendOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> onRoundEndEffects)
        {
            properties.AppendToListRoundEndEffect(player, permutation, onRoundEndEffects, this);
        }

        public void AppendDiscardEffects(Player player, Permutation permutation,
            List<IAnimationEffect> onDiscardTileEffects, bool withForce, Tile tile, bool isClone)
        {
            properties.AppendDiscardEffects(player, permutation, onDiscardTileEffects, tile, withForce, isClone);
        }

        public bool IsPlayerWind(Player player)
        {
            return GetCategory() == Category.Feng && player.IsPlayerWind(GetOrder());
        }

        public bool IsPrevalentWind(Player player)
        {
            return GetCategory() == Category.Feng && player.IsPrevalentWind(GetOrder());
        }
    }
}