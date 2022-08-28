using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using Assets.Script;

public enum BoardRunStates : int { Idle, Remove, Fall };
public class BoardControl : BoardBase
{
    BoardRunStates state = BoardRunStates.Idle;

    #region Button Click
    string saveDataStr = null;
    public void SaveButtonClick()
    {
        Debug.Log("Save Button Click");
        var boardData = new BoardData(board);
        saveDataStr = boardData.GetSaveJsonString();
        Debug.Log(saveDataStr);
        /*
        using (StreamWriter sw = new StreamWriter("BoardDataFile.json"))
        {
            sw.WriteLine(saveDataStr);
        }
        */
    }
    public void ResetButtonClick()
    {
        Debug.Log("Reset Button Click");
        //saveDataStr = File.ReadAllLines("BoardDataFile.json")[0];
        if (saveDataStr != null && saveDataStr != "")
        {
            var boardData = new BoardData(saveDataStr);
            boardData.LoadToBoard(board);
        }
    }
    public void RandomButtonClick()
    {
        Debug.Log("Random Button Click");
        foreach (var ibead in beads)
        {
            ibead.CreateNewRandomBead();
        }
        ComboStart();
    }
    #endregion

    #region Compute Button Click
    public InputField inputFieldMaxPath;
    public InputField inputFieldIterationNun;
    public InputField inputFieldGroupSize;
    public InputField inputFieldBatchSize;
    public InputField inputFieldLimitStep;
    public Text roadText;
    public Toggle toggle;

    string printStr = "";
    public void ComputeButtonClick()
    {
        this.board = GetBoardLinkBeads(beads, ROWS, COLS);
        Debug.Log("Compute Button Click");

        if (!int.TryParse(inputFieldMaxPath.text, out SolveBoard2.MAX_PATH) || SolveBoard2.MAX_PATH <= 0)
        {
            SolveBoard2.MAX_PATH = 50;
            inputFieldMaxPath.text = "50";
        }
        if (!int.TryParse(inputFieldIterationNun.text, out SolveBoard2.ITERATION_NUM) || SolveBoard2.ITERATION_NUM <= 0)
        {
            SolveBoard2.ITERATION_NUM = 100;
            inputFieldIterationNun.text = "100";
        }
        if (!int.TryParse(inputFieldGroupSize.text, out SolveBoard2.GROUP_SIZE) || SolveBoard2.GROUP_SIZE <= 0)
        {
            SolveBoard2.GROUP_SIZE = 1000;
            inputFieldGroupSize.text = "1000";
        }
        if (!int.TryParse(inputFieldBatchSize.text, out SolveBoard2.BATCH_SIZE) || SolveBoard2.BATCH_SIZE <= 0)
        {
            SolveBoard2.BATCH_SIZE = 100;
            inputFieldBatchSize.text = "100";
        }
        else if (SolveBoard2.BATCH_SIZE > SolveBoard2.GROUP_SIZE)
        {
            SolveBoard2.BATCH_SIZE = SolveBoard2.GROUP_SIZE;
            inputFieldBatchSize.text = SolveBoard2.BATCH_SIZE.ToString();
        }
        if (!int.TryParse(inputFieldLimitStep.text, out SolveBoard2.LIMIT_STEP) || SolveBoard2.LIMIT_STEP <= 0)
        {
            SolveBoard2.LIMIT_STEP = 8;
            inputFieldLimitStep.text = "8";
        }
        else if (SolveBoard2.LIMIT_STEP > SolveBoard2.MAX_PATH)
        {
            SolveBoard2.LIMIT_STEP = SolveBoard2.MAX_PATH;
            inputFieldLimitStep.text = SolveBoard2.LIMIT_STEP.ToString();
        }
        OrbsType[,] board = new OrbsType[TOS.ROWS, TOS.COLS];
        for (int i = 0; i < TOS.ROWS; i++)
        {
            for (int j = 0; j < TOS.COLS; j++)
            {
                //Debug.Log("this.board[" + i + "," + j +  "].beadType = " + this.board[i, j].beadType);
                board[i, j] = (OrbsType)(int)this.board[i, j].beadType;
            }
        }
        var solveBoard = new SolveBoard2(board);
        solveBoard.Run();
        var best = solveBoard.bestSolution;
        var row = best.init.row;
        var col = best.init.col;
        var pathStr = best.GetPathString();
        var (combos, weight) = Compute.Result(best.turn.board);
        string comboStr = "Combo: " + combos.Count + " | ";
        foreach (var (type, count) in combos)
            comboStr += (type.ToString() + count.ToString() + " ");
        comboStr += "\n";

        Debug.Log("\t\t\t(" + row + ", " + col + ") : " + pathStr + " weight = " + weight);
        Debug.Log("\t\t\t MaxScore = " + Compute.GetMaxScore(best.init.board));
        var searchCount = SolveBoard2.searchCount;
        var endCount = SolveBoard2.endCount;
        var deleteCount1 = SolveBoard2.deleteCount1;
        var deleteCount2 = SolveBoard2.deleteCount2;
        var deleteCount3 = SolveBoard2.deleteCount3;
        var nn = searchCount - endCount - deleteCount1 - deleteCount2;
        var str = "" + searchCount + "=" + endCount + "+(" + deleteCount1 + "+" + deleteCount2 + "+" + deleteCount3 + ")+" + nn;

        printStr = ("起始位置(x,y) = (" + col + ", " + row + ")\n");
        printStr += ("路徑(" + pathStr.Length + "): " + pathStr + "\n");
        printStr += ("得分: " + weight + "\n");
        printStr += comboStr + str;
        finger.RunAutoStart(this.board[row, col], new Vector2Int(col, row), pathStr);
    }
    #endregion

