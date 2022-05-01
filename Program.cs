using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

readonly struct NArray<T>
    where T : struct
{
    private readonly T[] array;
    private readonly int startIndex;
    private readonly int endIndex;

    public NArray(T[] array, int startIndex, int endIndex)
    {
        this.array = array;
        this.startIndex = startIndex;
        this.endIndex = endIndex;
    }

    public T this[int index]
    {
        get
        {
            if (index < startIndex || index > endIndex)
                throw new IndexOutOfRangeException();

            return array[index - startIndex];
        }
    }

    public int Length => endIndex - startIndex + 1;
}

class Solution
{
    private static readonly Regex regex1
        = new Regex(@"(\w+)\[(\-?\d+)\.\.(\-?\d+)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex regex2
        = new Regex(@"(\w*)\[", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex regex3
        = new Regex(@"\[(\-?\d*)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static (string name, NArray<int> array) GetArray(string input)
    {
        var assignments = input.Split('=', StringSplitOptions.RemoveEmptyEntries);
        var def = assignments[0].Trim();
        var match = regex1.Match(def);
        if (!match.Success)
            throw new Exception();

        var name = match.Groups[1].Value;
        var start = int.Parse(match.Groups[2].Value);
        var end = int.Parse(match.Groups[3].Value);

        var array = assignments[1]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();

        return (name, new NArray<int>(array, start, end));
    }

    private static int Access(Dictionary<string, NArray<int>> dict, string input)
    {
        var matches = regex2.Matches(input);
        var numberMatch = regex3.Match(input);
        if (matches.Count == 0 || !numberMatch.Success)
            throw new Exception();

        var index = int.Parse(numberMatch.Groups[1].Value);

        for (var i = matches.Count - 1; i >= 0; i--)
        {
            var match = matches[i];
            var name = match.Groups[1].Value;
            var array = dict[name];
            index = array[index];
        }

        return index;
    }

    static void Main(string[] args)
    {
        var n = int.Parse(Console.ReadLine());
        var dict = new Dictionary<string, NArray<int>>(n);
        for (var i = 0; i < n; i++)
        {
            var assignment = Console.ReadLine();
            var (name, array) = GetArray(assignment);
            dict[name] = array;

            Console.Error.WriteLine($"{name}: {array.Length}");
        }

        var x = Console.ReadLine();
        var result = Access(dict, x);

        Console.WriteLine(result);
    }
}