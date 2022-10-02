using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

enum Direction : short { Right, RightDown, Down, LeftDown, Left, LeftUp, Up, RightUp, Stop };

class DirectionMf {
    public static short RowVector(Direction dir) => dir switch
    {   // Up : +1 // Down : -1
        Direction.LeftUp => 1,
        Direction.Up => 1,
        Direction.RightUp => 1,
        Direction.RightDown => -1,
        Direction.Down => -1,
        Direction.LeftDown => -1,
        _ => 0
    };
    public static short ColVector(Direction dir) => dir switch
    {   // Right : +1 // Left : -1
        Direction.Right => 1,
        Direction.RightDown => 1,
        Direction.RightUp => 1,
        Direction.LeftDown => -1,
        Direction.Left => -1,
        Direction.LeftUp => -1,
        _ => 0
    };
    public static Direction Inverse(Direction dir) => dir switch
    {
        Direction.Right => Direction.Left,
        Direction.Left => Direction.Right,
        Direction.RightDown => Direction.LeftUp,
        Direction.LeftUp => Direction.RightDown,
        Direction.Down => Direction.Up,
        Direction.Up => Direction.Up,
        Direction.LeftDown => Direction.RightUp,
        Direction.RightUp => Direction.LeftDown,
        _ => dir
    };
    public static string ToString(Direction dir) => dir switch
    {
        Direction.Right => "→",
        Direction.RightDown => "↘",
        Direction.Down => "↓",
        Direction.LeftDown => "↙",
        Direction.Left => "←",
        Direction.LeftUp => "↖",
        Direction.Up => "↑",
        Direction.RightUp => "↗",
        _ => "_"
    };
    public static Direction ToDirection(string dirStr) => dirStr switch
    {
        "→" => Direction.Right,
        "↘" => Direction.RightDown,
        "↓" => Direction.Down,
        "↙" => Direction.LeftDown,
        "←" => Direction.Left,
        "↖" => Direction.LeftUp,
        "↑" => Direction.Up,
        "↗" => Direction.RightUp,
        _ => Direction.Stop
    };
}

struct Position2
{
    public short row; // y
    public short col; // x
    public Position2()
    {
        row = 0;
        col = 0;
    }
    public Position2(int row, int col)
    {
        this.row = (short)row;
        this.col = (short)col;
    }
    public Position2(Direction dir)
    {
        row = DirectionMf.RowVector(dir);
        col = DirectionMf.ColVector(dir);
    }
    public static Position2 Zero => new(0, 0);
    public static Position2 Right => new(Direction.Right);
    public static Position2 RightDown => new(Direction.RightDown);
    public static Position2 Down => new(Direction.Down);
    public static Position2 LeftDown => new(Direction.LeftDown);
    public static Position2 Left => new(Direction.Left);
    public static Position2 LeftUp => new(Direction.LeftUp);
    public static Position2 Up => new(Direction.Up);
    public static Position2 RightUp => new(Direction.RightUp);
    public static Position2 operator +(Position2 left, Position2 right) => new()
    {
        row = (short)(left.row + right.row),
        col = (short)(left.col + right.col)
    };
    public static Position2 operator -(Position2 left, Position2 right) => new()
    {
        row = (short)(left.row - right.row),
        col = (short)(left.col - right.col)
    };
    public static Position2 operator +(Position2 left, Direction right) => new()
    {
        row = (short)(left.row + DirectionMf.RowVector(right)),
        col = (short)(left.col + DirectionMf.ColVector(right))
    };
    public static Position2 operator -(Position2 left, Direction right) => new()
    {
        row = (short)(left.row - DirectionMf.RowVector(right)),
        col = (short)(left.col - DirectionMf.ColVector(right))
    };
    public override string ToString() => $"(r:{row}, c:{col})";
    public static bool InBox(Position2 leftDown, Position2 pos, Position2 rightUp) => pos >= leftDown && pos < rightUp;
    public static bool operator >=(Position2 left, Position2 right) => left.row >= right.row && left.col >= right.col;
    public static bool operator <=(Position2 left, Position2 right) => left.row <= right.row && left.col <= right.col;
    public static bool operator >(Position2 left, Position2 right) => left.row > right.row && left.col > right.col;
    public static bool operator <(Position2 left, Position2 right) => left.row < right.row && left.col < right.col;
    public static bool operator ==(Position2 left, Position2 right) => left.row == right.row && left.col == right.col;
    public static bool operator !=(Position2 left, Position2 right) => left.row != right.row || left.col != right.col;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is not null && this == (Position2)obj;
    public override int GetHashCode() => HashCode.Combine(row, col);
}

//----------------------------------------------------------------------------------------------------------

struct Path
{
    public List<Direction> directions;
    public Path() => directions = new List<Direction>();
    public Path(List<Direction> directions) => this.directions = new List<Direction>(directions); //淺複製

