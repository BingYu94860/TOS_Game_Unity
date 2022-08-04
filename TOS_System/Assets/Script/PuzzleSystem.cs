using UnityEngine;
using System.Collections.Generic;


public class PuzzleSystem : MonoBehaviour {
    public RectTransform BGRect;//新增變數，背景圖的RectTransform、盤面中行與列的珠子數、位移的距離。
    /*
     BGRect.sizeDelta.x = 720;
     BGRect.sizeDelta.y = 600;
     height = BGRect.sizeDelta.y / ROWS = 600/5 = 120;
     width  = BGRect.sizeDelta.x / COLS = 720/6 = 120;

     rect = GetComponent<RectTransform>();
     rect.anchoredPosition
         */
    public int ROWS = 5;
    public int COLS = 6;
    private Vector2 addPos = new Vector2(120, 120);
    public List<Orb> orbs = new List<Orb>();// PuzzleSystem中要新增兩個List，orbs一維List會儲存盤面中所有珠子的Orb腳本。orbGroups二維陣列會儲存珠子的群組。
    public List<List<Orb>> board = new List<List<Orb>>();
    


    public bool hasRemove = false;//新增布林變數hasRemove，用來判斷是否要重複執行找群組、combo等等動作。
  
    public float removeTime;

    public float point = 0f;//總分

    public const int removeCount = 3;//宣告珠子消除的數量門檻，這裡為3，當然可以調整成2消或4消，依個人狀況而定。
    public const float MULTI_ORB_BONUS = 0.25f;
    public const float COMBO_BONUS = 0.25f;