    #region 
    public void OnToggleChangeValue()
    {
        if (toggle.isOn)
        {
            inputFieldMaxPath.GetComponent<RectTransform>().sizeDelta = new Vector2(108, 30);
            inputFieldIterationNun.GetComponent<RectTransform>().sizeDelta = new Vector2(108, 30);
            inputFieldGroupSize.GetComponent<RectTransform>().sizeDelta = new Vector2(108, 30);
            inputFieldBatchSize.GetComponent<RectTransform>().sizeDelta = new Vector2(108, 30);
            inputFieldLimitStep.GetComponent<RectTransform>().sizeDelta = new Vector2(108, 30);
        }
        else
        {
            inputFieldMaxPath.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            inputFieldIterationNun.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            inputFieldGroupSize.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            inputFieldBatchSize.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            inputFieldLimitStep.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        }
    }
    #endregion


    public RectTransform BGRect;
    public FingerObj finger;
    [SerializeField] static int ROWS = 5;
    [SerializeField] static int COLS = 6;
    [SerializeField] List<BeadObj> beads = null;
    [SerializeField] BeadObj[,] board = null;

    void Start()
    {
        beads = GetCreateBeadsInRect(BGRect, ROWS, COLS);

        board = GetBoardLinkBeads(beads, ROWS, COLS);

        ComboStart();
        toggle.isOn = false;
        
    }

    #region ComboStart & RemoveAnimation
    //[SerializeField] BeadType[,] beadType2d = null;
    [SerializeField] bool[,] canRemove2d = null;
    //[SerializeField] List<List<(int r, int c)>> matches = null;
    [SerializeField] float removeTimer;
    [SerializeField] float removeTotalTime;
    [SerializeField] float removeUnitTime = 0.1f;
    public void ComboStart()
    {
        // 更新 board
        board = GetBoardLinkBeads(beads, ROWS, COLS);
        // 複製 beadType
        BeadType[,] beadType2d = GetBeadType2d(board);
        canRemove2d = FindCanRemove2d(beadType2d);
        // 計算 各combo有哪需要消除的珠子
        List<List<(int r, int c)>> matches = FindSingleMatches(beadType2d, canRemove2d);
        // 移除動畫 第幾個消除 總共多少個消除
        RemoveAnimation(board, matches, removeUnitTime);
        removeTotalTime = removeUnitTime * matches.Count;
        if (removeTotalTime != 0.0f)
        {
            state = BoardRunStates.Remove;
            removeTimer = 0.0f;
        }
        else ComboEnd();
    }

