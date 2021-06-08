using System.Linq;
using System.Text.RegularExpressions;

namespace MyBox
{
	public static class MyRegex
	{
		public const string WholeNumber = @"^-?\d+$";
		public const string FloatingNumber = @"^-?\d*(\.\d+)?$";

		public const string AlphanumericWithoutSpace = @"^[a-zA-Z0-9]*$";
		public const string AlphanumericWithSpace = @"^[a-zA-Z0-9 ]*$";

		public const string Email = @"^([a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6})*$";
		public const string URL = @"(https?:\/\/)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)";
		
		
		/// <summary>
		/// Replace all but matching parts of the input string
		/// </summary>
		public static string KeepMatching(this Regex regex, string input) => regex.Matches(input).Cast<Match>()
			.Aggregate(string.Empty, (a, m) => a + m.Value);
	}
}