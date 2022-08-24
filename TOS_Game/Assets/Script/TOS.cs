using System;
using System.Collections.Generic;

namespace Assets.Script
{
    public enum OrbsType : sbyte { R, B, G, L, D, H, Null };
    public enum DIR : sbyte { RIGHT, RIGHT_DOWN, DOWN, LEFT_DOWN, LEFT, LEFT_UP, UP, RIGHT_UP, STOP};
    class TOS {
        public const int ROWS = 5;
        public const int COLS = 6;
    }
    class TureBoard {
        public const int ROWS = TOS.ROWS;
        public const int COLS = TOS.COLS;
        public readonly OrbsType[,] board;
        public sbyte row, col;
        public TureBoard(ref OrbsType[,] board, int row, int col) {
            this.board = board;
            this.row = (sbyte)row;
            this.col = (sbyte)col;
        }
        public TureBoard(OrbsType[,] board, int row, int col) {
            this.board = new OrbsType[ROWS, COLS];
            Array.Copy(board, this.board, board.Length);
            this.row = (sbyte)row;
            this.col = (sbyte)col;
        }
        public TureBoard Copy() => new TureBoard(board, row, col);
        public bool CanMoveDir(DIR dir) {
            switch (dir) {
                case DIR.RIGHT:         return col < COLS - 1;
                case DIR.RIGHT_UP:    return row < ROWS - 1 && col < COLS - 1;
                case DIR.UP:          return row < ROWS - 1;
                case DIR.LEFT_UP:     return row < ROWS - 1 && col > 0;
                case DIR.LEFT:          return col > 0;
                case DIR.LEFT_DOWN:       return row > 0 && col > 0;
                case DIR.DOWN:            return row > 0;
                case DIR.RIGHT_DOWN:      return row > 0 && col < COLS - 1;
                default:                return false;
            }
        }
        public void MoveDir(DIR dir) {
            switch (dir) {
                case DIR.RIGHT:         col += 1; break;
                case DIR.RIGHT_UP:    row += 1; col += 1; break;
                case DIR.UP:          row += 1; break;
                case DIR.LEFT_UP:     row += 1; col -= 1; break;
                case DIR.LEFT:          col -= 1; break;
                case DIR.LEFT_DOWN:       row -= 1; col -= 1; break;
                case DIR.DOWN:            row -= 1; break;
                case DIR.RIGHT_DOWN:      row -= 1; col += 1; break;
                default: break;
            }
        }
        public void MoveBoardDir(DIR dir) {
            var old = (row, col);
            MoveDir(dir);
            Swap(ref board[old.row, old.col], ref board[row, col]);
        }
        private void Swap(ref OrbsType a, ref OrbsType b) {
            var temp = a; a = b; b = temp;
        }
        public bool Equal(TureBoard trueBoard) {
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

    class Solution {
        public static DIR[] ModeDir4 = { DIR.RIGHT, DIR.DOWN, DIR.LEFT, DIR.UP };
        public static DIR[] ModeDir8 = { DIR.RIGHT, DIR.RIGHT_DOWN, DIR.DOWN, DIR.LEFT_DOWN, DIR.LEFT, DIR.LEFT_UP, DIR.UP, DIR.RIGHT_UP };
        public TureBoard init;
        public TureBoard turn;
        public DIR[] path;
        public float weight;
        public sbyte bestPathLength;
        public float beforeBestWeight;
        public Solution(OrbsType[,] init_board, int row, int col) {
            init =  new TureBoard(ref init_board, row, col);
            turn = init.Copy();
            path = Array.Empty<DIR>();//(空)
            weight = 0f;
            bestPathLength = 0;
            beforeBestWeight = 0f;
        }
        public Solution(Solution solution) {
            init = solution.init;
            turn = solution.turn.Copy();
            path = solution.path;
            weight = 0f;
            bestPathLength = solution.bestPathLength;
            beforeBestWeight = solution.beforeBestWeight;
        }
        public bool CanMoveTurnDir(DIR dir) {
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
            for (var i = path.Length - 1; i >= 0; i--) {
                var idir = path[i];
                beforeBoard.MoveBoardDir(DirInv(idir));
                if (beforeBoard.Equal(nextBoard))
                    return false;
            }
            return true;
        }
        public void MoveTurnDir(DIR dir) {
            turn.MoveBoardDir(dir);
            //path add
            var new_path = new DIR[path.Length + 1];
            Array.Copy(path, new_path, path.Length);
            path = new_path;
            path[path.Length - 1] = dir;//add
        }
        public void Evaluate() {
            var (_, score) = Compute.Result(turn.board);
            weight = score;
        }
        public List<Solution> NextStepSolutions() {
            var solutions = new List<Solution>();
            foreach (var dir in ModeDir4) {
                if (CanMoveTurnDir(dir)) {
                    var solution = new Solution(this);
                    solution.MoveTurnDir(dir);
                    solution.Evaluate();
                    if (solution.weight > beforeBestWeight) {
                        solution.beforeBestWeight = solution.weight;
                        solution.bestPathLength = (sbyte)solution.path.Length;
                    }
                    solutions.Add(solution);
                }
            }
            return solutions;
        }
        private DIR DirInv(DIR dir) {
            switch (dir) {
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
        private String DirStr(DIR dir) {
            switch (dir) {
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
        public String GetPathString() {
            if (path.Length <= 0)
                return "Empty";
            var pathString = "";
            foreach (var dir in path)
                pathString += DirStr(dir);
            return pathString;
        }
    }
    class Compute {
        public const int ROWS = TOS.ROWS;
        public const int COLS = TOS.COLS;
        public const int removeCount = 3;
        public const float MULTI_ORB_BONUS = 0.25f;
        public const float COMBO_BONUS = 0.25f;
        public static Dictionary<OrbsType, int> weights = new Dictionary<OrbsType, int> {
            [OrbsType.R] = 100,
            [OrbsType.G] = 100,
            [OrbsType.B] = 100,
            [OrbsType.D] = 100,
            [OrbsType.L] = 100,
            [OrbsType.H] = 100,
            [OrbsType.Null] = 0
        };
        public static (List<(OrbsType type, int count)> combos, float score) Result(OrbsType[,] board) {
            //tempBoard is all TYPE.X
            var tempBoard = new OrbsType[ROWS, COLS];
            for (var i = 0; i < ROWS; i++)
                for (var j = 0; j < COLS; j++)
                    tempBoard[i, j] = OrbsType.Null;
            //
            for (var i = 0; i < ROWS; i++) {
                OrbsType prev_1_orb = OrbsType.Null, prev_2_orb = OrbsType.Null;
                for (int j = 0; j < COLS; j++) {
                    var cur_orb = board[i, j];
                    if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != OrbsType.Null) {
                        tempBoard[i, j] = tempBoard[i, j - 1] = tempBoard[i, j - 2] = cur_orb;
                    }
                    prev_1_orb = prev_2_orb;
                    prev_2_orb = cur_orb;
                }
            }
            for (var j = 0; j < COLS; j++) {
                OrbsType prev_1_orb = OrbsType.Null, prev_2_orb = OrbsType.Null;
                for (var i = 0; i < ROWS; i++) {
                    var cur_orb = board[i, j];
                    if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != OrbsType.Null) {
                        tempBoard[i, j] = tempBoard[i - 1, j] = tempBoard[i - 2, j] = cur_orb;
                    }
                    prev_1_orb = prev_2_orb;
                    prev_2_orb = cur_orb;
                }
            }
            //洪水法
            var combos = new List<(OrbsType type, int count)>();
            for (var i = 0; i < ROWS; i++) {
                for (var j = 0; j < COLS; j++) {
                    var cur_orb = tempBoard[i, j];
                    if (cur_orb == OrbsType.Null)
                        continue;
                    var stack = new Stack<(int row, int col)>();
                    stack.Push((i, j));
                    var count = 0;
                    while (stack.Count != 0) {
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

        public static float GetMaxScore(OrbsType[,] board) {
            var countDict = new Dictionary<OrbsType, int>();
            foreach (OrbsType item in Enum.GetValues(typeof(OrbsType)))
                countDict.Add(item, 0);
            foreach (var item in board)
                countDict[item]++;
            var comboSum = 0;
            var maxScore = 0f;
            foreach (var item in countDict) {
                var type = item.Key;
                var count = item.Value;
                if (count >= removeCount) {
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
    class SolveBoard2 {
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
        public SolveBoard2(OrbsType[,] board) {
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
        public void Run(sbyte row = -1, sbyte col = -1) {
            searchCount = 0;
            endCount = 0;
            deleteCount1 = 0;
            deleteCount2 = 0;
            deleteCount3 = 0;
            if (row >= 0 && col >= 0 && row < ROWS && col < COLS) {
                searchCount++;
                var solution = new Solution(init_board, row, col);
                solution.Evaluate();
                batchGroup.Add(solution);
            } else {//加入30個位置初始解族群
                for (var i = 0; i < ROWS; i++)
                    for (var j = 0; j < COLS; j++) {
                        searchCount++;
                        var solution = new Solution(init_board, i, j);
                        solution.Evaluate();
                        batchGroup.Add(solution);
                    }
            }
            for (int iter = 0; iter < ITERATION_NUM; iter++) {
                if (batchGroup.Count <= 0) break;
                //產生 批量 解族群
                foreach (var parent in batchGroup) {
                    endCount++;
                    if (parent.path.Length >= MAX_PATH) {
                        deleteCount3++;
                        continue;
                    }
                    foreach (var child in parent.NextStepSolutions()) {
                        searchCount++;
                        var n = child.path.Length - child.bestPathLength;
                        if (n > LIMIT_STEP) {
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
                if (group.Count > GROUP_SIZE) {
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
