
public enum DIR : sbyte { RIGHT, RIGHT_DOWN, DOWN, LEFT_DOWN, LEFT, LEFT_UP, UP, RIGHT_UP, STOP };
class DirectionMf
{
    public static DIR[] ModeDir4 = { DIR.RIGHT, DIR.DOWN, DIR.LEFT, DIR.UP };
    public static DIR[] ModeDir8 = { DIR.RIGHT, DIR.RIGHT_DOWN, DIR.DOWN, DIR.LEFT_DOWN, DIR.LEFT, DIR.LEFT_UP, DIR.UP, DIR.RIGHT_UP };

    #region 判斷是否可以移動
    public static bool CanMoveDir(DIR dir, int row, int col, int ROWS, int COLS)
    {
        switch (dir)
        {
            case DIR.RIGHT: return col < COLS - 1;
            case DIR.RIGHT_UP: return row < ROWS - 1 && col < COLS - 1;
            case DIR.UP: return row < ROWS - 1;
            case DIR.LEFT_UP: return row < ROWS - 1 && col > 0;
            case DIR.LEFT: return col > 0;
            case DIR.LEFT_DOWN: return row > 0 && col > 0;
            case DIR.DOWN: return row > 0;
            case DIR.RIGHT_DOWN: return row > 0 && col < COLS - 1;
            default: return false;
        }
    }
    #endregion

    #region 移動1個方向
    public static (int r, int c) MoveBoardDir(DIR dir, int row, int col, ref BeadType[,] board)
    {
        var (r, c) = MoveDir(dir, row, col);
        var temp = board[row, col];
        board[row, col] = board[r, c];
        board[r, c] = temp;
        return (r, c);
    }
    public static (int r, int c) MoveDir(DIR dir, int row, int col)
    {
        switch (dir)
        {
            case DIR.RIGHT: return (row, col + 1);
            case DIR.RIGHT_UP: return (row + 1, col + 1);
            case DIR.UP: return (row + 1, col);
            case DIR.LEFT_UP: return (row + 1, col - 1);
            case DIR.LEFT: return (row, col - 1);
            case DIR.LEFT_DOWN: return (row - 1, col - 1);
            case DIR.DOWN: return (row - 1, col);
            case DIR.RIGHT_DOWN: return (row - 1, col + 1);
            default: return (row, col);
        }
    }
    #endregion



    public static DIR DirInv(DIR dir)
    {
        switch (dir)
        {
            case DIR.RIGHT: return DIR.LEFT;
            case DIR.RIGHT_DOWN: return DIR.LEFT_UP;
            case DIR.DOWN: return DIR.UP;
            case DIR.LEFT_DOWN: return DIR.RIGHT_UP;
            case DIR.LEFT: return DIR.RIGHT;
            case DIR.LEFT_UP: return DIR.RIGHT_DOWN;
            case DIR.UP: return DIR.DOWN;
            case DIR.RIGHT_UP: return DIR.LEFT_DOWN;
            default: return DIR.STOP;
        }
    }

    public static string DirStr(DIR dir)
    {
        switch (dir)
        {
            case DIR.RIGHT: return "→";
            case DIR.RIGHT_DOWN: return "↘";
            case DIR.DOWN: return "↓";
            case DIR.LEFT_DOWN: return "↙";
            case DIR.LEFT: return "←";
            case DIR.LEFT_UP: return "↖";
            case DIR.UP: return "↑";
            case DIR.RIGHT_UP: return "↗";
            default: return "";
        }
    }
}

