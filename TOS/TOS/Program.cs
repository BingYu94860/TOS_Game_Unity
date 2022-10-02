using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

enum Direction : short { Right, RightDown, Down, LeftDown, Left, LeftUp, Up, RightUp, Stop };

class DirectionMf {
    public static Direction[] ModeDir4 = 
    {
        Direction.Right,
        Direction.Down,
        Direction.Left,
        Direction.Up
    };
    public static Direction[] ModeDir8 =
    {
        Direction.Right,
        Direction.RightDown,
        Direction.Down,
        Direction.LeftDown,
        Direction.Left,
        Direction.LeftUp,
        Direction.Up,
        Direction.RightUp
    };
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
    #region 成員
    public short row; // y
    public short col; // x
    #endregion

    #region 建構子
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
    #endregion

    #region 常數
    public static Position2 Zero => new(0, 0);
    public static Position2 Right => new(Direction.Right);
    public static Position2 RightDown => new(Direction.RightDown);
    public static Position2 Down => new(Direction.Down);
    public static Position2 LeftDown => new(Direction.LeftDown);
    public static Position2 Left => new(Direction.Left);
    public static Position2 LeftUp => new(Direction.LeftUp);
    public static Position2 Up => new(Direction.Up);
    public static Position2 RightUp => new(Direction.RightUp);
    #endregion

    #region 運算操作
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
    public static bool operator >=(Position2 left, Position2 right) => left.row >= right.row && left.col >= right.col;
    public static bool operator <=(Position2 left, Position2 right) => left.row <= right.row && left.col <= right.col;
    public static bool operator >(Position2 left, Position2 right) => left.row > right.row && left.col > right.col;
    public static bool operator <(Position2 left, Position2 right) => left.row < right.row && left.col < right.col;
    public static bool operator ==(Position2 left, Position2 right) => left.row == right.row && left.col == right.col;
    public static bool operator !=(Position2 left, Position2 right) => left.row != right.row || left.col != right.col;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is not null && this == (Position2)obj;
    public override int GetHashCode() => HashCode.Combine(row, col);
    #endregion

    public static bool InBox(Position2 leftDown, Position2 pos, Position2 rightUp) => pos >= leftDown && pos < rightUp;

    public override string ToString() => $"(r:{row}, c:{col})";
}

struct Path
{
    #region 成員
    public Direction[] dirs;
    #endregion

    #region 建構子
    public Path() => dirs = Array.Empty<Direction>();
    public Path(Direction dir) => dirs = new Direction[] { dir };
    #endregion

    #region 常數
    public static Path Empty => new();
    #endregion

    #region 運算操作
    public static Path operator +(Path left, Direction right)
    {
        Path path = new() { dirs = new Direction[left.dirs.Length + 1] };
        Array.Copy(left.dirs, path.dirs, left.dirs.Length);
        path.dirs[^1] = right;
        return path;
    }
    #endregion

    public override string ToString()
    {
        var str = "";
        if (dirs.Length > 0)
            foreach (var dir in dirs)
                str += DirectionMf.ToString(dir);
        return str;
    }
}
//----------------------------------------------------------------------------------------------------------
class TurnPath
{
    #region 成員
    protected Position2 pos;
    protected Path path;
    #endregion

    #region 建構子
    public TurnPath()
    {
        pos = new Position2();
        path = new Path();
    }
    public TurnPath(int row, int col)
    {
        pos = new Position2(row, col);
        path = new Path();
    }
    #endregion

    public virtual void MoveDir(Direction dir)
    {
        pos += dir;
        path += dir;
    }
    public override string ToString() => $"(r:{pos.row}, c:{pos.col}) p:{path}\n";
}
enum BeadType : short { R, B, G, L, D, H, Null };

class BeadTypeMf
{
    public static string ToString(BeadType type) => type switch
    {
        BeadType.R => "R",
        BeadType.B => "B",
        BeadType.G => "G",
        BeadType.L => "L",
        BeadType.D => "D",
        BeadType.H => "H",
        _ => "_"
    };
}

class TurnBoard: TurnPath
{
    #region 靜態常數
    public static int ROWS = 5; // y
    public static int COLS = 6; // x
    #endregion

    #region 成員
    protected BeadType[,] board;
    #endregion

    #region 建構子
    public TurnBoard() : base() => board = new BeadType[ROWS, COLS];
    public TurnBoard(int row, int col) : base(row, col) => board = new BeadType[ROWS, COLS];
    #endregion

    public void RandomBoard()
    {
        var randomObject = new Random();
        for (var ir = 0; ir < ROWS; ir++)
            for (var ic = 0; ic < COLS; ic++)
                board[ir, ic] = (BeadType)randomObject.Next(0, (int)BeadType.Null);
    }

