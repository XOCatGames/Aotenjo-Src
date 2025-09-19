using System;

namespace Aotenjo
{
    [Serializable]
    public class PlayerProperties
    {
        /// <summary>
        /// 默认玩家属性
        /// </summary>
        public static PlayerProperties DEFAULT => new(15, 10, 6, 6, 5);

        /// <summary>
        /// 每回合出牌上限
        /// </summary>
        public int HandLimit;

        /// <summary>
        /// 每次出牌前弃牌上限
        /// </summary>
        public int DiscardLimit;


        /// <summary>
        /// 每次出牌后补充弃牌
        /// </summary>
        public int DiscardRefill = 5;

        /// <summary>
        /// 持有遗物上限
        /// </summary>
        public int ArtifactLimit;

        /// <summary>
        /// 持有小道具上限
        /// </summary>
        public int GadgetLimit;

        public PlayerProperties(int handLimit, int discardLimit, int artifactLimit, int gadgetLimit, int discardRefill)
        {
            HandLimit = handLimit;
            DiscardLimit = discardLimit;
            ArtifactLimit = artifactLimit;
            GadgetLimit = gadgetLimit;
            DiscardRefill = discardRefill;
        }
    }
}