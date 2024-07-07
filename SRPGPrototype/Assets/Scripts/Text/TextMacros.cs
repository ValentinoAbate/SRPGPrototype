using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TextMacros
{
	private const char beginMacro = '{';
	private const char endMacro = '}';
	private const string errorString = "?";
	public static readonly char[] macroDelim = new char[2] { beginMacro, endMacro }; // Macro delimiters
	public static readonly char[] optDelim = new char[1] { ',' }; // Option delimiter
	public static readonly char[] escapeChar = new char[1] { '\\' }; // Escape character

	private static string MacroErrorText(string macroName, string macroType)
	{
		return $"Invalid {macroType} macro: {macroName}. Returning empty string";
	}

	private static string ArgumentErrorText(string macroName, int index, string expectedType)
	{
		return $"Invalid argument macro ({index}) in {macroName}: expected {expectedType}";
	}

	private delegate string ActionMacro(string[] args, Action action, Unit user);
	private delegate string ProgramMacro(string[] args, Program program, Unit user);

	private static string ApplyMacros(string text, System.Func<string, string[], string> applyMacro)
	{
		var builder = new System.Text.StringBuilder(text.Length * 2);
		for (int i = 0; i < text.Length;)
		{
			if (text[i] == macroDelim[0])
			{
				int startPos = i + 1;
				int endPos = text.IndexOf(macroDelim[1], startPos, escapeChar);
				string[] macro = text.Substring(startPos, endPos - startPos).Split(optDelim, escapeChar);
				string[] opt = macro.Skip(1).Take(macro.Length - 1).ToArray();
				builder.Append(applyMacro(macro[0], opt));
				i = endPos + 1;
			}
			else
			{
				builder.Append(text[i++]);
			}
		}
		return builder.ToString();
	}

	#region Action Macros

	private const string dmgMacro = "dmg";
	private const string chanceMacro = "chance";
	private static readonly Dictionary<string, ActionMacro> actionMacroMap = new Dictionary<string, ActionMacro>
	{
		{ dmgMacro, ActionMacroDamage },
		{ chanceMacro, ActionMacroGambleChance },
	};

	public static string ApplyActionTextMacros(string text, Action action, Unit user)
	{
		string ApplyMacro(string macroName, string[] args)
		{
			if (actionMacroMap.TryGetValue(macroName, out var macro))
			{
				return macro(args, action, user);
			}
			Debug.LogError(MacroErrorText(macroName, "action"));
			return errorString;
		}
		return ApplyMacros(text, ApplyMacro);
	}

	private static string ActionMacroDamage(string[] args, Action action, Unit user)
	{
		var indices = GetIndices(dmgMacro, args);
		if (!TryGetSubAction(action, indices, out var sub))
		{
			return errorString;
		}
		return sub.BaseDamage(action, user, indices).ToString();
	}

	private static string ActionMacroGambleChance(string[] args, Action action, Unit user)
    {
		var indices = GetIndices(dmgMacro, args);
		if (!TryGetSubAction(action, indices, out var sub))
		{
			return errorString;
		}
		if(!TryGetActionEffect(sub, indices, out ActionEffectGamble effect))
        {
			return errorString;
        }
		int percent = Mathf.RoundToInt(effect.SuccessChance * 100);
		return $"{percent}%";
	}

	private static bool TryGetSubAction(Action action, Queue<int> indices, out SubAction sub)
	{
		int subAction = indices.Count > 0 ? indices.Dequeue() : 0;
		if (subAction >= action.SubActions.Count)
		{
			sub = null;
			return false;
		}
		sub = action.SubActions[subAction];
		return true;
	}

	private static bool TryGetActionEffect<T>(SubAction sub, Queue<int> indices, out T effect)
    {
		int effectIndex = indices.Count > 0 ? indices.Dequeue() : 0;
		if (effectIndex < sub.Effects.Count && sub.Effects[effectIndex] is T tEffect)
        {
			effect = tEffect;
			return true;
        }
		effect = default;
		return false;
	}

	#endregion

	#region Program Macros

	private const string modDmg = "modDmg";
	private static readonly Dictionary<string, ProgramMacro> programMacroMap = new Dictionary<string, ProgramMacro>
	{
		{ modDmg, ProgramMacroModDmg },
	};

	public static string ApplyProgramTextMacros(string text, Program program, Unit user)
	{
		string ApplyMacro(string macroName, string[] args)
		{
			if (programMacroMap.TryGetValue(macroName, out var macro))
			{
				return macro(args, program, user);
			}
			Debug.LogError(MacroErrorText(macroName, "program"));
			return errorString;
		}
		return ApplyMacros(text, ApplyMacro);
	}

	private static string ProgramMacroModDmg(string[] args, Program program, Unit user)
	{
		if (program.ModifierEffects.Length <= 0)
			return errorString;
		var indices = GetIndices(modDmg, args);
		var progMod = program.ModifierEffects[indices.Count > 0 ? indices.Dequeue() : 0];
		var mod = progMod.Modifiers[indices.Count > 0 ? indices.Dequeue() : 0];
		if(mod is ModifierActionDamage dmgMod)
        {
			int dmg = dmgMod.BasicDamageMod(ActionEffectDamage.TargetStat.HP, null, user);
			return dmg >= 0 ? $"+{dmg}" : dmg.ToString();
        }
		return errorString;
	}

	#endregion

	private static Queue<int> GetIndices(string macroName, string[] args, int startingIndex = 0)
    {
		if (args.Length == 0)
			return new Queue<int>(0);
		var indices = new Queue<int>(args.Length - 1);
		for (int i = startingIndex; i < args.Length; ++i)
		{
			if (!int.TryParse(args[i], out int index))
			{
				Debug.LogError(ArgumentErrorText(macroName, i, "int"));
				continue;
			}
			indices.Enqueue(index);
		}
		return indices;
	}
}