    void Start() {
        InitGrid();//【0】
        do {
           hasRemove = false;
            OrbInit();//【1】
            BoardCombo();//【2】
            OrbRemove(true);//【4】
        } while (hasRemove);//if orb was removed
    }
    //【0】
    public void InitGrid() {//新增初始化盤面的函式InitGrid，函式當中使用兩層的迴圈，在盤面中每個位置新增珠子。
        //Debug.Log("\t\t【0】InitGrid");
        addPos.y = BGRect.sizeDelta.y / ROWS; // 600/5=120
        addPos.x = BGRect.sizeDelta.x / COLS; // 720/6=120

        for (int r = 0; r < ROWS; r++) {
            var line = new List<Orb>();
            for (int c = 0; c < COLS; c++) {
                //orb pos scale
                GameObject orbObj = Instantiate(Resources.Load("Prefabs/Orb")) as GameObject;//產生珠子物件。
                RectTransform orbRect = orbObj.GetComponent<RectTransform>();
                orbRect.SetParent(BGRect);//設定珠子物件的父物件。
                orbRect.localScale = Vector2.one;//設定比例。
                orbRect.anchoredPosition = new Vector2(c * addPos.x, r * addPos.y);//設定位置。
                //orb type number init
                Orb orb = orbObj.GetComponent<Orb>();//抓取珠子物件下的Orb腳本，修改Orb腳本下的row和column，接著呼叫OrbCreate函式。
                orb.type = Orb.OrbsType.Null;//修正在InitGrid中呼叫OrbCreate的地方，改成帶入Null屬性。
                orb.row = r;
                orb.col = c;
                orb.id = orb.row * COLS + orb.col;
                orb.height = BGRect.sizeDelta.y / ROWS;
                orb.width = BGRect.sizeDelta.x / COLS;
                orbs.Add(orb); //在產生盤面的迴圈中，把當前的珠子腳本新增到orbs一維List中。  
                line.Add(orb);
            }
            board.Add(line);
        }
    }
    //【1】
    public void OrbInit() {//在OrbCreate函式中，使用Random隨機一個珠子類型編號，並且將該編號帶入珠子的類型中。
        //Debug.Log("\t\t【1】OrbInit");
        foreach (Orb orb in orbs) {
            orb.removed = false;
            if (orb.type == Orb.OrbsType.Null) {
                orb.SetAniPos(Vector2.up, Orb.OrbsState.Create);
                orb.type = (Orb.OrbsType)Random.Range(0, (int)Orb.OrbsType.Null);
            }
        }
    }
    public void BoardCombo() {
        var orb2d = new Orb.OrbsType[ROWS, COLS];
        var temp2d = new Orb.OrbsType[ROWS, COLS];
        for (int i = 0; i < ROWS; i++) {
            for (int j = 0; j < COLS; j++) {
                orb2d[i, j] = board[i][j].type;
                temp2d[i, j] = Orb.OrbsType.Null;
            }
        }
        Orb.OrbsType prev_1_orb, prev_2_orb;
        for (var i = 0; i < ROWS; i++) {
            prev_1_orb = prev_2_orb = Orb.OrbsType.Null;
            for (int j = 0; j < COLS; j++) {
                var cur_orb = orb2d[i, j];
                if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != Orb.OrbsType.Null) {
                    temp2d[i, j] = temp2d[i, j - 1] = temp2d[i, j - 2] = cur_orb;
                }
                prev_1_orb = prev_2_orb;
                prev_2_orb = cur_orb;
            }
        }
        for (var j = 0; j < COLS; j++) {
            prev_1_orb = prev_2_orb = Orb.OrbsType.Null;
            for (var i = 0; i < ROWS; i++) {
                var cur_orb = orb2d[i, j];
                if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != Orb.OrbsType.Null) {
                    temp2d[i, j] = temp2d[i - 1, j] = temp2d[i - 2, j] = cur_orb;
                }
                prev_1_orb = prev_2_orb;
                prev_2_orb = cur_orb;
            }
        }
        var matches = new List<List<(int row, int col)>>();
        for (var i = 0; i < ROWS; i++) {
            for (var j = 0; j < COLS; j++) {
                var cur_orb = temp2d[i, j];
                if (cur_orb == Orb.OrbsType.Null)
                    continue;
                var stack = new Stack<(int row, int col)>();
                stack.Push((i, j));
                var matche = new List<(int row, int col)>();
                while (stack.Count != 0) {
                    var p = stack.Pop();
                    if (temp2d[p.row, p.col] != cur_orb)
                        continue;
                    matche.Add(p);
                    temp2d[p.row, p.col] = Orb.OrbsType.Null;
                    if (p.row < ROWS -1)//↑
                        stack.Push((p.row+1, p.col));
                    if (p.row > 0)//↓
                        stack.Push((p.row-1, p.col));
                    if (p.col > 0)//←
                        stack.Push((p.row, p.col-1));
                    if (p.col < COLS - 1)//→
                        stack.Push((p.row, p.col+1));
                }// End of while(stack is not Empty)
                matches.Add(matche);
            }
        }
        removeTime = 0f;
        point = 0f;
        
        for (int i = 0; i < matches.Count; i++) {
            string str = "第 " + (i+1) + " Combo : ";
            for (int j = 0; j < matches[i].Count; j++) {
                var (row, col) = matches[i][j];
                str += "(" + row + ", " + col + ") ";
                var orb = board[row][col];
                orb.state = Orb.OrbsState.Remove;
                orb.removed = true;
                orb.removeTime = removeTime;
            }
            //Debug.Log(str);
            removeTime += 0.5f;
            point += (100f) * (1f + ((matches[i].Count - removeCount) * MULTI_ORB_BONUS));//計消珠分
        }
        point *= (1f + ((matches.Count - 1f) * COMBO_BONUS));//計combo分
        Debug.Log("共 " + matches.Count + " Combo " + point + " 分");
    }

    //【4】
    public void OrbRemove(bool remove) {   //宣告OrbRemove函式用來處理珠子消除與落下。
        //Debug.Log("\t\t【4】OrbRemove");
        //可移除珠子 設定 為空珠
        foreach (var orb in orbs) {//針對每一個珠子重設屬性。
            if (orb.removed == true) {
                orb.type = Orb.OrbsType.Null;//當找到珠子為需要被移除的狀態，把珠子的屬性修改為Null。
                if (remove)
                    hasRemove = true;//同時將hasRemove設為true，這時Start中的while迴圈就會再次執行。
            }
        }
        //珠子落下  //針對每一個珠子做掉落判定。
        for (var col = 0; col < COLS; col++) {
            //src (source, 來源, From); dest (destination, 目的地, To);
            for (var dest_row = 0; dest_row < ROWS; dest_row++) {           //目的地To
                var dest_orb = board[dest_row][col];
                if (dest_orb.type != Orb.OrbsType.Null) 
                    continue;
                //找空珠(落下位置) //當找到屬性為Null時。
                var count = 0;
                for (var src_row = dest_row; src_row < ROWS; src_row++) {   //來源From    //往珠子的上方搜尋。
                    count++;
                    var src_orb = board[src_row][col];
                    //找一般珠(使落下)
                    if (src_orb.type != Orb.OrbsType.Null) {//當找到非Null屬性的珠子時。
                        dest_orb.type = src_orb.type;       //把當前珠子屬性設定為該珠子的屬性。
                        src_orb.type = Orb.OrbsType.Null;   //將該珠子設定為Null屬性。
                        //落下 目的地 的 珠子 設為新狀態
                        dest_orb.SetAniPos(Vector2.up, Orb.OrbsState.Create);
                        break;      //設定完屬性就代表該珠子掉落完成，結束往上搜尋的迴圈。
                    }
                }
            }
        }
    }
    //void Update() {}
}
