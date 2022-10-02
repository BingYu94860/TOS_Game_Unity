/*
using System;
using System.Collections.Generic;


class Compute
{
    public const int ROWS = TOS.ROWS;
    public const int COLS = TOS.COLS;
    public const int removeCount = 3;
    public const float MULTI_ORB_BONUS = 0.25f;
    public const float COMBO_BONUS = 0.25f;
    public static Dictionary<BeadType, int> weights = BeadMf.weights;
    public static (List<(BeadType type, int count)> combos, float score) Result(BeadType[,] board)
    {
        //tempBoard is all TYPE.X
        var tempBoard = new BeadType[ROWS, COLS];
        for (var i = 0; i < ROWS; i++)
            for (var j = 0; j < COLS; j++)
                tempBoard[i, j] = BeadType.Null;
        //
        for (var i = 0; i < ROWS; i++)
        {
            BeadType prev_1_orb = BeadType.Null, prev_2_orb = BeadType.Null;
            for (int j = 0; j < COLS; j++)
            {
                var cur_orb = board[i, j];
                if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != BeadType.Null)
                {
                    tempBoard[i, j] = tempBoard[i, j - 1] = tempBoard[i, j - 2] = cur_orb;
                }
                prev_1_orb = prev_2_orb;
                prev_2_orb = cur_orb;
            }
        }
        for (var j = 0; j < COLS; j++)
        {
            BeadType prev_1_orb = BeadType.Null, prev_2_orb = BeadType.Null;
            for (var i = 0; i < ROWS; i++)
            {
                var cur_orb = board[i, j];
                if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != BeadType.Null)
                {
                    tempBoard[i, j] = tempBoard[i - 1, j] = tempBoard[i - 2, j] = cur_orb;
                }
                prev_1_orb = prev_2_orb;
                prev_2_orb = cur_orb;
            }
        }
        //洪水法
        var combos = new List<(BeadType type, int count)>();
        for (var i = 0; i < ROWS; i++)
        {
            for (var j = 0; j < COLS; j++)
            {
                var cur_orb = tempBoard[i, j];
                if (cur_orb == BeadType.Null)
                    continue;
                var stack = new Stack<(int row, int col)>();
                stack.Push((i, j));
                var count = 0;
                while (stack.Count != 0)
                {
                    var (row, col) = stack.Pop();
                    if (tempBoard[row, col] != cur_orb)
                        continue;
                    count++;
                    tempBoard[row, col] = BeadType.Null;
                    if (row < ROWS - 1)//↑
                        stack.Push((row + 1, col));
                    if (row > 0)//↓
                        stack.Push((row - 1, col));
                    if (col > 0)//←
                        stack.Push((row, col - 1));
                    if (col < COLS - 1)//→
                        stack.Push((row, col + 1));
                }
                combos.Add((cur_orb, count));
            }
        }
        //計分
        var score = 0f;
        foreach (var (type, count) in combos)
            score += weights[type] * (1.0f + (count - removeCount) * MULTI_ORB_BONUS);
        score *= (1f + ((combos.Count - 1f) * COMBO_BONUS));
        return (combos, score);
    }

    public static float GetMaxScore(BeadType[,] board)
    {
        var countDict = new Dictionary<BeadType, int>();
        foreach (BeadType item in Enum.GetValues(typeof(BeadType)))
            countDict.Add(item, 0);
        foreach (var item in board)
            countDict[item]++;
        var comboSum = 0;
        var maxScore = 0f;
        foreach (var item in countDict)
        {
            var type = item.Key;
            var count = item.Value;
            if (count >= removeCount)
            {
                var comboNum = count / removeCount;
                var multiOrb = count % removeCount;
                comboSum += comboNum;
                maxScore += weights[type] * (comboNum + multiOrb * MULTI_ORB_BONUS);
            }
        }
        maxScore *= (1f + ((comboSum - 1f) * COMBO_BONUS));
        return maxScore;
    }
}

*/