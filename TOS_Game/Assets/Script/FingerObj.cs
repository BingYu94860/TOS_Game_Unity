using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum FingerRunStates : int { Idle, Touch, Auto };

public class FingerObj : BeadBase
{
    FingerRunStates state = FingerRunStates.Idle;
    #region 給定起始座標和路徑 自動演示移動
    [SerializeField] private float autoTimer = 0.0f;
    [SerializeField] private int autoStepCount = 9999;
    [SerializeField] private Vector3 autoDestPosition = new Vector3();
    [SerializeField] private List<Vector2Int> autoPosPathList = new List<Vector2Int>();
    [SerializeField]
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
    public void RunAuto(Vector2Int pos, string pathStr)
    {
        state = FingerRunStates.Auto;
        autoTimer = 0.0f;
        autoStepCount = 0;
        autoPosPathList = GetPosPathList(pos, pathStr);
        transform.position = GetPosition(autoPosPathList[0].x, autoPosPathList[0].y);
        opacity = 1.0f;
    }
    #endregion


    public BoardControl boardControl;
    public Text timerText; //畫面第4行 //剩餘時間
    public Text positionText; //畫面第5-1行 //轉珠位置
    public Text pathText; //畫面第5-2行 //已走步數
    private bool _touchState = false;
    public bool touchState
    {
        get => _touchState;
        private set => _touchState = value;
    }
    [SerializeField] private int touchCount = 0;
    [SerializeField] private BeadObj cursorBeadObj = null;
    [SerializeField] private float turnMaxTime = 10.0f;
    [SerializeField] private float turnTimer = 0.0f;
    public void TurnStart(BeadObj beadObj) // 當滑鼠 點擊 珠子 呼叫此函數
    {
        if (state != FingerRunStates.Idle) return;
        if (touchState == true) return;
        state = FingerRunStates.Touch;
        beadObj.opacity = 0.1f;
        Debug.Log("TurnStart " + beadObj.name);
        touchState = true;
        opacity = 1.0f;

        transform.position = Input.mousePosition;
        touchCount = 0;
        beadType = beadObj.beadType;
        cursorBeadObj = null;
        
    }
    public void TurnEnd() // 當滑鼠 彈起 珠子 呼叫此函數
    {
        if (touchState == false) return;
        state = FingerRunStates.Idle;
        Debug.Log("TurnEnd");
        resetInit();
        if (touchCount > 1)
        {
            Debug.Log("轉了" + (touchCount - 1) + "步");
            boardControl.ComboStart();
        }
    }

    private void resetInit()
    {
        state = FingerRunStates.Idle;
        touchState = false;
        opacity = 0.0f;
        ResetToInitPosition();
        if (cursorBeadObj != null)
        {
            cursorBeadObj.moveAnimationDirectlyEnd();
            cursorBeadObj.opacity = 1.0f;
            cursorBeadObj = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if (touchState == false) return;
        if (state == FingerRunStates.Idle) return;
        if (!other.CompareTag("BeadObj")) return;
        var otherBeadObj = other.GetComponent<BeadObj>();
        if (otherBeadObj == cursorBeadObj) return;

        touchCount += 1;
        if (touchCount == 1)
        {
            cursorBeadObj = otherBeadObj;
        }
        else if (touchCount == 2)
        {
            turnTimer = 0.0f; //碰觸到第2個開始計時
            var beforeBeadObj = cursorBeadObj;
            cursorBeadObj = otherBeadObj;
            cursorBeadObj.moveAnimation(beforeBeadObj);
        }
        else if (touchCount > 2)
        {
            var beforeBeadObj = cursorBeadObj;
            cursorBeadObj = otherBeadObj;
            beforeBeadObj.moveAnimationDirectlyEnd();
            cursorBeadObj.moveAnimation(beforeBeadObj);
        }
    }

    void Start()
    {
        if (boardControl == null)
            boardControl = GameObject.Find("BoardControl").GetComponent<BoardControl>();

        SetInitPosition(-1, 0);
        resetInit();

    }

    void Update()
    {
        if (state == FingerRunStates.Auto)
        {
            autoTimer += Time.deltaTime;
            if (autoTimer > 0.5f)
            {
                autoTimer = 0.0f;
                autoStepCount += 1;
            }
            if (autoStepCount < autoPosPathList.Count)
            {
                touchState = true;
                autoDestPosition = GetPosition(autoPosPathList[autoStepCount].x, autoPosPathList[autoStepCount].y);
                transform.position = Vector3.MoveTowards(transform.position, autoDestPosition, 1000 * Time.deltaTime);
            }
            else
            {
                touchState = false;
                transform.position = GetPosition(-1, -1);
                opacity = 0.0f;
                boardControl.ComboStart();
                state = FingerRunStates.Idle;
            }
        }

        if (state == FingerRunStates.Touch)
        {
            timerText.text = "剩餘時間:" + (turnMaxTime - turnTimer).ToString("00.00") + "秒";

            pathText.text = "已走" + (touchCount + 1) + "步";
            if (cursorBeadObj != null)
            { positionText.text = "位置:(" + cursorBeadObj.init_pos.x + ", " + cursorBeadObj.init_pos.y + ")"; }
            else { positionText.text = "位置:(?, ?)"; }



            transform.position = Input.mousePosition;
            if (touchCount > 1 && turnTimer < turnMaxTime)
            {
                turnTimer += Time.deltaTime;
                if (turnTimer >= turnMaxTime) // 瞬間
                {
                    TurnEnd();
                }
            }

        }
    }
}
