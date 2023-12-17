var startingBeam = new Beam(new BeamLocation(new Location(0, -1), Direction.Right));
var map = new Map();
map.InitializeMap("input/input.txt");
map.CalculateBeamPaths(startingBeam);
Console.WriteLine($"Part one answer: {map.GetVisitedTileCount()}");

var startingBeams = Enumerable.Range(0, map.Rows)
   .Select(row => new[] {
       new Beam(new BeamLocation(new Location(row, -1), Direction.Right)),
       new Beam(new BeamLocation(new Location(row, map.Columns), Direction.Left)) })
   .Concat(Enumerable.Range(0, map.Columns)
        .Select(column => new[] {
            new Beam(new BeamLocation(new Location(-1, column), Direction.Down)),
            new Beam(new BeamLocation(new Location(map.Rows, column), Direction.Up)) }))
   .SelectMany(b => b).ToList();
var visitedTileCounts = new List<int>();
startingBeams.ForEach(startingBeam => {
    map.CalculateBeamPaths(startingBeam);
    visitedTileCounts.Add(map.GetVisitedTileCount());
    }
);
Console.WriteLine($"Part two answer: {visitedTileCounts.Max()}");

public enum Direction { Right, Up, Left, Down};

public class Map
{
    const char HORIZONTAL_SPLITTER = '-';
    const char VERTICAL_SPLITTER = '|';
    const char MIRROR_PLUS_45 = '/';
    const char MIRROR_MINUS_45 = '\\';
    Dictionary<Direction, Location> LocationDifference = new Dictionary<Direction, Location>
    {
        { Direction.Right,  new Location(0, 1) },
        { Direction.Up,     new Location(-1, 0) },
        { Direction.Left,   new Location(0, -1) },
        { Direction.Down,   new Location(1, 0) }
    };
    Dictionary<Direction, Direction> MirrorPlus45NextDirection = new Dictionary<Direction, Direction>
    {
        { Direction.Right, Direction.Up },
        { Direction.Up,     Direction.Right },
        { Direction.Left,  Direction.Down },
        { Direction.Down,   Direction.Left }
    };
    Dictionary<Direction, Direction> MirrorMinus45NextDirection = new Dictionary<Direction, Direction>
    {
        { Direction.Right, Direction.Down },
        { Direction.Up,     Direction.Left },
        { Direction.Left,  Direction.Up},
        { Direction.Down,   Direction.Right }
    };
    private int _rows = 0;
    private int _columns = 0;

    public int Rows
    {
       get { return _rows; }
    }

    public int Columns
    {
        get { return _columns; }
    }
    private char[,] _map { get; set; } = new char[0, 0];
    private List<Beam> _activeBeams { get; set; } = new List<Beam>();
    private List<Beam> _passiveBeams { get; set; } = new List<Beam>();

    public void InitializeMap(string filePath)
    {
        var fileLines = File.ReadAllLines(filePath).ToList();
        _rows = fileLines.Count;
        _columns = fileLines.First().Length;
        _map = new char[_rows, _columns];
        fileLines.Select((line, row) => line.ToCharArray().Select((c, column) => new { c, row, column })).SelectMany(i => i)
        .ToList().ForEach(i => _map[i.row, i.column] = i.c);     
    }

    public void CalculateBeamPaths(Beam startingBeam)
    {
        _activeBeams.Clear();
        _passiveBeams.Clear();
        _activeBeams.Add(startingBeam);
        while(_activeBeams.Any()) 
        {
            CalculateNextLocation(_activeBeams.First());
        }
    }

    public int GetVisitedTileCount()
    {
        var visited = _passiveBeams
            .Select(pb => pb.BeamPath)
            .SelectMany(i => i).Select(b => b.Location)
            .Distinct(new LocationComparer());

        return visited.Count();
    }

