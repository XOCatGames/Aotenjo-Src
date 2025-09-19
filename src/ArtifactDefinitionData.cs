using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aotenjo
{
    [CreateAssetMenu(menuName = "遗物定义数据", fileName = "new_artifact_definition")]
    public class ArtifactDefinitionData: ScriptableObject
    {
        [HorizontalGroup("基础"), PreviewField(100), HideLabel] 
        [ShowInInspector]
        public Sprite icon => Resources.LoadAll<Sprite>("Artifacts")[Artifacts.ARTIFACT_SPRITE_ID_MAP[Artifacts.GetArtifact(regName)]];
        
        [VerticalGroup("基础/信息")] [LabelText("ID")] public string regName;

        [VerticalGroup("基础/信息")] [LabelText("稀有度"), EnumToggleButtons] public Rarity baseRarity;

        [HorizontalGroup("归属信息")]
        
        [VerticalGroup("归属信息/套牌")] [LabelText("所在套牌")]
        public List<string> deckIn;

        [VerticalGroup("归属信息/套牌")] [LabelText("套牌黑名单")]
        public List<string> deckBlocked;
        
        [VerticalGroup("归属信息/牌体集")] [LabelText("所在牌体集")]
        public List<string> setIn;

        [VerticalGroup("归属信息/牌体集")] [LabelText("牌体集黑名单")]
        public List<string> setBlocked;

        [VerticalGroup("归属信息/牌体集")] [LabelText("所需牌体")]
        public List<string> materialRequired;

        [HorizontalGroup("标签信息")] [LabelText("遗物标签")] public List<ArtifactTag> tags;
        
        public void SyncToArtifact(Artifact artifact)
        {
            if (artifact == null) return;

            if (artifact.GetNameID() != regName)
            {
                Debug.LogError("WRONG ARTIFACT NAME ID: " + artifact.GetNameID() + " != " + regName);
                return;
            }
            
            artifact.rarity = this.baseRarity;
            artifact.deckIn = this.deckIn;
            artifact.deckBlocked = this.deckBlocked;
            artifact.setIn = this.setIn;
            artifact.setBlocked = this.setBlocked;
            artifact.materialRequired = this.materialRequired;
            artifact.tags = this.tags;
        }
    }
}