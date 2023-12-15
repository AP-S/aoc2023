using Microsoft.Toolkit.HighPerformance;
using System.Data;

var maps = InitializeMaps("input/input.txt");
int partOneAnswer = 0;
int partTwoAnswer = 0;
maps.ForEach(map =>
{
    var result = GetMirrorFromMap(map, differenceCount: 0);
    partOneAnswer += result.leftLinesCount + result.topLinesCount * 100;
    result = GetMirrorFromMap(map, differenceCount: 1);
    partTwoAnswer += result.leftLinesCount + result.topLinesCount * 100;
});

Console.WriteLine($"Part one answer: {partOneAnswer}");
Console.WriteLine($"Part two answer: {partTwoAnswer}");

List<char[,]> InitializeMaps(string filePath)
{
    var maps = new List<char[,]>();
    var mapLines = new List<string>();
    var fileLines = File.ReadAllLines(filePath).ToList();
    fileLines.Add("");

    fileLines.ForEach(l =>
    {
        if (string.IsNullOrWhiteSpace(l))
        {
            maps.Add(new char[mapLines.Count, mapLines.First().Length]);
            mapLines.Select((line, row) => line.ToCharArray().Select((c, column) => new { c, row, column })).SelectMany(i => i)
            .ToList().ForEach(i => maps.Last()[i.row, i.column] = i.c);
            mapLines.Clear();
        }
        else
        {
            mapLines.Add(l);
        }
    });

    return maps;
}

bool IsMatch(char[,] leftOrTop, char[,] rightOrBottom, int differenceCount, MirrorDirection mirrorDirection)
{
    var differences = 0;
    var rows = leftOrTop.GetLength(0);
    var columns = leftOrTop.GetLength(1);
    for (int row = 0; row < rows; row++)
    {
        for (int column = 0; column < columns; column++)
        {
            if (mirrorDirection == MirrorDirection.Vertical)
            {
                if (leftOrTop[row, columns - column - 1] != rightOrBottom[row, column])
                { differences++; }
            }
            else
            {
                if (leftOrTop[rows - row - 1, column] != rightOrBottom[row, column])
                { differences++; }
            }
        }
    }
    return differences == differenceCount;
}

(int mirrorLinesWidth, int leftLinesCount, int topLinesCount) GetMirrorFromMap(char[,] map, int differenceCount)
{
    var matrix = new ReadOnlySpan2D<char>(map);
    var columns = map.GetLength(1);
    var rows = map.GetLength(0);
    for (int column = 1; column < columns; column++)
    {
        var maxSliceWidth = new[] { column, columns - column }.Min();
        var left = matrix.Slice(0, column - maxSliceWidth, rows, maxSliceWidth).ToArray();
        var right = matrix.Slice(0, column, rows, maxSliceWidth).ToArray();
        if (IsMatch(left, right, differenceCount, MirrorDirection.Vertical))
        {
            return new(maxSliceWidth, column, 0);
        }
    }

    for (int row = 1; row < rows; row++)
    {
        var maxSliceHeight = new[] { row, rows - row }.Min();
        var top = matrix.Slice(row - maxSliceHeight, 0, maxSliceHeight, columns).ToArray();
        var bottom = matrix.Slice(row, 0, maxSliceHeight, columns).ToArray();
        if (IsMatch(top, bottom, differenceCount, MirrorDirection.Horizontal))
        {
            return new(maxSliceHeight, 0, row);
        }
    }
    return new(0, 0, 0);
}

public enum MirrorDirection { Vertical = 0, Horizontal = 1 }