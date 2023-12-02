var firstLastPairs = File.ReadAllText("input/example2.txt")
    .Split('\n')
    .Select(l => $"{l.ToCharArray().First(c => char.IsDigit(c))}{l.ToCharArray().Reverse().First(c => char.IsDigit(c))}");
Console.WriteLine($"value sum is {firstLastPairs.Select(v=>int.Parse(v)).Sum()}");

var phrases = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
firstLastPairs = File.ReadAllText("input/example2.txt")
    .Split('\n')
    .Select(l => { var t = l; for (var i = 0; i < phrases.Length; i++) { t = t.Replace(phrases[i], phrases[i][0] +(i + 1).ToString() + phrases[i][^1]); } return t; })
    .Select(l => $"{l.ToCharArray().First(c => char.IsDigit(c))}{l.ToCharArray().Reverse().First(c => char.IsDigit(c))}");
Console.WriteLine($"value sum is {firstLastPairs.Select(v => int.Parse(v)).Sum()}");