    private void RemoveAnimation(BeadObj[,] board, List<List<(int r, int c)>> matches, float removeUnitTime)
    {   // 執行整個版面 消除動畫 // removeTotalTime = removeUnitTime * matches.Count;
        for (var icombo = 0; icombo < matches.Count; icombo++)
        {
            for (var inum = 0; inum < matches[icombo].Count; inum++)
            {
                var (ir, ic) = matches[icombo][inum];
                board[ir, ic].removeAnimation(icombo, matches.Count, removeUnitTime);
            }
        }
    }
    #endregion

    #region FallAnimation
    [SerializeField] float fallTimer;
    [SerializeField] float fallTotalTime;
    [SerializeField] float fallUnitTime = 0.1f;
    private void FallAnimation()
    {
        // 計算各直行 保留&消除 的珠子有哪些
        var fallKeeps = new List<List<int>>();
        var fallRemoves = new List<List<int>>();
        var maxLineRemoveNum = 0;
        for (var ic = 0; ic < COLS; ic++) //x COLS=6
        {
            fallKeeps.Add(new List<int>());
            fallRemoves.Add(new List<int>());
            for (var ir = 0; ir < ROWS; ir++) //y ROWS=5
                if (canRemove2d[ir, ic] == false)
                    fallKeeps[ic].Add(ir);
                else
                    fallRemoves[ic].Add(ir);
            if (fallRemoves[ic].Count > maxLineRemoveNum)
                maxLineRemoveNum = fallRemoves[ic].Count;
        }
        fallTotalTime = fallUnitTime * maxLineRemoveNum;
        //Debug.Log("fallTotalTime=" + fallTotalTime);
        for (var ic = 0; ic < COLS; ic++) //x COLS=6
        {
            if (fallRemoves[ic].Count == 0) continue;
            var removeNum = fallRemoves[ic].Count;
            var dest_row = 0;
            for (var i = 0; i < fallKeeps[ic].Count; i++)
            {
                var ir = fallKeeps[ic][i];
                var src_row = ir;
                //Debug.Log(" board[" + ir + "][" + ic + "] => src_row:" + src_row + " dest_row:" + dest_row);
                board[ir, ic].fallAnimation(src_row, board[dest_row, ic], removeNum, fallUnitTime);
                dest_row += 1;
            }
            for (var i = 0; i < fallRemoves[ic].Count; i++)
            {
                var ir = fallRemoves[ic][i];
                var src_row = ROWS + i;
                //Debug.Log("_board[" + ir + "][" + ic + "] => src_row:" + src_row + " dest_row:" + dest_row);
                board[ir, ic].fallAnimation(src_row, board[dest_row, ic], removeNum, fallUnitTime);
                dest_row += 1;
            }
        }
    }

    #endregion

    public void ComboEnd()
    {
        //Debug.Log("ComboEnd");
        state = BoardRunStates.Idle;
    }


    // Update is called once per frame
    void Update()
    {
        if (state == BoardRunStates.Remove)
        {
            removeTimer += Time.deltaTime;
            if (removeTimer > removeTotalTime)
            {
                state = BoardRunStates.Fall;
                FallAnimation();
                fallTimer = 0.0f;
            }
        }
        else if (state == BoardRunStates.Fall)
        {
            fallTimer += Time.deltaTime;
            if (fallTimer > fallTotalTime)
            {
                state = BoardRunStates.Remove;
                ComboStart();
            }
        }





        roadText.text = printStr;


    }
}
