using System;
using System.Collections.Generic;

namespace Assets.Script
{
    /*
    class SolveBoard {
        public const int ROWS = TOS.ROWS;
        public const int COLS = TOS.COLS;
        public static int MAX_PATH = 16;       //最大搜尋步數
        public static int GROUP_SIZE = 1000;    //每代保留最佳解的個數限制
        public static int BEST_SIZE = 20;       //保留歷史最佳前20個
        //==========成員==============//--------------------------------------------------------------------------
        private readonly OrbsType[,] init_board;                               //初始版面
        public Solution bestSolution;                                       //最佳解
        public readonly List<Solution> bestSolutionGroup;                  //歷史最佳解族群
        public readonly List<Solution> bestSolutionGroupForEveryStep;      //每步最佳解族群
        private List<Solution> cursorGroup;                                 //原始組群
        private List<Solution> nextGroup;                                   //走下一步產生族群
        //==========建構式==============//--------------------------------------------------------------------------
        public SolveBoard(OrbsType[,] board) {
            init_board = board;
            bestSolutionGroup = new List<Solution>();
            bestSolutionGroupForEveryStep = new List<Solution>();
            cursorGroup = new List<Solution>();
            nextGroup = new List<Solution>();
            //計算初始版面解的分數
            bestSolution = new Solution(init_board, 0, 0);
            bestSolution.Evaluate();
            bestSolutionGroup.Add(bestSolution);
            bestSolutionGroupForEveryStep.Add(bestSolution);
            
        }//End of SolveBoard()
        //==========其他函數==============//--------------------------------------------------------------------------
        //更新歷史最佳前 BEST_SIZE 個解
        private void UpdateBestSolutions(List<Solution> group) {
            bestSolutionGroup.AddRange(group);
            bestSolutionGroup.Sort((s1, s2) => s2.weight.CompareTo(s1.weight));
            if (bestSolutionGroup.Count > BEST_SIZE)
                bestSolutionGroup.RemoveRange(BEST_SIZE, bestSolutionGroup.Count - BEST_SIZE);
            bestSolution = bestSolutionGroup[0];
        }
        //須執行計算
        public void Run(sbyte row = -1, sbyte col = -1) {
            if (row >= 0 && col >= 0 && row < ROWS && col < COLS) {
                var solution = new Solution(init_board, row, col);
                solution.Evaluate();
                cursorGroup.Add(solution);
            } else {//加入30個位置初始解族群
                for (var i = 0; i < ROWS; i++)
                    for (var j = 0; j < COLS; j++) {
                        var solution = new Solution(init_board, i, j);
                        solution.Evaluate();
                        cursorGroup.Add(solution);
                    }
            }
            //暴力貪婪 找尋 最多到 MAX_PATH 的解族群
            for (var p = 0; p < MAX_PATH; p++) {

                //產生下一步解族群
                foreach (var solution in cursorGroup) {
                    nextGroup.AddRange(solution.NextStepSolutions());
                }

                //篩選最好前 GROUP_SIZE 個解
                nextGroup.Sort((s1, s2) => s2.weight.CompareTo(s1.weight));

                if (nextGroup.Count > GROUP_SIZE) {
                    nextGroup.RemoveRange(GROUP_SIZE, nextGroup.Count - GROUP_SIZE);
                    GC.Collect();//強制記憶體回收
                }
                //更新歷史最佳解族群
                UpdateBestSolutions(nextGroup);
                bestSolutionGroupForEveryStep.Add(nextGroup[0]);
                //group <= next_group
                var temp = cursorGroup;
                cursorGroup = nextGroup;
                nextGroup = temp;
                //清空 next_group
                nextGroup.RemoveRange(0, nextGroup.Count);
                GC.Collect();//強制記憶體回收
            }
        }
    }
    */
    class SolveBoard2
    {
        public const int ROWS = TOS.ROWS;
        public const int COLS = TOS.COLS;
        //==========參數==============//--------------------------------------------------------------------------
        public static int MAX_PATH = 50;            //最大搜尋步數k
        public static int ITERATION_NUM = 100;      //疊代搜尋次數m
        public static int GROUP_SIZE = 1000;    //每代保留最佳解的個數限制
        public static int BATCH_SIZE = 100;         // BATCH_SIZE <= GROUP_SIZE
        public static int LIMIT_STEP = 8;    // LIMIT_SEARCH_STEP <= MAX_PATH

