using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Script;

public class ComputeButton : MonoBehaviour {
    private PuzzleSystem puzzleSystem;//PuzzleSystem.cs
    private PuzzleControl puzzleControl;//PuzzleControl.cs
    public Text roadText;
    string printStr = "";
    Button btn;
    Text btnText;
    bool isRun = false;

    public InputField inputFieldMaxPath;
    public InputField inputFieldIterationNun;
    public InputField inputFieldGroupSize;
    public InputField inputFieldBatchSize;
    public InputField inputFieldLimitStep;
    /*
     RectTransform inputFieldMaxPathRect;
     RectTransform inputFieldIterationNunRect;
     RectTransform inputFieldGroupSizeRect;
     RectTransform inputFieldBatchSizeRect;
     RectTransform inputFieldLimitStepRect;
     */
    public Toggle toggle;
    void Start() {
        puzzleSystem = GameObject.Find("PuzzleSystem").GetComponent<PuzzleSystem>();
        puzzleControl = GameObject.Find("PuzzleControl").GetComponent<PuzzleControl>();
        btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
        btnText = btn.GetComponentInChildren<Text>();
        
        /*
        inputFieldMaxPath.text = "50";
        inputFieldIterationNun.text = "100";
        inputFieldGroupSize.text = "1000";
        inputFieldBatchSize.text = "100";
        inputFieldLimitStep.text = "8";
        */
    }

