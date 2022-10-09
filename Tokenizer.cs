using System.Text.RegularExpressions;
using System.Text;

namespace Magnit.Tokenization
{
    public sealed class Tokenizer
    {
        public Specification Specification { get; set; }
        private string? Input { get; set; }
        private int Cursor { get; set; }
        public int PreviousCursorPosition { get; set; }
        private Token? Lookahead { get; set; }

        public Tokenizer(Specification specification)
        {
            this.Specification = specification;
            this.Input = string.Empty;
            this.PreviousCursorPosition = int.MinValue;
        }


        public async Task<List<Token>> Parse(string input, Specification? specification = null)
        {
            if(specification != null)
            {
                this.Specification = specification;
            }

            return await GetTokens(input);
        }
        private async Task<List<Token>> GetTokens(string input)
        {
            this.Input = input;
            this.Cursor = 0;

            this.Lookahead = await this.GetNextToken();

            List<Token> tokens = new List<Token>();
            while (this.HasMoreTokens())
            {
                this.PreviousCursorPosition = this.GetCursorPosition();
                tokens.Add(await this.Consume(this.Lookahead?.Type));
            }
            if (this.Lookahead != null)
            {
                tokens.Add(await this.Consume(this.Lookahead.Type));
            }
            return tokens;
        }

        public async Task<Token> Consume(string? tokenType)
        {
            Token? token = this.Lookahead;
            if (token == null)
            {
                throw new FormatException($"Unexpected end of input. Expected: \"{tokenType}\"");
            }
            if (token.Type != tokenType)
            {
                throw new FormatException($"Unexpected token: \"{token.Value}\". Expected: \"{tokenType}\"");
            }

            this.Lookahead = await this.GetNextToken();
            return token;
        }

        public bool IsEndOfFile()
        {
            return this.Cursor == this.Input?.Length;
        }

        public bool HasMoreTokens()
        {
            return this.Cursor < this.Input?.Length;
        }

        public int GetCursorPosition()
        {
            return this.Cursor;
        }

        public async Task<Token?> GetNextToken()
        {
            if (!this.HasMoreTokens() || Specification == null || Input == null)
            {
                return null;
            }

            string input = this.Input.Substring(this.Cursor);
            int startIndex = this.Cursor;
            foreach (SpecificationItem item in this.Specification)
            {
                string? tokenType = item.Key;
                string? tokenValue = Match(item, input);
                if (tokenValue == null)
                {
                    continue;
                }
                if (tokenType == null)
                {
                    return await this.GetNextToken();
                }
                return new Token()
                {
                    StartIndex = startIndex,
                    Type = tokenType,
                    Value = (item.ResultCallback == null) ? tokenValue : await item.ResultCallback(tokenValue)
                };
            }

            throw new FormatException($"Unexpected token: \"{input[0]}\"");
        }
        private string? Match(SpecificationItem item, string input)
        {
            var match = item.Regex.Match(input);
            if (match.Success == false)
            {
                return null;
            }

            string value = match.Value;
            this.Cursor += match.Value.Length;

            if (item.CaptureGroups != null)
            {
                StringBuilder combinedCapturedValue = new StringBuilder();
                for (int i = 0; i < item.CaptureGroups.Length; i++)
                {
                    string captureGroupName = item.CaptureGroups[i];
                    string? capturedValue = null;
                    foreach (Group group in match.Groups)
                    {
                        if (group.Name == captureGroupName)
                        {
                            capturedValue = group.Value;
                            break;
                        }
                    }
                    if (capturedValue != null)
                    {
                        combinedCapturedValue.Append(capturedValue);
                        if (item.GroupJoinValue != null && item.CaptureGroups.Length > (i + 1))
                        {
                            combinedCapturedValue.Append(item.GroupJoinValue);
                        }
                    }
                }
                value = combinedCapturedValue.ToString();
            }


            return value;

        }
    }
}