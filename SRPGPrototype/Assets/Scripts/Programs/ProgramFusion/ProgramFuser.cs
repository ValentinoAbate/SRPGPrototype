using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramFuser : MonoBehaviour
{
    [SerializeField] private Program programTemplate;
    [SerializeField] private ActionFuser actionFuser;

    public Program FusePrograms(Transform container, Program p1, Program p2)
    {
        var fusedProgram = Instantiate(programTemplate.gameObject, container.transform).GetComponent<Program>();
        fusedProgram.color = Program.Color.Yellow;
        int maxTransientUses = 0;
        int currentTransientUses = 0;
        var actions = new List<Action>();
        var effects = new List<ProgramEffect>(p1.Effects.Length + p2.Effects.Length);
        var modifiers = new List<ProgramModifier>(p1.ModifierEffects.Length + p2.ModifierEffects.Length);
        // TODO: refactor to not be loop, just helper function or individual calculations
        IEnumerable<Program> Programs()
        {
            yield return p1;
            yield return p2;
        }
        foreach(var program in Programs())
        {
            fusedProgram.attributes |= program.attributes;
            foreach(var effect in program.Effects)
            {
                if(effect is ProgramEffectAddAction addActionEffect)
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
            if(trAttr != null)
            {
                maxTransientUses += trAttr.MaxUses;
                currentTransientUses += trAttr.Uses;
            }
        }
        if(actions.Count >= 1)
        {
            var fusedAction = fusedProgram.gameObject.AddComponent<ProgramEffectAddAction>();
            fusedAction.action = actionFuser.Fuse(fusedProgram.transform, actions);
            effects.Add(fusedAction);
        }
        if(maxTransientUses > 0)
        {
            var transientAttribute = fusedProgram.gameObject.AddComponent<ProgramAttributeTransient>();
            transientAttribute.MaxUses = maxTransientUses;
            transientAttribute.Uses = currentTransientUses;
        }
        fusedProgram.SetEffects(effects);
        fusedProgram.SetModifiers(modifiers);
        fusedProgram.SetDescription($"{p1.DisplayName} + {p2.DisplayName}");
        fusedProgram.shape = FusePatterns(p1.shape, p2.shape);
        return fusedProgram;
    }

    private Pattern FusePatterns(Pattern p1, Pattern p2)
    {
        int overlap = (p1.Offsets.Count + p2.Offsets.Count) / 3; // TODO, filter for overlap
        return RandomUtils.RandomU.instance.Choice(PatternUtils.GetAllOverlaps(p1, p2));
    }
}
