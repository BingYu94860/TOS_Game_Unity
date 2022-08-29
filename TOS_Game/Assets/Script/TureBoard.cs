using System;

namespace Assets.Script
{
    class TureBoard
    {
        public const int ROWS = TOS.ROWS;
        public const int COLS = TOS.COLS;
        public readonly OrbsType[,] board;
        public sbyte row, col;
        public TureBoard(ref OrbsType[,] board, int row, int col)
        {
            this.board = board;
            this.row = (sbyte)row;
            this.col = (sbyte)col;
        }
        public TureBoard(OrbsType[,] board, int row, int col)
        {
            this.board = new OrbsType[ROWS, COLS];
            Array.Copy(board, this.board, board.Length);
            this.row = (sbyte)row;
            this.col = (sbyte)col;
        }
        public TureBoard Copy() => new TureBoard(board, row, col);
        public bool CanMoveDir(DIR dir)
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
        public void MoveDir(DIR dir)
        {
            switch (dir)
            {
                case DIR.RIGHT: col += 1; break;
                case DIR.RIGHT_UP: row += 1; col += 1; break;
                case DIR.UP: row += 1; break;
                case DIR.LEFT_UP: row += 1; col -= 1; break;
                case DIR.LEFT: col -= 1; break;
                case DIR.LEFT_DOWN: row -= 1; col -= 1; break;
                case DIR.DOWN: row -= 1; break;
                case DIR.RIGHT_DOWN: row -= 1; col += 1; break;
                default: break;
            }
        }
        public void MoveBoardDir(DIR dir)
        {
            var old = (row, col);
            MoveDir(dir);
            Swap(ref board[old.row, old.col], ref board[row, col]);
        }
        private void Swap(ref OrbsType a, ref OrbsType b)
        {
            var temp = a; a = b; b = temp;
        }
        public bool Equal(TureBoard trueBoard)
        {
            //cursor
            if (trueBoard.row != row || trueBoard.col != col)
                return false;
            //board
            for (var i = 0; i < ROWS; i++)
                for (var j = 0; j < COLS; j++)
                    if (board[i, j] != trueBoard.board[i, j])
                        return false;
            return true;
        }
    }
}
