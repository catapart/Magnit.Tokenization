# Magnit.Tokenization
 Tokenize strings into custom tokens using ordered regex operations.

## Overview
This library takes a string input and asynchronously parses through it to produce a `List` of `Token` objects. These `Token` objects are completely custom, and are used to represent whatever distinct parts of the text you would like to separate.

The `Token`s are defined in a `Specification` object that requires a `Regex` to match a string and a "Type" string, which is used to identify the token to you.

The order that you define your `Specification` is used to run the regex comparisons. That means that the first `SpecificationItem`'s regex to match a given string in the text will be how that string is tokenized. This usually takes a little trial and error, but it allows you to do things like ignore all whitespace. A good rule of thumb is to always use the "start of line" expression (`^`), and not to use multiline flags. You can see working examples, below. 

You also have the option of defining an asynchronous function to perform string manipulation on the matched string. That way if you match something with markup, like `<custom-tag>`, you can strip the unnecessary markup and use `custom-tag` as your `Token`'s value.

## Usage

 ### Create a Tokenizer
 ```C#
 public Tokenizer Tokenizer { get; set; } = new Tokenizer(CurrentSpecification);
 ```

 ### Create a Specification
 ```C#
public static Magnit.Tokenization.Specification CurrentSpecification { get; set; } = new()
{
    // Whitespace
    { new Regex(@"^\s+"), null }, // Returning null as the token Type will skip the match. This regex prevents whitespace from being represented in the returned token list. 

    // Comments
    { new Regex(@"^\/\/.*"), null },
    { new Regex(@"^\/\*[\s\S]*?\*\/"), null },

    // String
    { new Regex(@"^.*"), "STRING" },
    // Tagged String
    { new Regex(@"^#.*"), "TAGGED_STRING" },
    // Cleaned Tagged String
    { new Regex(@"^#.*"), "CLEANED_TAGGED_STRING", (result) => { return Task.FromResult(result.TrimStart('#')); } }, // Pass in an async function to handle any string manipulation on the matching token
            
    // Utility
    { new Regex(@"^[\s\S]*"), "UNKNOWN" }, // Capture the point where an unknown character is represented to prevent errors.
};
 ```

### Parse into a List of Tokens
```C#
private async Task ParseText(string input)
{
    List<Token> tokens = await Tokenizer.Parse(input);
    foreach (Token token in tokens)
    {
        Console.WriteLine($"Type: {token.Type}, Start Index: {token.StartIndex}")
    }
}
```

## What is this for?
Tokenization is used for breaking up plain text into discrete objects. That could be paragraphs, for grammatical tools, or into blocks that are then interpreted as logic for a code language.  
It is usually just the very first part of a larger process. This library is focused on making Tokenization simple and straightforward, rather than super optimized. Most of what stops me from parsing plain text isn't the speed; it's the many layers of planning it takes to get something useable. This library crunches that down into three questions:
 1. What do you want to match?
 2. How do you want to represent that to your code?
 3. How do you want to handle the result?

With that simplification, I find it easier to convert plain text blobs into useable objects for my code to interact with. If it sounds like you would get that same benefit, give this library a try.