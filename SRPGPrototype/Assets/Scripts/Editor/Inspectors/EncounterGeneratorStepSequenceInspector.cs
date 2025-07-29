using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EncounterGeneratorStepSequence))]
public class EncounterGeneratorStepSequenceInspector : Editor
{
    public const int targetDifficultyGenerationIterations = 10000;
    public override void OnInspectorGUI()
    {
        var data = target as EncounterGeneratorStepSequence;
        if(GUILayout.Button(new GUIContent("Generate Target Difficulty")))
        {
            float sum = 0;
            for(int i = 0; i < targetDifficultyGenerationIterations; ++i)
            {
                sum += data.Generate(string.Empty, 0).Difficulty;
            }
            data.targetDifficulty = Mathf.Floor(sum / targetDifficultyGenerationIterations);
            EditorUtility.SetDirty(data);
        }
        base.OnInspectorGUI();
    }
}
