using System;


class TureBoard
{
    #region 主要欄位 版面和起始位置
    public BeadType[,] board;
    public sbyte row, col;
    #endregion

    #region 建構式
    public TureBoard(ref BeadType[,] board, int row, int col)
    {
        this.board = board;
        this.row = (sbyte)row;
        this.col = (sbyte)col;
    }
    public TureBoard(BeadType[,] board, int row, int col)
    {
        this.board = (BeadType[,])board.Clone();
        this.row = (sbyte)row;
        this.col = (sbyte)col;
    }
    public TureBoard Copy() => new TureBoard(board, row, col);
    #endregion

    #region 判斷是否是可移動方向
    public bool CanMoveDir(DIR dir) => DirectionMf.CanMoveDir(dir, row, col, TOS.ROWS, TOS.COLS);
    #endregion

    #region 移動1個方向
    public void MoveBoardDir(DIR dir)
    {
        var (r, c) = DirectionMf.MoveBoardDir(dir, row, col, ref board);
        row = (sbyte)r;
        col = (sbyte)c;
    }
    #endregion

    #region 判斷是否相等
    public bool Equal(TureBoard trueBoard)
    {
        //cursor
        if (trueBoard.row != row || trueBoard.col != col) return false;
        //board
        for (var i = 0; i < TOS.ROWS; i++)
            for (var j = 0; j < TOS.COLS; j++)
                if (board[i, j] != trueBoard.board[i, j])
                    return false;
        return true;
    }
    #endregion
}

