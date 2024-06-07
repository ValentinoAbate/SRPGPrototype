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

	private const string dmgMacro = "dmg";
	private static readonly Dictionary<string, ActionMacro> actionMacroMap = new Dictionary<string, ActionMacro>
	{
		{ dmgMacro, ActionMacroDamage }
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
			return string.Empty;
		}
		return ApplyMacros(text, ApplyMacro);
	}

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

	private static string ActionMacroDamage(string[] args, Action action, Unit user)
	{
		if (!TryGetSubAction(args, action, 0, out var sub))
		{
			return errorString;
		}
		if (!sub.DealsDamage)
		{
			return errorString;
		}
		if(args.Length > 1)
        {
			var indices = new int[args.Length - 1];
			for(int i = 1; i < args.Length; ++i)
            {
				if(!int.TryParse(args[i], out int index))
                {
					Debug.LogError(ArgumentErrorText(dmgMacro, i, "int"));
					continue;
                }
				indices[i - 1] = index;
            }
			return sub.BaseDamage(action, user, indices).ToString();
		}
		return sub.BaseDamage(action, user).ToString();
	}

	private static bool TryGetSubAction(string[] args, Action action, int argIndex, out SubAction sub)
	{
		int subAction = 0;
		if (args.Length > argIndex)
		{
			int.TryParse(args[argIndex], out subAction);
		}
		if (subAction >= action.SubActions.Count)
		{
			sub = null;
			return false;
		}
		sub = action.SubActions[0];
		return true;
	}
}
