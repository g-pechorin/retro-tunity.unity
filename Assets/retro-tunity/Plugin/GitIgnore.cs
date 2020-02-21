
using System.Collections.Generic;

using System.IO;

public static class GitIgnore
{
	public static HashSet<string> Ignored
	{
		get
		{
			HashSet<string> set = new HashSet<string>();

			foreach (string line in File.ReadAllLines(".gitignore"))
			{
				// strip it
				string strip = line.replaceAll("#.*", "");

				// trim it
				string trim = strip.Trim();

				// add it
				set.Add(trim);
			}

			return set;
		}
	}

	public static string Ignore
	{
		set
		{
			if (!Ignored.Contains(value))
			{
				using (StreamWriter writer = File.AppendText(".gitignore"))
				{
					writer.WriteLine(value);
				}
			}
		}
	}
}
