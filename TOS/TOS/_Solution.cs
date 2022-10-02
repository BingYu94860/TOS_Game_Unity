/*
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
*/

/*
using System;
using System.Collections.Generic;


class Solution
{
    public TureBoard init;
    public TureBoard turn;
    public DIR[] path;
    public float weight;
    public sbyte bestPathLength;
    public float beforeBestWeight;

    #region 建構式
    public Solution(BeadType[,] init_board, int row, int col)
    {
        init = new TureBoard(ref init_board, row, col);
        turn = init.Copy();
        path = Array.Empty<DIR>();//(空)
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
    #endregion

    #region 判斷是否是可以移動的方向 還有限制條件
    public bool CanMoveTurnDir(DIR dir)
    {
        //不為空&&不能反向
        if (path.Length > 0 && path[path.Length - 1] == DirectionMf.DirInv(dir))
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
            beforeBoard.MoveBoardDir(DirectionMf.DirInv(idir));
            if (beforeBoard.Equal(nextBoard))
                return false;
        }
        return true;
    }
    #endregion

    #region 移動1個方向
    public void MoveTurnDir(DIR dir)
    {
        turn.MoveBoardDir(dir);
        //path add
        var new_path = new DIR[path.Length + 1];
        Array.Copy(path, new_path, path.Length);
        path = new_path;
        path[path.Length - 1] = dir;//add
    }
    #endregion

    #region 計算解的評分
    public void Evaluate()
    {
        var (_, score) = Compute.Result(turn.board);
        weight = score;
    }
    #endregion

    #region 產生下1步的解
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
    #endregion

    #region 得到路徑字串
    public String GetPathString()
    {
        if (path.Length <= 0)
            return "Empty";
        var pathString = "";
        foreach (var dir in path)
            pathString += DirectionMf.DirStr(dir);
        return pathString;
    }
    #endregion
}

*/