    public static Path operator +(Path left, Direction right)
    {
        left.directions.Add(right);
        return left;
    }
    public override string ToString()
    {
        var str = "";
        if (directions.Count > 0)
            foreach (var dir in directions)
                str += DirectionMf.ToString(dir);
        return str;
    }
}

class Solution
{
    public Position2 position;
    public Solution()
    {
        position = new Position2();
    }
}

struct PathDirection
{
    public Direction[]? dirs;
    public PathDirection() => dirs = null;
    public PathDirection(Direction dir) => dirs = new Direction[] { dir };
    public static PathDirection operator +(PathDirection left, Direction right)
    {
        PathDirection pathDirection;
        if (left.dirs == null)
        {
            pathDirection = new PathDirection(right);
            return pathDirection;
        }
        else
        {
            pathDirection = new PathDirection() { dirs = new Direction[left.dirs.Length + 1] };
            Array.Copy(left.dirs, pathDirection.dirs, left.dirs.Length);
            pathDirection.dirs[^1] = right;
            return pathDirection;
        }
    }
    public override string ToString()
    {
        
        if (dirs != null)
        {
            string v = "";
            foreach (var dir in dirs)
                v += DirectionMf.ToString(dir);
            return v;
        }
        else return "Path Directions is Null.";
        
    }
}

enum BeadType : sbyte { R, B, G, L, D, H, Null };

class TureBoard
{
    static readonly int ROWS = 5; // y
    static readonly int COLS = 6; // x
    private Position2 pos;
    private readonly BeadType[,] board;
    public TureBoard()
    {
        pos = new Position2(0, 0);
        board = new BeadType[ROWS, COLS];
    }
    public TureBoard(Position2 pos)
    {
        this.pos = pos;
        board = new BeadType[ROWS, COLS];
    }
    public TureBoard(Position2 pos, BeadType[,] board)
    {
        this.pos = pos;
        this.board = (BeadType[,])board.Clone();
    }
    public void RandomBoard()
    {
        var randomObject = new Random();
        for (var ir = 0; ir < ROWS; ir++)
            for (var ic = 0; ic < COLS; ic++)
                board[ir, ic] = (BeadType)randomObject.Next(0, (int)BeadType.Null);
    }
    public void MoveBoardDir(Direction dir)
    {
        var pos2 = pos + dir;
        (board[pos2.row, pos2.col], board[pos.row, pos.col]) = (board[pos.row, pos.col], board[pos2.row, pos2.col]);
        pos = pos2;
    }
    public bool CanMoveBoardDir(Direction dir) => Position2.InBox(Position2.Zero, pos + dir, new Position2(ROWS, COLS));
    public bool Equal(TureBoard trueBoard)
    {
        //cursor
        if (trueBoard.pos != pos) return false;
        //board
        for (var i = 0; i < ROWS; i++)
            for (var j = 0; j < COLS; j++)
                if (board[i, j] != trueBoard.board[i, j])
                    return false;
        return true;
    }
    public override string ToString()
    {
        string v = $"(r:{pos.row}, c:{pos.col})\n";
        for (var ir = ROWS - 1; ir >= 0; ir--)
        {
            for (var ic = 0; ic < COLS; ic++)
            {
                v += board[ir, ic] switch
                {
                    BeadType.R => "R",
                    BeadType.B => "B",
                    BeadType.G => "G",
                    BeadType.L => "L",
                    BeadType.D => "D",
                    BeadType.H => "H",
                    _ => "_",
                };
            }
            v += "\n";
        }
        return v;
    }
}


class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine(Position2.Zero);
        Console.WriteLine("Hello, World!");

        var pp = new Path();
        Console.WriteLine(pp);
        pp += Direction.Right;
        Console.WriteLine(pp);

        var path = new PathDirection();
        Console.WriteLine(path);
        path += Direction.Up;
        Console.WriteLine(path);
        path += Direction.Down;
        Console.WriteLine(path);

        //var board = new BeadType[6, 5];
        
        var x = new Position2(5, 1) + Position2.Up;
        var p = new Position2(0, 0);
        var q = new Position2(5, 6);

        var y = p + q;
        Console.WriteLine(y);
        Console.WriteLine(y + Direction.Left);
        Console.WriteLine(x <= q);
        

        var tureBoard = new TureBoard();
        tureBoard.RandomBoard();
        var dir = Direction.Up;
        Console.WriteLine(tureBoard);
        Console.WriteLine(tureBoard.CanMoveBoardDir(dir));
        if (tureBoard.CanMoveBoardDir(dir))
            tureBoard.MoveBoardDir(dir);
        Console.WriteLine(tureBoard);


        var s = new Vector2();
    }
}






//Console.WriteLine(TOS.ROWS);


// Board

/*
 Board

 row : int // y = 0 ~ 4
 col : int // x = 0 ~ 5
 BeadType[,]
 
 
 */