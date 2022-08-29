using System;
using System.Collections.Generic;

namespace Assets.Script
{
    class Solution
    {
        public static DIR[] ModeDir4 = { DIR.RIGHT, DIR.DOWN, DIR.LEFT, DIR.UP };
        public static DIR[] ModeDir8 = { DIR.RIGHT, DIR.RIGHT_DOWN, DIR.DOWN, DIR.LEFT_DOWN, DIR.LEFT, DIR.LEFT_UP, DIR.UP, DIR.RIGHT_UP };
        public TureBoard init;
        public TureBoard turn;
        public DIR[] path;
        public float weight;
        public sbyte bestPathLength;
        public float beforeBestWeight;
        public Solution(OrbsType[,] init_board, int row, int col)
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
        public bool CanMoveTurnDir(DIR dir)
        {
            //不為空&&不能反向
            if (path.Length > 0 && path[path.Length - 1] == DirInv(dir))
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
                beforeBoard.MoveBoardDir(DirInv(idir));
                if (beforeBoard.Equal(nextBoard))
                    return false;
            }
            return true;
        }
        public void MoveTurnDir(DIR dir)
        {
            turn.MoveBoardDir(dir);
            //path add
            var new_path = new DIR[path.Length + 1];
            Array.Copy(path, new_path, path.Length);
            path = new_path;
            path[path.Length - 1] = dir;//add
        }
        public void Evaluate()
        {
            var (_, score) = Compute.Result(turn.board);
            weight = score;
        }
        public List<Solution> NextStepSolutions()
        {
            var solutions = new List<Solution>();
            foreach (var dir in ModeDir4)
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
        private DIR DirInv(DIR dir)
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
        private String DirStr(DIR dir)
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
        public String GetPathString()
        {
            if (path.Length <= 0)
                return "Empty";
            var pathString = "";
            foreach (var dir in path)
                pathString += DirStr(dir);
            return pathString;
        }
    }
}
