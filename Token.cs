using System.Text.RegularExpressions;

namespace Magnit.Tokenization
{
    public class Token
    {
        public int StartIndex { get; set; }
        public string? Type { get; set; }
        public object? Value { get; set; }
    }
}