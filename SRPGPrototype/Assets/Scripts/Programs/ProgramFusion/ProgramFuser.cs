using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ProgramFuser : MonoBehaviour
{
    private const string descFilterRegex = @"Adds Actions:[\s\S]*\.";
    [SerializeField] private Program programTemplate;
    [SerializeField] private ActionFuser actionFuser;

    public Program FusePrograms(Transform container, Program p1, Program p2, Pattern pattern = null)
    {
        var fusedProgram = FuseProgramsInternal(container, p1, p2, false);
        fusedProgram.shape = pattern ?? FusePatterns(p1.shape, p2.shape);
        return fusedProgram;
    }

    public IReadOnlyList<Program> GetFusionPreviews(Transform container, Program p1, Program p2, int maxFusions, int maxFallbackFusions)
    {
        var fusedProgramTemplate = FuseProgramsInternal(container, p1, p2, true);
        var patterns = FusePatterns(p1.shape, p2.shape, maxFusions, maxFallbackFusions);
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

    private Program FuseProgramsInternal(Transform container, Program p1, Program p2, bool preview)
    {
        var fusedProgram = Instantiate(programTemplate.gameObject, container.transform).GetComponent<Program>();
        if (!preview)
        {
            p1.transform.SetParent(fusedProgram.transform);
            p2.transform.SetParent(fusedProgram.transform);
        }
        int maxTransientUses = 0;
        int currentTransientUses = 0;
        var effects = new List<ProgramEffect>(p1.Effects.Length + p2.Effects.Length);
        var modifiers = new List<ProgramModifier>(p1.ModifierEffects.Length + p2.ModifierEffects.Length);
        var p1ActionEffects = new List<ProgramEffectAddAction>();
        var p1ActionEffectsSecondary = new List<ProgramEffectAddAction>();
        ProcessProgram(p1, ref p1ActionEffects, ref p1ActionEffectsSecondary, ref effects, ref modifiers, ref maxTransientUses, ref currentTransientUses);
        var p2ActionEffects = new List<ProgramEffectAddAction>();
        var p2ActionEffectsSecondary = new List<ProgramEffectAddAction>();
        ProcessProgram(p2, ref p2ActionEffects, ref p2ActionEffectsSecondary, ref effects, ref modifiers, ref maxTransientUses, ref currentTransientUses);
        // Process Actions
        bool fusionActionCreated = ProcessActions(fusedProgram.gameObject, p1ActionEffects, p2ActionEffects, ref effects, false, preview);
        fusionActionCreated |= ProcessActions(fusedProgram.gameObject, p1ActionEffectsSecondary, p2ActionEffectsSecondary, ref effects, true, preview);
        // Fuse Colors
        fusedProgram.color = FuseColors(p1.color, p2.color, fusionActionCreated);
        // Fuse Attributes
        fusedProgram.attributes = p1.attributes | p2.attributes;
        if (maxTransientUses > 0)
        {
            var transientAttribute = fusedProgram.gameObject.AddComponent<ProgramAttributeTransient>();
            transientAttribute.MaxUses = maxTransientUses;
            transientAttribute.Uses = currentTransientUses;
        }
        // Set effects and modifiers
        fusedProgram.SetEffects(effects, !preview);
        fusedProgram.SetModifiers(modifiers);
        if (!preview)
        {
            foreach(var mod in GetComponentsInChildren<Modifier>())
            {
                mod.Program = fusedProgram;
            }
        }
        fusedProgram.SetDescription(FuseDescriptions(p1, p2, fusedProgram, fusionActionCreated));
        return fusedProgram;
    }

    private void ProcessProgram(Program program, ref List<ProgramEffectAddAction> actionEffects, ref List<ProgramEffectAddAction> actionEffectsSecondary, ref List<ProgramEffect> effects, ref List<ProgramModifier> modifiers, ref int maxTransientUses, ref int currentTransientUses)
    {
        foreach (var effect in program.Effects)
        {
            if (effect is ProgramEffectAddAction addActionEffect)
            {
                if (addActionEffect.isSecondary)
                {
                    actionEffectsSecondary.Add(addActionEffect);
                }
                else
                {
                    actionEffects.Add(addActionEffect);
                }
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

    private bool ProcessActions(GameObject container, List<ProgramEffectAddAction> actionEffects1, List<ProgramEffectAddAction> actionEffects2, ref List<ProgramEffect> effects, bool isSecondary, bool preview)
    {
        // If either action effect list is empty, pass through the other's actions (if any)
        if (CheckEmptyActionEffects(actionEffects1, actionEffects2, ref effects) || CheckEmptyActionEffects(actionEffects2, actionEffects1, ref effects))
        {
            return false;
        }
        if (actionEffects1.Count == actionEffects2.Count)
        {
            FuseActionsSimple(container, actionEffects1, actionEffects2, ref effects, isSecondary, preview);
            return true;
        }
        FuseActionsAsymmetrical(container, actionEffects1, actionEffects2, ref effects, isSecondary, preview);
        return true;
    }

    private static bool CheckEmptyActionEffects(List<ProgramEffectAddAction> actionEffects, List<ProgramEffectAddAction> actionEffectsOther, ref List<ProgramEffect> effects)
    {
        if (actionEffects.Count <= 0)
        {
            effects.AddRange(actionEffectsOther);
            return true;
        }
        return false;
    }

    private void FuseActionsSimple(GameObject container, List<ProgramEffectAddAction> actionEffects1, List<ProgramEffectAddAction> actionEffects2, ref List<ProgramEffect> effects, bool isSecondary, bool preview)
    {
        var actions = new List<Action>(2);
        for (int i = 0; i < actionEffects1.Count; i++)
        {
            actions.Add(actionEffects1[i].action);
            actions.Add(actionEffects2[i].action);
            effects.Add(CreateFusedAction(container, actions, isSecondary, preview));
            actions.Clear();
        }
    }

    private void FuseActionsAsymmetrical(GameObject container, List<ProgramEffectAddAction> actionEffects1, List<ProgramEffectAddAction> actionEffects2, ref List<ProgramEffect> effects, bool isSecondary, bool preview)
    {
        int greaterCount = Mathf.Max(actionEffects1.Count, actionEffects2.Count);
        int lesserCount = Mathf.Min(actionEffects1.Count, actionEffects2.Count);
        int ratio = greaterCount / lesserCount;
        if (greaterCount % lesserCount != 0)
            ++ratio;
        int index1 = 0;
        int index2 = 0;
        bool e1Greater = actionEffects1.Count > actionEffects2.Count;
        var actions = new List<Action>(ratio + 1);
        for (int i = 0; i < lesserCount; i++)
        {
            if (e1Greater)
            {
                AddActions(actionEffects1, ratio, ref index1, ref actions);
                actions.Add(actionEffects2[index2++].action);
            }
            else
            {
                actions.Add(actionEffects1[index1++].action);
                AddActions(actionEffects2, ratio, ref index2, ref actions);
            }
            effects.Add(CreateFusedAction(container, actions, isSecondary, preview));
            actions.Clear();
        }
    }

    private static void AddActions(List<ProgramEffectAddAction> actionEffects, int num, ref int index, ref List<Action> actions)
    {
        for (int i = 0; i < num; ++i)
        {
            if (index >= actionEffects.Count)
                break;
            actions.Add(actionEffects[index++].action);
        }
    }

    private ProgramEffectAddAction CreateFusedAction(GameObject container, IReadOnlyList<Action> actions, bool isSecondary, bool preview)
    {
        var fusedAction = container.AddComponent<ProgramEffectAddAction>();
        fusedAction.action = actionFuser.Fuse(container.transform, actions, preview);
        fusedAction.isSecondary = isSecondary;
        return fusedAction;
    }

    private Program.Color FuseColors(Program.Color c1, Program.Color c2, bool fusionActionCreated)
    {
        if (fusionActionCreated)
            return Program.Color.Yellow;
        if (c1 == c2)
            return c1;
        if (c1 == Program.Color.White)
            return c2;
        if (c2 == Program.Color.White)
            return c1;
        // If c1 || c2 is Gray, return Gray
        return Program.Color.Yellow;
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

    private string FuseDescriptions(Program p1, Program p2, Program fusedProgram, bool fusionActionCreated)
    {
        if (p1.Description == p2.Description)
            return p1.Description;
        if (fusionActionCreated)
        {
            int fusedProgramActionCount = 0;
            foreach(var effect in fusedProgram.Effects)
            {
                if(effect is ProgramEffectAddAction)
                {
                    ++fusedProgramActionCount;
                }
            }
            if (fusedProgramActionCount == 1)
            {
                return CombineDescriptionsFiltered(p1, p2, string.Empty);
            }
            string desc = "Adds Actions: ";
            bool firstAction = true;
            foreach (var effect in fusedProgram.Effects)
            {
                if (effect is ProgramEffectAddAction addAction)
                {
                    if (firstAction)
                    {
                        desc += addAction.action.DisplayName;
                        firstAction = false;
                    }
                    else
                    {
                        desc += $", and {addAction.action.DisplayName}";
                    }
                }
            }
            desc += ".";
            return CombineDescriptionsFiltered(p1, p2, desc);
        }
        else
        {
            return CombineDescriptionsBasic(p1, p2);
        }

    }

    private string CombineDescriptionsBasic(Program p1, Program p2)
    {
        if (p1.Description == Program.actionOnlyDescription)
        {
            return p2.Description;
        }
        else if (p2.Description == Program.actionOnlyDescription)
        {
            return p1.Description;
        }
        else
        {
            return p1.Description + " " + p2.Description;
        }
    }

    private string CombineDescriptionsFiltered(Program p1, Program p2, string desc)
    {
        if (p1.Description != Program.actionOnlyDescription)
        {
            string keep = Regex.Replace(p1.Description, descFilterRegex, string.Empty);
            if (!string.IsNullOrWhiteSpace(keep))
            {
                desc += " " + p1.Description;
            }
        }
        else if (p2.Description != Program.actionOnlyDescription)
        {
            string keep = Regex.Replace(p2.Description, descFilterRegex, string.Empty);
            if (!string.IsNullOrWhiteSpace(keep))
            {
                desc += " " + p2.Description;
            }
        }
        return string.IsNullOrWhiteSpace(desc) ? Program.actionOnlyDescription : desc;
    }
}
