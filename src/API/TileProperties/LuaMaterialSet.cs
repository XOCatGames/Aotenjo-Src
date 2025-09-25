using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Aotenjo
{
    [Serializable]
    public class LuaMaterialSet : MaterialSet
    {
        
        public Action<Player, MaterialSet> OnPlayerSubscribe;
        public Action<Player, MaterialSet> OnPlayerUnsubscribe;
        public Func<MaterialSet, LotteryPool<TileMaterial>> OnGenerateCommonMaterialPool;
        public Func<MaterialSet, LotteryPool<TileMaterial>> OnGenerateRareMaterialPool;
        
        protected LuaMaterialSet(int id, string regName, List<string> availableMaterials, 
            Func<MaterialSet, LotteryPool<TileMaterial>> onGenerateRareMaterialPool, 
            Func<MaterialSet, LotteryPool<TileMaterial>> onGenerateCommonMaterialPool, 
            Action<Player, MaterialSet> onPlayerUnsubscribe, 
            Action<Player, MaterialSet> onPlayerSubscribe) : base(id, regName, availableMaterials)
        {
            OnGenerateRareMaterialPool = onGenerateRareMaterialPool;
            OnGenerateCommonMaterialPool = onGenerateCommonMaterialPool;
            OnPlayerUnsubscribe = onPlayerUnsubscribe;
            OnPlayerSubscribe = onPlayerSubscribe;
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            OnPlayerSubscribe?.Invoke(player, this);
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            OnPlayerUnsubscribe?.Invoke(player, this);
        }

        public override LotteryPool<TileMaterial> GenerateCommonMaterialPool()
        {
            
            return OnGenerateCommonMaterialPool?.Invoke(this) ?? base.GenerateCommonMaterialPool();
        }

        public override LotteryPool<TileMaterial> GenerateRareMaterialPool()
        {
            return OnGenerateRareMaterialPool?.Invoke(this) ?? base.GenerateRareMaterialPool();
        }

        public static LuaMaterialSet Create(int id, string regName, List<string> availableMaterials, 
            Func<MaterialSet, LotteryPool<TileMaterial>> onGenerateRareMaterialPool, 
            Func<MaterialSet, LotteryPool<TileMaterial>> onGenerateCommonMaterialPool, 
            Action<Player, MaterialSet> onPlayerUnsubscribe, Action<Player, MaterialSet> onPlayerSubscribe)
        {
            return new LuaMaterialSet(id, regName, availableMaterials, onGenerateRareMaterialPool, onGenerateCommonMaterialPool, onPlayerUnsubscribe, onPlayerSubscribe);
        }
    }
}