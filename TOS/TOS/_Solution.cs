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