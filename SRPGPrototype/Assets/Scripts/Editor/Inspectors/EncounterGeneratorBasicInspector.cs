using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EncounterGeneratorBasic))]
public class EncounterGeneratorBasicInspector : Editor
{
    public const int targetDifficultyGenerationIterations = 10000;
    public override void OnInspectorGUI()
    {
        var data = target as EncounterGeneratorBasic;
        if(GUILayout.Button(new GUIContent("Generate Target Difficulty")))
        {
            float sum = 0;
            for(int i = 0; i < targetDifficultyGenerationIterations; ++i)
            {
                if(data.seed != null)
                {
                    sum += data.seed.units.Where((u) => u.unit is IEncounterUnit)
                                          .Sum((u) => (u.unit as IEncounterUnit).EncounterData.challengeRating);
                }
                int numEnemies = RandomU.instance.Choice(data.numEnemies, data.numEnemiesWeights);
                for (int j = 0; j < numEnemies; ++j)
                    sum += RandomU.instance.Choice(data.enemies, data.baseEnemyWeights).EncounterData.challengeRating;
            }
            data.targetDifficulty = Mathf.Floor(sum / targetDifficultyGenerationIterations);
            EditorUtility.SetDirty(data);
        }
        base.OnInspectorGUI();
    }
}
