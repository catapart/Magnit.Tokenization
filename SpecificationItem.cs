using System.Text.RegularExpressions;

namespace Magnit.Tokenization
{
    public class SpecificationItem
    {
        public Regex Regex { get; set; }
        public string Key { get; set; }
        public string[]? CaptureGroups { get; set; }
        public string? GroupJoinValue { get; set; }
        public Func<string, Task<string>>? ResultCallback { get; set; }
    }
}