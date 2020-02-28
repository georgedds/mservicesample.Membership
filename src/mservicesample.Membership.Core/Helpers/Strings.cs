using System.Text.RegularExpressions;

namespace mservicesample.Membership.Core.Helpers
{
    public class Strings
    {
        public static string RemoveAllNonPrintableCharacters(string target)
        {
            return Regex.Replace(target, @"\p{C}+", string.Empty);
        }
    }
}
