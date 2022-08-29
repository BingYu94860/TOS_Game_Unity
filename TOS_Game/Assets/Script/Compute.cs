using System;
using System.Collections.Generic;

namespace Assets.Script
{
    class Compute
    {
        public const int ROWS = TOS.ROWS;
        public const int COLS = TOS.COLS;
        public const int removeCount = 3;
        public const float MULTI_ORB_BONUS = 0.25f;
        public const float COMBO_BONUS = 0.25f;
        public static Dictionary<OrbsType, int> weights = new Dictionary<OrbsType, int>
        {
            [OrbsType.R] = 100,
            [OrbsType.G] = 100,
            [OrbsType.B] = 100,
            [OrbsType.D] = 100,
            [OrbsType.L] = 100,
            [OrbsType.H] = 100,
            [OrbsType.Null] = 0
        };
        public static (List<(OrbsType type, int count)> combos, float score) Result(OrbsType[,] board)
        {
            //tempBoard is all TYPE.X
            var tempBoard = new OrbsType[ROWS, COLS];
            for (var i = 0; i < ROWS; i++)
                for (var j = 0; j < COLS; j++)
                    tempBoard[i, j] = OrbsType.Null;
            //
            for (var i = 0; i < ROWS; i++)
            {
                OrbsType prev_1_orb = OrbsType.Null, prev_2_orb = OrbsType.Null;
                for (int j = 0; j < COLS; j++)
                {
                    var cur_orb = board[i, j];
                    if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != OrbsType.Null)
                    {
                        tempBoard[i, j] = tempBoard[i, j - 1] = tempBoard[i, j - 2] = cur_orb;
                    }
                    prev_1_orb = prev_2_orb;
                    prev_2_orb = cur_orb;
                }
            }
            for (var j = 0; j < COLS; j++)
            {
                OrbsType prev_1_orb = OrbsType.Null, prev_2_orb = OrbsType.Null;
                for (var i = 0; i < ROWS; i++)
                {
                    var cur_orb = board[i, j];
                    if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != OrbsType.Null)
                    {
                        tempBoard[i, j] = tempBoard[i - 1, j] = tempBoard[i - 2, j] = cur_orb;
                    }
                    prev_1_orb = prev_2_orb;
                    prev_2_orb = cur_orb;
                }
            }
            //洪水法
            var combos = new List<(OrbsType type, int count)>();
            for (var i = 0; i < ROWS; i++)
            {
                for (var j = 0; j < COLS; j++)
                {
                    var cur_orb = tempBoard[i, j];
                    if (cur_orb == OrbsType.Null)
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
                        tempBoard[row, col] = OrbsType.Null;
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

        public static float GetMaxScore(OrbsType[,] board)
        {
            var countDict = new Dictionary<OrbsType, int>();
            foreach (OrbsType item in Enum.GetValues(typeof(OrbsType)))
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
}