    public void SetPosition(int row, int col)
    {
        pos.row = (short)row;
        pos.col = (short)col;
    }

    public bool CanMoveDir(Direction dir) => Position2.InBox(Position2.Zero, pos + dir, new Position2(ROWS, COLS));

    public override void MoveDir(Direction dir)
    {
        var odd = pos;
        base.MoveDir(dir);
        (board[odd.row, odd.col], board[pos.row, pos.col]) = (board[pos.row, pos.col], board[odd.row, odd.col]);
    }

    public override string ToString()
    {
        string str = $"(r:{pos.row}, c:{pos.col}) p:{path}\n";
        for (var ir = ROWS - 1; ir >= 0; ir--)
        {
            for (var ic = 0; ic < COLS; ic++)
                str += BeadTypeMf.ToString(board[ir, ic]);
            str += "\n";
        }
        return str;
    }
}

class Solution: TurnBoard
{
    //public TurnBoard init;

    public float weight;
    public short bestPathLength;
    public float beforeBestWeight;

    #region 建構式
    public Solution() : base()
    {
        weight = 0f;
        bestPathLength = 0;
        beforeBestWeight = 0f;
    }

    /*
    public Solution(BeadType[,] init_board, int row, int col)
    {
        init = new TurnBoard(ref init_board, row, col);
        turn = init.Copy();
        path = Array.Empty<Direction>();//(空)
        weight = 0f;
        bestPathLength = 0;
        beforeBestWeight = 0f;
    }
    public Solution(Solution solution)
    {
        init = solution.init;
        turn = solution.turn.Copy();
        path = solution.path;
        weight = 0f;
        bestPathLength = solution.bestPathLength;
        beforeBestWeight = solution.beforeBestWeight;
    }
    */
    #endregion

    #region 判斷是否是可以移動的方向 還有限制條件
    /*
    public bool CanMoveTurnDir(Direction dir)
    {
        //不為空&&不能反向
        if (path.Length > 0 && path[path.Length - 1] == DirectionMf.Inverse(dir))
            return false;
        //不能超出邊界
        if (!turn.CanMoveDir(dir))
            return false;
        //不能與過去狀態相同
        var nextBoard = turn.Copy();
        nextBoard.MoveBoardDir(dir);
        var beforeBoard = turn.Copy();
        for (var i = path.Length - 1; i >= 0; i--)
        {
            var idir = path[i];
            beforeBoard.MoveBoardDir(DirectionMf.Inverse(idir));
            if (beforeBoard.Equal(nextBoard))
                return false;
        }
        return true;
    }
    */
    #endregion

    #region 移動1個方向
    /*
    public void MoveTurnDir(Direction dir)
    {
        turn.MoveDir(dir);
        //path add
        var new_path = new Direction[path.Length + 1];
        Array.Copy(path, new_path, path.Length);
        path = new_path;
        path[^1] = dir;//add
    }
    */
    #endregion

    #region 計算解的評分
    public void Evaluate() { }
    #endregion

    #region 產生下1步的解
    /*
    public List<Solution> NextStepSolutions()
    {
        var solutions = new List<Solution>();
        foreach (var dir in DirectionMf.ModeDir4)
        {
            if (CanMoveTurnDir(dir))
            {
                var solution = new Solution(this);
                solution.MoveTurnDir(dir);
                solution.Evaluate();
                if (solution.weight > beforeBestWeight)
                {
                    solution.beforeBestWeight = solution.weight;
                    solution.bestPathLength = (sbyte)solution.path.Length;
                }
                solutions.Add(solution);
            }
        }
        return solutions;
    }
    */
    #endregion

    #region 得到路徑字串
    /*
    public String GetPathString()
    {
        if (path.Length <= 0)
            return "Empty";
        var pathString = "";
        foreach (var dir in path)
            pathString += DirectionMf.ToString(dir);
        return pathString;
    }
    */
    #endregion
}

class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        TurnBoard turnBoard = new();
        turnBoard.RandomBoard();
        turnBoard.SetPosition(0, 1);
        Console.WriteLine(turnBoard);

        //var path = Path.Empty + Direction.Right + Direction.Right + Direction.Right + Direction.Right + Direction.Right;

        Direction[] dirs =
        {
            Direction.Right,
            Direction.Left,
            Direction.Right,
            Direction.Up,
            Direction.Right
        };

        foreach (var dir in dirs)
        {
            if (turnBoard.CanMoveDir(dir))
                turnBoard.MoveDir(dir);
            Console.WriteLine(turnBoard);
        }
    }
}