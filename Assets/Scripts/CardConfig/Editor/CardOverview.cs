using System.Linq;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities;
using Sirenix.OdinInspector;

[GlobalConfig("Assets/Resources/ScriptableObject/EditorOverviews")]
public class CardOverview : GlobalConfig<CardOverview>
{
    [ReadOnly]
    [ListDrawerSettings(Expanded = true)]
    public CardInfo[] AllInfos;

    [Button(ButtonSizes.Medium), PropertyOrder(-1)]
    public void UpdateOverview()
    {
        // Finds and assigns all scriptable objects of type Character
        this.AllInfos = AssetDatabase.FindAssets("t:CardInfo")
            .Select(guid => AssetDatabase.LoadAssetAtPath<CardInfo>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToArray();
    }
}
