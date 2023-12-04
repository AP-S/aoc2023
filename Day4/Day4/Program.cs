

var cards = File.ReadAllLines("input/example2.txt")
    .Select(l => l.Split(':')[1])
    .Select(l => l.Split('|'))
    .Select(i => new
    {
        winningNumbers = i[0].Trim().Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => int.Parse(n)),
        myNumbers = i[1].Trim().Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => int.Parse(n))
    })
    .Select((n, index) => new Card{ CardId = index, MyWinningNumbers = n.winningNumbers.Intersect(n.myNumbers), CardCount = 1})
    .ToList();

var firstPartAnswer = cards
    .Select(n => n.MyWinningNumbers.Any() ? Math.Pow(2, n.MyWinningNumbers.Count()-1) : 0)
    .Sum();
Console.WriteLine($"First part answer: {firstPartAnswer}");

cards.ForEach(i =>
{
    var cardIds = Enumerable.Range(i.CardId + 1, i.MyWinningNumbers.Count());
    cards.Where(c => cardIds.Contains(c.CardId)).ToList().ForEach(c => c.CardCount += i.CardCount);
});
var secondPartAnswer = cards.Select(c => c.CardCount).Sum();
Console.WriteLine($"Second part answer: {secondPartAnswer}");

class Card
{
    public int CardId { get; set; }
    public IEnumerable<int> MyWinningNumbers { get; set; } = Enumerable.Empty<int>();
    public int CardCount { get; set; }
}