    private void CalculateNextLocation(Beam activeBeam)
    {
        var activeBeamDirection = activeBeam.CurrentBeamLocation.Direction;
        var activeBeamLocation = activeBeam.CurrentBeamLocation.Location;
        var locDiff= LocationDifference[activeBeam.CurrentBeamLocation.Direction];
        var nextLocation = locDiff + activeBeamLocation;
        if (nextLocation.Row < 0 || nextLocation.Row >= _rows || nextLocation.Column < 0 || nextLocation.Column >= _columns) 
        {
            _passiveBeams.Add(activeBeam);
            _activeBeams.Remove(activeBeam);
            return;
        }

        var nextTile = _map[nextLocation.Row, nextLocation.Column];
        if (nextTile == HORIZONTAL_SPLITTER)
        {
            if (activeBeamDirection == Direction.Up || activeBeamDirection == Direction.Down)
            {
                activeBeam.CurrentBeamLocation.Direction = Direction.Left;
                var newBeamLocation = new BeamLocation(nextLocation, Direction.Right);
                var newBeam = new Beam(newBeamLocation);
                newBeam.AddToBeamPath();
                _activeBeams.Add(newBeam);
            }
        } else if (nextTile == VERTICAL_SPLITTER)
        {
            if (activeBeamDirection == Direction.Right || activeBeamDirection == Direction.Left)
            {
                activeBeam.CurrentBeamLocation.Direction = Direction.Up;
                var newBeamLocation = new BeamLocation(nextLocation, Direction.Down);          
                var newBeam = new Beam(newBeamLocation);
                newBeam.AddToBeamPath();
                _activeBeams.Add(newBeam);
            }
        }
        else if (nextTile == MIRROR_PLUS_45)
        {
            activeBeam.CurrentBeamLocation.Direction = MirrorPlus45NextDirection[activeBeamDirection];
        }
        else if (nextTile == MIRROR_MINUS_45)
        {
            activeBeam.CurrentBeamLocation.Direction = MirrorMinus45NextDirection[activeBeamDirection];
        }

        activeBeam.CurrentBeamLocation.Location = nextLocation;
        UpdateBeamStatus(activeBeam);
        activeBeam.AddToBeamPath();
    }

    private void UpdateBeamStatus(Beam activeBeam)
    {
        var oldBeamLocations = _activeBeams.Concat(_passiveBeams).Select(b => b.BeamPath)
            .SelectMany(b => b).Distinct(new BeamLocationComparer()).ToList();
        if (oldBeamLocations.Any(obl => activeBeam.CurrentBeamLocation.Equals(obl)))
        {
            _passiveBeams.Add(activeBeam);
            _activeBeams.Remove(activeBeam);
        }
    }
}

public class Location
{
    public Location(int row, int column)
    {
        Row = row;
        Column = column;
    }
    public int Row { get; set; }
    public int Column { get; set; }
    public static Location operator +(Location a, Location b)
        => new Location(a.Row + b.Row,  a.Column + b.Column);
}

class LocationComparer : IEqualityComparer<Location>
{
    public bool Equals(Location? l1, Location? l2)
    {
        if (ReferenceEquals(l1, l2))
            return true;

        if (l2 is null || l1 is null)
            return false;

        return l1.Row == l2.Row
            && l1.Column == l2.Column;
    }

    public int GetHashCode(Location bl) => bl.Row ^ bl.Column;
}

public class BeamLocation : IEquatable<BeamLocation>
{
    public BeamLocation(Location location, Direction direction)
    {
        Location = location;
        Direction = direction;
    }
    public Location Location { get; set; } = new Location(-1, -1);
    public Direction Direction { get; set; }

    public bool Equals(BeamLocation? other)
    {
        if (other is null) 
            return false;

        if (ReferenceEquals(this, other))
            return true;

        // If run-time types are not exactly the same, return false.
        if (this.GetType() != other.GetType())
        {
            return false;
        }

        return this.Location.Row == other.Location.Row
            && this.Location.Column == other.Location.Column
            && this.Direction == other.Direction;
    }
}

class BeamLocationComparer : IEqualityComparer<BeamLocation>
{
    public bool Equals(BeamLocation? l1, BeamLocation? l2)
    {
        if (ReferenceEquals(l1, l2))
            return true;

        if (l2 is null || l1 is null || l2.Location is null | l1.Location is null)
            return false;

        return l1.Location.Row == l2.Location.Row
            && l1.Location.Column == l2.Location.Column
            && l1.Direction == l2.Direction;
    }

    public int GetHashCode(BeamLocation bl) => bl.Location.Row ^ bl.Location.Column ^ (int)bl.Direction;
}

public class Beam
{
    public Beam(BeamLocation beamLocation)
    {
        CurrentBeamLocation = beamLocation;
    }
    public BeamLocation CurrentBeamLocation { get; set; } = new BeamLocation(new Location(-1, -1), Direction.Right);
    public List<BeamLocation> BeamPath { get; set; } = new List<BeamLocation>();

    public void AddToBeamPath()
    {
        BeamPath.Add(new BeamLocation(CurrentBeamLocation.Location, CurrentBeamLocation.Direction));
    }
}