        public static int BEST_SIZE = 20;       //保留歷史最佳前20個
        //==========計數==============//--------------------------------------------------------------------------
        public static int searchCount = 0;
        public static int endCount = 0;
        public static int deleteCount1 = 0;
        public static int deleteCount2 = 0;
        public static int deleteCount3 = 0;
        //==========成員==============//--------------------------------------------------------------------------
        private readonly OrbsType[,] init_board;                               //初始版面
        public Solution bestSolution;                                       //最佳解
        public readonly List<Solution> bestGroup;                  //歷史最佳解族群
        private readonly List<Solution> batchGroup;
        private readonly List<Solution> group;
        //==========建構式==============//--------------------------------------------------------------------------
        public SolveBoard2(OrbsType[,] board)
        {
            init_board = board;
            bestGroup = new List<Solution>();
            batchGroup = new List<Solution>();
            group = new List<Solution>();
            //計算初始版面解的分數
            bestSolution = new Solution(init_board, 0, 0);
            bestSolution.Evaluate();
            bestGroup.Add(bestSolution);

        }//End of SolveBoard()
        //須執行計算
        public void Run(sbyte row = -1, sbyte col = -1)
        {
            searchCount = 0;
            endCount = 0;
            deleteCount1 = 0;
            deleteCount2 = 0;
            deleteCount3 = 0;
            if (row >= 0 && col >= 0 && row < ROWS && col < COLS)
            {
                searchCount++;
                var solution = new Solution(init_board, row, col);
                solution.Evaluate();
                batchGroup.Add(solution);
            }
            else
            {//加入30個位置初始解族群
                for (var i = 0; i < ROWS; i++)
                    for (var j = 0; j < COLS; j++)
                    {
                        searchCount++;
                        var solution = new Solution(init_board, i, j);
                        solution.Evaluate();
                        batchGroup.Add(solution);
                    }
            }
            for (int iter = 0; iter < ITERATION_NUM; iter++)
            {
                if (batchGroup.Count <= 0) break;
                //產生 批量 解族群
                foreach (var parent in batchGroup)
                {
                    endCount++;
                    if (parent.path.Length >= MAX_PATH)
                    {
                        deleteCount3++;
                        continue;
                    }
                    foreach (var child in parent.NextStepSolutions())
                    {
                        searchCount++;
                        var n = child.path.Length - child.bestPathLength;
                        if (n > LIMIT_STEP)
                        {
                            deleteCount1++;
                            continue;
                        }
                        group.Add(child);
                        if (n == 0) bestGroup.Add(child);
                    }
                }
                //排序
                group.Sort((s1, s2) => s2.weight.CompareTo(s1.weight));


                //篩選
                if (group.Count > GROUP_SIZE)
                {
                    deleteCount2 += (group.Count - GROUP_SIZE);
                    group.RemoveRange(GROUP_SIZE, group.Count - GROUP_SIZE);
                    GC.Collect();//強制記憶體回收
                }
                //取出最好的前 100 個
                batchGroup.Clear();
                var minBatchSize = (BATCH_SIZE <= group.Count) ? BATCH_SIZE : group.Count;
                for (int i = 0; i < minBatchSize; i++)
                    batchGroup.Add(group[i]);
                group.RemoveRange(0, minBatchSize);

                //更新最佳解族群
                bestGroup.Sort((s1, s2) => s2.weight.CompareTo(s1.weight));
                if (bestGroup.Count > BEST_SIZE)
                    bestGroup.RemoveRange(BEST_SIZE, bestGroup.Count - BEST_SIZE);
                //最佳解
                bestSolution = bestGroup[0];
            }
        }
    }
}
