using System;
using System.Collections.Generic;

namespace Aotenjo
{
    public class MessageManager
    {
        private static MessageManager _instance;

        public static MessageManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MessageManager();
                }

                return _instance;
            }
        }


        public static void RefreshEventBus()
        {
            _instance = new MessageManager();
        }

        public event Action<PirateChestEvent> PirateChestOpeningEvent;
        public event Action<SoundEvent> SoundEvent;
        public event Action<Artifact, Effect> ActivateArtifactEvent;
        public event Action<MiniBrushEvent> UseMiniBrushEvent;
        public event Action<PlayerGadgetEvent> UseFishingRodEvent;
        public event Action<YakuType, int, int> UpgradeYakuEvent;
        public event Action<Player> UnfreezeEvent;
        public event Action<RookGadget, Tile> UseRookEvent;
        public event Action<List<int>> DrawTilesEvent;
        public event Action<int> OpenYakuPackEvent;
        public event Action<List<Tile>> AddTileEvent;
        public event Action<List<Tile>> RemoveTileEvent;
        public event Action<List<Tile>> EnterHandEvent;
        public event Action<Tile, SneakyPlayer> SneakTileEvent;
        public event Action<string> CompleteAchievementEvent;
        public event Action<Tile, bool> EnqueueDiscardEvent;
        public event Action<int> PlayerSpendMoneyEvent;
        public event Action<float> OnSetProgressBarEvent;
        public event Action<Tile, string> OnChangeTileMaskEvent;
        public event Action<int, Artifact> OnArtifactEarnMoneyEvent;

        public void OnAddTileEvent(List<Tile> tiles)
        {
            AddTileEvent?.Invoke(tiles);
        }


        public void OnRemoveTileEvent(List<Tile> tiles)
        {
            RemoveTileEvent?.Invoke(tiles);
        }

        public void OnOpenPirateChest(List<PirateChestReward> rewards, Player player)
        {
            PirateChestOpeningEvent?.Invoke(new PirateChestEvent(rewards, player));
        }

        public void OnSoundEvent(string soundName)
        {
            SoundEvent?.Invoke(new SoundEvent(soundName));
        }

        public void OnActivateArtifactEvent(Artifact artifact, Effect effect)
        {
            ActivateArtifactEvent?.Invoke(artifact, effect);
        }

        public void OnUseMiniBrushEvent(Gadget gadgetPtr, Tile targetTile)
        {
            UseMiniBrushEvent?.Invoke(new MiniBrushEvent(gadgetPtr, targetTile));
        }

        public void OnUpgradeYakuEvent(YakuType type, int levelBefore, int levelAfter)
        {
            UpgradeYakuEvent?.Invoke(type, levelBefore, levelAfter);
        }

        public void OnUnfreezeEvent(Player player)
        {
            UnfreezeEvent?.Invoke(player);
        }

        public void OnUseRookEvent(RookGadget rookGadget, Tile tile)
        {
            UseRookEvent?.Invoke(rookGadget, tile);
        }

        public void OnDrawTiles(List<int> posDrew)
        {
            DrawTilesEvent?.Invoke(posDrew);
        }

        public void OnOpenYakuPack(int v)
        {
            OpenYakuPackEvent?.Invoke(v);
        }

        public void OnTilesEnterHand(List<Tile> tiles)
        {
            EnterHandEvent?.Invoke(tiles);
        }

        public void OnSneakTile(Tile tile, SneakyPlayer player)
        {
            SneakTileEvent?.Invoke(tile, player);
        }

        public void OnCompleteAchievement(string id)
        {
            CompleteAchievementEvent?.Invoke(id);
        }

        public PlayerStats GetGlobalPlayerStats()
        {
            return ProfileManager.GetCurrentPlayerProfile().profileStats;
        }

        public void EnqueueToDiscard(Tile t, bool forced)
        {
            EnqueueDiscardEvent?.Invoke(t, forced);
        }

        public void OnSpendMoney(int v)
        {
            PlayerSpendMoneyEvent?.Invoke(v);
        }

        public void OnUseFishingRodEvent(PlayerGadgetEvent evt)
        {
            UseFishingRodEvent?.Invoke(evt);
        }

        public void OnSetProgressBarLength(float percentage)
        {
            OnSetProgressBarEvent?.Invoke(percentage);
        }
        
        public void OnChangeTileMask(Tile tile, string mask)
        {
            OnChangeTileMaskEvent?.Invoke(tile, mask);
        }

        public void OnArtifactEarnMoney(int money, Artifact artifact)
        {
            OnArtifactEarnMoneyEvent?.Invoke(money, artifact);
        }
    }
}