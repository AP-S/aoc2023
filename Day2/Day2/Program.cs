// See https://aka.ms/new-console-template for more information
Dictionary<string, int> GetRevealData(string revealsLine)
{
    var reveals = new Dictionary<string, int>();
    revealsLine.Split(',')
        .Select(r => r.Trim())
        .Select(i => { var parts = i.Split(' '); return new { color = parts[1], count = int.Parse(parts[0]) }; })
        .ToList().ForEach(i => reveals[i.color] = i.count);
    return reveals;
};

(int gameId, List<Dictionary<string, int>> gameReveals) GetGameData(string gameLine)
{
        var lineParts = new { id = gameLine.Split(':')[0], reveals = gameLine.Split(':')[1] };
        var gameId = int.Parse(lineParts.id.Replace("Game", "").Trim());
        var gameReveals = new List<Dictionary<string, int>>();
        lineParts.reveals.Split(';')
            .Select(reveal => GetRevealData(reveal))
            .ToList().ForEach(reveal => gameReveals.Add(reveal));
    return (gameId, gameReveals);
};

IEnumerable<int> GetPossibleGameIds(Dictionary<string, int> maxCubeValues, Dictionary<int, List<Dictionary<string, int>>> gameData)
{
    var impossibleGameIds = new List<int>();
    foreach (var maxCubeValue in maxCubeValues)
    {
        foreach(var game in gameData)
        {
            if (game.Value.Any(reveal => reveal.ContainsKey(maxCubeValue.Key) && reveal[maxCubeValue.Key] > maxCubeValue.Value))
            {
                impossibleGameIds.Add(game.Key);
            }
        }      
    }
    foreach (var game in gameData)
    {
        if (game.Value.Any(reveals => reveals.Keys.Any(k => !maxCubeValues.Keys.Contains(k))))
        {
            impossibleGameIds.Add(game.Key);
        }
    }

    return gameData.Keys.Except(impossibleGameIds);
}

IEnumerable<int> GetMinCubeMultiply(Dictionary<int, List<Dictionary<string, int>>> gameData)
{
    var multiplierResults = new List<int>();
    foreach(var game in gameData) 
    {
        var colors = game.Value.SelectMany(v => v.Keys).Distinct();
        var multipliers = new List<int>();
        foreach (var color in colors)
        {
            multipliers.Add(game.Value.Where(v => v.ContainsKey(color)).Max(v => v[color]));
        }
        var result = 1;
        multipliers.ForEach(m => result *= m);
        multiplierResults.Add(result);
    }

    return multiplierResults;
}



void CalculateCheckSumForPossibleGames(string inputFilePath)
{
    var lines = File.ReadAllText(inputFilePath).Split('\n');
    var parsedGameData = new Dictionary<int, List<Dictionary<string, int>>>();
    lines
        .Select(l => GetGameData(l))
        .ToList().ForEach(gameData => parsedGameData[gameData.gameId] = gameData.gameReveals);
    // only 12 red cubes, 13 green cubes, and 14 blue cubes
    var checkCondition = new Dictionary<string, int>
{
    {"red",12 },
    {"green",13 },
    {"blue",14 }
};
    var possibleGameIds = GetPossibleGameIds(checkCondition, parsedGameData);
    Console.WriteLine(nameof(CalculateCheckSumForPossibleGames));
    Console.WriteLine(string.Join(",", possibleGameIds));
    Console.WriteLine(possibleGameIds.Sum());
}

void CalculateCheckSumForMinimumCubes(string inputFilePath)
{
    var lines = File.ReadAllText(inputFilePath).Split('\n');
    var parsedGameData = new Dictionary<int, List<Dictionary<string, int>>>();
    lines
        .Select(l => GetGameData(l))
        .ToList().ForEach(gameData => parsedGameData[gameData.gameId] = gameData.gameReveals);
    
    var multiplyResults = GetMinCubeMultiply(parsedGameData);
    Console.WriteLine(nameof(CalculateCheckSumForMinimumCubes));
    Console.WriteLine(string.Join(",", multiplyResults));
    Console.WriteLine(multiplyResults.Sum());
}


CalculateCheckSumForPossibleGames("input/example1.txt");
CalculateCheckSumForPossibleGames("input/example2.txt");

CalculateCheckSumForMinimumCubes("input/example1.txt");
CalculateCheckSumForMinimumCubes("input/example2.txt");


