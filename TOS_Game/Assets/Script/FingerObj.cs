using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum FingerRunStates : int { Idle, Touch, Auto };

public class FingerObj : BeadBase
{
    [SerializeField] FingerRunStates state = FingerRunStates.Idle;
    public Text timerText; //畫面第4行 //剩餘時間
    public Text positionText; //畫面第5-1行 //轉珠位置
    public Text pathText; //畫面第5-2行 //已走步數

    #region 自動演示移動
    [SerializeField] private float autoStepTimer = 0.0f;
    [SerializeField] private int autoStepCount = 9999;
    [SerializeField] private Vector3 autoDestPosition = new Vector3();
    [SerializeField] private List<Vector2Int> autoPosPathList = new List<Vector2Int>();

    public void RunAutoStart(BeadObj beadObj, Vector2Int pos, string pathStr)
    {
        // 狀態 從 Idle 變成 Auto
        if (state != FingerRunStates.Idle) return;
        state = FingerRunStates.Auto;
        Debug.Log("RunAuto ");
        // 移動位置 & 改變起手珠色
        transform.position = beadObj.InitPosition;
        beadType = beadObj.beadType;
        // 計算接下來的路徑位子的列表
        autoPosPathList = GetPosPathList(pos, pathStr);
        // 參數重置
        autoStepTimer = 0.0f;
        autoStepCount = 0;
        touchCount = 0;
        touchBeadObj = null;
    }

    private List<Vector2Int> GetPosPathList(Vector2Int pos, string pathStr)
    {
        var posMove = new Vector2Int(pos.x, pos.y);
        var autoPosPathList = new List<Vector2Int>();
        autoPosPathList.Add(posMove);
        foreach (var dirChar in pathStr)
        {
            switch (dirChar)
            {
                case '↑':
                    posMove = posMove + Vector2Int.up;
                    autoPosPathList.Add(posMove);
                    break;
                case '↓':
                    posMove = posMove + Vector2Int.down;
                    autoPosPathList.Add(posMove);
                    break;
                case '←':
                    posMove = posMove + Vector2Int.left;
                    autoPosPathList.Add(posMove);
                    break;
                case '→':
                    posMove = posMove + Vector2Int.right;
                    autoPosPathList.Add(posMove);
                    break;
                default:
                    break;
            }
        }
        return autoPosPathList;
    }
    #endregion

    #region 手動移動
    [SerializeField] private float turnMaxTime = 10.0f;
    [SerializeField] private float turnTimer = 0.0f;
    public void RunTurnStart(BeadObj beadObj) // 當滑鼠 點擊 珠子 呼叫此函數
    {
        // 狀態 從 Idle 變成 Touch
        if (state != FingerRunStates.Idle) return;
        state = FingerRunStates.Touch;
        Debug.Log("TurnStart " + beadObj.name);
        // 移動位置 & 改變起手珠色
        transform.position = Input.mousePosition;
        beadType = beadObj.beadType;
        // 參數重置
        touchCount = 0;
        touchBeadObj = null;
    }
    public void TurnEnd() // 當 滑鼠彈起珠子/轉珠時間到 呼叫此函數
    {
        // 狀態 從 Touch 變成 Idle
        if (state != FingerRunStates.Touch) return;
        state = FingerRunStates.Idle;
        Debug.Log("TurnEnd");
        // 移動位置
        ResetToInitPosition();
        if (touchBeadObj != null)
        {
            touchBeadObj.moveAnimationDirectlyEnd();
            touchBeadObj = null;
        }
        if (touchCount > 1)
        {
            Debug.Log("轉了" + (touchCount - 1) + "步");
            GameObject.Find("BoardControl").GetComponent<BoardControl>().ComboStart();
        }
    }
    #endregion

    #region 控制碰撞
    [SerializeField] private BeadObj touchBeadObj = null;
    [SerializeField] private int touchCount = 0;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (state == FingerRunStates.Idle) return;
        if (!other.CompareTag("BeadObj")) return;
        var otherBeadObj = other.GetComponent<BeadObj>();
        if (otherBeadObj == touchBeadObj) return;

        touchCount += 1;
        if (touchCount == 1)
        {
            touchBeadObj = otherBeadObj;
        }
        else if (touchCount == 2)
        {
            turnTimer = 0.0f; //碰觸到第2個開始計時
            var beforeBeadObj = touchBeadObj;
            touchBeadObj = otherBeadObj;
            touchBeadObj.moveAnimation(beforeBeadObj);
        }
        else if (touchCount > 2)
        {
            var beforeBeadObj = touchBeadObj;
            touchBeadObj = otherBeadObj;
            beforeBeadObj?.moveAnimationDirectlyEnd();
            touchBeadObj.moveAnimation(beforeBeadObj);
        }
    }
    #endregion

    #region Start() 和 Update()
    void Start()
    {
        state = FingerRunStates.Idle;
        SetInitPosition(-1, -1);
    }
    void Update()
    {
        timerText.text = "剩餘時間:" + (turnMaxTime - turnTimer).ToString("00.00") + "秒";
        pathText.text = "已走" + (touchCount - 1) + "步";
        positionText.text = "位置:" + (touchBeadObj != null ? "(" + touchBeadObj.init_pos.x + ", " + touchBeadObj.init_pos.y + ")" : "(?, ?)");

        switch (state)
        {
            case FingerRunStates.Auto:
                autoStepTimer += Time.deltaTime;
                if (autoStepTimer > 0.5f)
                {
                    autoStepTimer = 0.0f;
                    autoStepCount += 1;
                }
                if (autoStepCount < autoPosPathList.Count)
                {
                    autoDestPosition = GetPosition(autoPosPathList[autoStepCount].x, autoPosPathList[autoStepCount].y);
                    transform.position = Vector3.MoveTowards(transform.position, autoDestPosition, 1000 * Time.deltaTime);
                }
                else
                {   // (瞬間)
                    state = FingerRunStates.Idle;
                    ResetToInitPosition();
                    GameObject.Find("BoardControl").GetComponent<BoardControl>().ComboStart();
                }
                break;
            case FingerRunStates.Touch:
                transform.position = Input.mousePosition;
                if (touchCount > 1 && turnTimer < turnMaxTime)
                {
                    turnTimer += Time.deltaTime;
                    if (turnTimer >= turnMaxTime) // 瞬間
                    {
                        turnTimer = turnMaxTime;
                        TurnEnd();
                    }
                }
                break;
        }
    }
    #endregion
}