    public void BtnClick() {
        
        if (isRun == false) {
            //Debug.Log("3.路徑顯示");
            
            OrbsType[,] board = new OrbsType[TOS.ROWS, TOS.COLS];
            for (int i = 0; i < TOS.ROWS; i++) {
                for (int j = 0; j < TOS.COLS; j++) {
                    board[i, j] = (OrbsType)puzzleSystem.board[i][j].type;
                }
            }
            if (!int.TryParse(inputFieldMaxPath.text, out SolveBoard2.MAX_PATH) || SolveBoard2.MAX_PATH <= 0) {
                SolveBoard2.MAX_PATH = 50;
                inputFieldMaxPath.text = "50";
            }
            if (!int.TryParse(inputFieldIterationNun.text, out SolveBoard2.ITERATION_NUM) || SolveBoard2.ITERATION_NUM <= 0) {
                SolveBoard2.ITERATION_NUM = 100;
                inputFieldIterationNun.text = "100";
            }
            if (!int.TryParse(inputFieldGroupSize.text, out SolveBoard2.GROUP_SIZE) || SolveBoard2.GROUP_SIZE <= 0) {
                SolveBoard2.GROUP_SIZE = 1000;
                inputFieldGroupSize.text = "1000";
            }
            if (!int.TryParse(inputFieldBatchSize.text, out SolveBoard2.BATCH_SIZE) || SolveBoard2.BATCH_SIZE <= 0) {
                SolveBoard2.BATCH_SIZE = 100;
                inputFieldBatchSize.text = "100";
            } else if (SolveBoard2.BATCH_SIZE > SolveBoard2.GROUP_SIZE) {
                SolveBoard2.BATCH_SIZE = SolveBoard2.GROUP_SIZE;
                inputFieldBatchSize.text = SolveBoard2.BATCH_SIZE.ToString();
            }
            if (!int.TryParse(inputFieldLimitStep.text, out SolveBoard2.LIMIT_STEP) || SolveBoard2.LIMIT_STEP <= 0) {
                SolveBoard2.LIMIT_STEP = 8;
                inputFieldLimitStep.text = "8";
            } else if (SolveBoard2.LIMIT_STEP > SolveBoard2.MAX_PATH) {
                SolveBoard2.LIMIT_STEP = SolveBoard2.MAX_PATH;
                inputFieldLimitStep.text = SolveBoard2.LIMIT_STEP.ToString();
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

            //Debug.Log("\t\t\t(" + row + ", " + col + ") : " + pathStr + " weight = " + weight);
            //Debug.Log("\t\t\t MaxScore = " + Compute.GetMaxScore(best.init.board));
            var searchCount = SolveBoard2.searchCount;
            var endCount = SolveBoard2.endCount;
            var deleteCount1 = SolveBoard2.deleteCount1;
            var deleteCount2 = SolveBoard2.deleteCount2;
            var deleteCount3 = SolveBoard2.deleteCount3;
            var nn = searchCount - endCount - deleteCount1 - deleteCount2;
            var str = "" + searchCount + "=" + endCount + "+(" + deleteCount1 + "+" + deleteCount2 + "+" + deleteCount3 + ")+" + nn;

            printStr = ("起始位置(x,y) = (" + col +", "+ row+")\n");            
            printStr += ("路徑(" + pathStr.Length + ")：" + pathStr + "\n");
            printStr += ("得分: " + weight + "\n");
            printStr += comboStr + str;



            SetMove(row, col, pathStr);
            btnText.text = "開始轉珠";
        } else {
            
            var timer = 0f;
            for (int i = 0; i < moveVector.Count; i++) {
                timer += 0.3f;
                Invoke("Move", timer);
            }
            
            timer += 0.5f;
            Invoke("Run", timer);
            btnText.text = "計算路徑";
        }
        isRun = !isRun;

    }
    void RunTOS() {
        System.Diagnostics.Process.Start(@"TOS\TOS.exe");
    }
    /*
    private string[] readTXT;
    void ReadTOS() {
        readTXT = File.ReadAllLines("TOS/bestSolutionGroup.txt");//读取文件的所有行，并将数据读取到定义好的字符数组strs中，一行存一个单元
        roadText.text = readTXT[0];
        var readTxtLine = readTXT[0];
        var strArr = readTxtLine.Split('|');
        var pointStrArr = strArr[0].Split(',');
        var row = int.Parse(pointStrArr[0]);
        var col = int.Parse(pointStrArr[1]);
        var pathStr = strArr[1];
        SetMove(row, col, pathStr);
    }*/
    private Vector2 startPoint;
    private List<Vector2> moveVector = new List<Vector2>();
    private int move_count = 0;
    void SetMove(int row, int col, string pathStr) {
        moveVector.Clear();
        move_count = 0;
        startPoint = new Vector2(col, row);
        foreach (var dirChar in pathStr) {
            switch (dirChar) {
                case '↑':
                    moveVector.Add(Vector2.up);
                    break;
                case '↓':
                    moveVector.Add(Vector2.down);
                    break;
                case '←':
                    moveVector.Add(Vector2.left);
                    break;
                case '→':
                    moveVector.Add(Vector2.right);
                    break;
                default:
                    break;
            }
        }
    }
    void Move() {
        //Debug.Log("move : " + move_count);
        if (move_count >= moveVector.Count)
            return;
        var board = puzzleSystem.board;

        var aPoint = startPoint;
        var aVector = moveVector[move_count];
        var bPoint = aPoint + aVector;

        var orb_a = board[(int)aPoint.y][(int)aPoint.x];
        var orb_b = board[(int)bPoint.y][(int)bPoint.x];

        orb_a.SetAniPos(aVector, Orb.OrbsState.Create);
        orb_b.SetAniPos(-aVector, Orb.OrbsState.Create);

        var temp = orb_a.type;
        orb_a.type = orb_b.type;
        orb_b.type = temp;

        startPoint = bPoint;
        move_count++;
    }

    void Run() {
        puzzleControl.state = PuzzleControl.PuzzleState.End;
    }

    void Update()  {

        roadText.text = printStr;
        if (toggle.isOn) {
            inputFieldMaxPath.GetComponent<RectTransform>().sizeDelta = new Vector2(108, 30);
            inputFieldIterationNun.GetComponent<RectTransform>().sizeDelta = new Vector2(108, 30);
            inputFieldGroupSize.GetComponent<RectTransform>().sizeDelta = new Vector2(108, 30);
            inputFieldBatchSize.GetComponent<RectTransform>().sizeDelta = new Vector2(108, 30);
            inputFieldLimitStep.GetComponent<RectTransform>().sizeDelta = new Vector2(108, 30);
        } else {
            inputFieldMaxPath.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            inputFieldIterationNun.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            inputFieldGroupSize.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            inputFieldBatchSize.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            inputFieldLimitStep.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        }
    }
}
