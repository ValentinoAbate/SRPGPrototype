using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramFuser : MonoBehaviour
{
    [SerializeField] private Program programTemplate;
    [SerializeField] private ActionFuser actionFuser;

    public Program FusePrograms(Transform container, Program p1, Program p2)
    {
        var p1Clone = Instantiate(p1, container);
        var p2Clone = Instantiate(p2, container);
        var fusedProgram = FuseProgramsInternal(container, p1Clone, p2Clone);
        fusedProgram.shape = FusePatterns(p1Clone.shape, p2Clone.shape);
        return fusedProgram;
    }

    public IReadOnlyList<Program> GetFusions(Transform container, Program p1, Program p2, int maxFusions, int maxFallbackFusions)
    {
        var p1Clone = Instantiate(p1, container);
        var p2Clone = Instantiate(p2, container);
        var fusedProgramTemplate = FuseProgramsInternal(container, p1Clone, p2Clone);
        var patterns = FusePatterns(p1Clone.shape, p2Clone.shape, maxFusions, maxFallbackFusions);
        if (patterns.Count <= 0)
        {
            return System.Array.Empty<Program>();
        }
        var fusions = new List<Program>(patterns.Count) { fusedProgramTemplate };
        for(int i = 1; i < patterns.Count; ++i)
        {
            var program = Instantiate(fusedProgramTemplate, container);
            program.shape = patterns[i];
            fusions.Add(program);
        }
        fusedProgramTemplate.shape = patterns[0];
        return fusions;
    }

    private Program FuseProgramsInternal(Transform container, Program p1, Program p2)
    {
        var fusedProgram = Instantiate(programTemplate.gameObject, container.transform).GetComponent<Program>();
        p1.transform.SetParent(fusedProgram.transform);
        p2.transform.SetParent(fusedProgram.transform);
        fusedProgram.color = Program.Color.Yellow;
        int maxTransientUses = 0;
        int currentTransientUses = 0;
        var actions = new List<Action>();
        var effects = new List<ProgramEffect>(p1.Effects.Length + p2.Effects.Length);
        var modifiers = new List<ProgramModifier>(p1.ModifierEffects.Length + p2.ModifierEffects.Length);
        ProcessProgram(p1, ref actions, ref effects, ref modifiers, ref maxTransientUses, ref currentTransientUses);
        ProcessProgram(p2, ref actions, ref effects, ref modifiers, ref maxTransientUses, ref currentTransientUses);
        if (actions.Count >= 1)
        {
            var fusedAction = fusedProgram.gameObject.AddComponent<ProgramEffectAddAction>();
            fusedAction.action = actionFuser.Fuse(fusedProgram.transform, actions);
            effects.Add(fusedAction);
        }
        if (maxTransientUses > 0)
        {
            var transientAttribute = fusedProgram.gameObject.AddComponent<ProgramAttributeTransient>();
            transientAttribute.MaxUses = maxTransientUses;
            transientAttribute.Uses = currentTransientUses;
        }
        fusedProgram.attributes = p1.attributes | p2.attributes;
        fusedProgram.SetEffects(effects);
        fusedProgram.SetModifiers(modifiers);
        fusedProgram.SetDescription($"{p1.DisplayName} + {p2.DisplayName}");
        return fusedProgram;
    }

    private void ProcessProgram(Program program, ref List<Action> actions, ref List<ProgramEffect> effects, ref List<ProgramModifier> modifiers, ref int maxTransientUses, ref int currentTransientUses)
    {
        foreach (var effect in program.Effects)
        {
            if (effect is ProgramEffectAddAction addActionEffect)
            {
                actions.Add(addActionEffect.action);
            }
            else
            {
                effects.Add(effect);
            }
        }
        modifiers.AddRange(program.ModifierEffects);
        var trAttr = program.GetComponent<ProgramAttributeTransient>();
        if (trAttr != null)
        {
            maxTransientUses += trAttr.MaxUses;
            currentTransientUses += trAttr.Uses;
        }
    }

    private Pattern FusePatterns(Pattern p1, Pattern p2)
    {
        int targetSize = GetPatternFusionTargetSize(p1, p2);
        if (!GetPatternFusionOptions(p1, p2, targetSize, out var choices, out var allOptions))
        {
            // Choices is empty, choose from all options, weighted by distance from targetSize (less is better)
            float Weight(Pattern p) => FallbackFusionPatternWeight(p, targetSize);
            choices.AddRange(allOptions, Weight);
        }
        return RandomU.instance.Choice(choices);
    }

    private IReadOnlyList<Pattern> FusePatterns(Pattern p1, Pattern p2, int maxOptions, int maxFallbackOptions)
    {
        int targetSize = GetPatternFusionTargetSize(p1, p2);
        int limit;
        if(GetPatternFusionOptions(p1, p2, targetSize, out var choices, out var allOptions))
        {
            limit = maxOptions;
        }
        else
        {
            // Choices is empty, choose from all options, weighted by distance from targetSize (less is better)
            float Weight(Pattern p) => FallbackFusionPatternWeight(p, targetSize);
            choices.AddRange(allOptions, Weight);
            limit = maxFallbackOptions;
        }
        var patterns = new List<Pattern>(Mathf.Min(limit, choices.Count));
        while (choices.Count > 0 && patterns.Count < limit)
        {
            var choice = RandomU.instance.Choice(choices);
            patterns.Add(choice);
            choices.Remove(choice);
        }
        return patterns;
    }

    private static int GetPatternFusionTargetSize(Pattern p1, Pattern p2) => Mathf.Max(p1.Offsets.Count, p2.Offsets.Count) + (Mathf.Min(p1.Offsets.Count, p2.Offsets.Count) / 2);

    private static float FallbackFusionPatternWeight(Pattern p, int targetSize) => 1 / Mathf.Pow(Mathf.Abs(targetSize - p.Offsets.Count), 3);

    private bool GetPatternFusionOptions(Pattern p1, Pattern p2, int targetSize, out WeightedSet<Pattern> targetSizeOptions, out IReadOnlyList<Pattern> allOptions)
    {
        allOptions = PatternUtils.GetAllOverlaps(p1, p2);
        targetSizeOptions = new WeightedSet<Pattern>(allOptions.Count);
        foreach (var option in allOptions)
        {
            if (option.Offsets.Count == targetSize)
                targetSizeOptions.Add(option);
        }
        return targetSizeOptions.Count > 0;
    }
}
