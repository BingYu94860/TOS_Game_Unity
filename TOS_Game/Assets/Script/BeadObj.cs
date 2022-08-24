using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

struct BeadData // 定義需要覆寫的資料
{
    public BeadType beadType;
    public float opacity;
}

public enum BeadRunStates : int { Idle, Move, Remove, Fall, Wait };

public class BeadObj : BeadBase, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private BeadRunStates state;
    [SerializeField] private float timer;
    [SerializeField] private float speed;

    #region 移動 (嘗試)
    /*
    public void moveAnimation2((int x, int y) pos)
    {
        timer = 0.0f;
        state = BeadRunStates.Move;

        var distPosition = GetPosition(pos.x, pos.y);
        var moveDistance = Vector3.Distance(InitPosition, destBeadObj.InitPosition);
    }
    */
    #endregion

    #region 移動
    [SerializeField] private float moveUnitTime; // 在 Start 設定參數
    public void moveAnimation(BeadObj dist_beadObj) // 開始進入點
    {
        timer = 0.0f;
        state = BeadRunStates.Move;

        destBeadObj = dist_beadObj;
        var moveDistance = Vector3.Distance(InitPosition, destBeadObj.InitPosition);
        speed = moveDistance / moveUnitTime;
    }
    public void moveAnimationDirectlyEnd()
    {
        if (state != BeadRunStates.Move) return;
        state = BeadRunStates.Idle;
        timer = moveUnitTime;
        if (destBeadObj != null)
        {
            ResetToInitPosition();
            destBeadObj.ResetToInitPosition();
            SwapBeadObj(destBeadObj);
            destBeadObj = null;
        }
        else { Debug.Log("moveAnimtionDirectlyEnd Error"); }
    }
    private void SwapBeadObj(BeadObj beadObj)
    {
        //交換 珠色
        var temp_beadType = beadType; //temp <- me
        beadType = beadObj.beadType; //me <- other
        beadObj.beadType = temp_beadType; //other <- me
        //交換 珠子不透明度
        var temp_opacity = opacity;
        opacity = beadObj.opacity;
        beadObj.opacity = temp_opacity;
        //交換 移除狀態
        if (removeFlag != beadObj.removeFlag)
        {
            removeFlag = !removeFlag;
            beadObj.removeFlag = !beadObj.removeFlag;
        }
    }
    #endregion
    //==========<>==========<>==========<>==========<>==========<>==========<>==========<>==========//
    #region 消除
    [SerializeField] private float removeStartTime; // 呼叫removeAnimation函數傳入的參數
    [SerializeField] private float removeEndTime; // 呼叫removeAnimation函數傳入的參數
    [SerializeField] private float removeTotalTime; // 呼叫removeAnimation函數傳入的參數
    public bool removeFlag = false;
    public void removeAnimation(int icombo, int comboCount, float unitTime = 0.1f) // 開始進入點
    {
        timer = 0.0f;
        state = BeadRunStates.Remove;
        removeFlag = true;
        removeStartTime = unitTime * icombo;
        removeEndTime = unitTime * (icombo + 1);
        removeTotalTime = unitTime * comboCount;
        speed = 1.0f / (removeTotalTime - removeStartTime);
    }
    #endregion
    //==========<>==========<>==========<>==========<>==========<>==========<>==========<>==========//
    #region 落下
    [SerializeField] private float fallEndTime; // 呼叫fallAnimation函數設定的參數
    [SerializeField] private float fallTotalTime; // 呼叫fallAnimation函數設定的參數
    public void fallAnimation(int start_y, BeadObj dist_beadObj, int total_y, float unitTime = 0.1f)
    {
        timer = 0.0f;
        state = BeadRunStates.Fall;

        transform.position = GetPosition(init_pos.x, start_y); // 移動到落下點
        if (removeFlag == true) CreateNewRandomBead(); //  如果是被標記移除的，重新生成。

        var fallDistance = Vector3.Distance(transform.position, dist_beadObj.InitPosition);
        fallEndTime = unitTime * (start_y - dist_beadObj.init_pos.y);
        fallTotalTime = unitTime * total_y;
        speed = fallDistance / fallEndTime;

        dist_beadObj.copyInPreprocess(this);
        destBeadObj = dist_beadObj;
    }
    #endregion
    //==========<>==========<>==========<>==========<>==========<>==========<>==========<>==========//








    //==========<>==========<>==========<>==========<>==========<>==========<>==========<>==========//
    // 初始位置是固定，在動畫結束時，將在該位置上面的珠子內容，覆寫到下面實際位置的珠子上。
    // 來源 source      (src)  : 上方 動畫後的位置 (src_beadObj) 
    // 目的 destination (dest) : 下方 實際儲存位置 (this) 
    [SerializeField] private BeadObj srcBeadObj = null; // 動畫結束用
    [SerializeField] private BeadData srcBeadData;
    [SerializeField] private BeadObj destBeadObj = null; // 動畫移動用
    private void copyInPreprocess(BeadObj src_beadObj)
    {   // ex: destBeadObj.copyInPreprocess(srcBeadObj);
        srcBeadObj = src_beadObj;
        srcBeadData.beadType = src_beadObj.beadType;
        srcBeadData.opacity = src_beadObj.opacity;
    }
    protected void copyInRun()
    {   // ex: destBeadObj.copyInRun(); // destBeadObj <= srcBeadObj
        srcBeadObj = null;
        beadType = srcBeadData.beadType;
        opacity = srcBeadData.opacity;
    }



    public void CreateNewRandomBead()
    {
        opacity = 1.0f;
        setRandomBeadType();
    }

    #region 判斷滑鼠按下與彈起
    [SerializeField] private FingerObj finger;
    public void OnPointerDown(PointerEventData eventData) => finger.TurnStart(this);
    public void OnPointerUp(PointerEventData eventData) => finger.TurnEnd();
    #endregion

    #region Start() 和 Update()
    void Start()
    {
        finger = GameObject.Find("FingerObj").GetComponent<FingerObj>();

        //SetZoom(); //控制縮放比例

        moveUnitTime = 0.1f;
    }

    void Update()
    {
        switch (state)
        {
            case BeadRunStates.Move:
                timer += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, destBeadObj.InitPosition, speed * Time.deltaTime);

                if (transform.position == destBeadObj.InitPosition)
                    moveAnimationDirectlyEnd();
                if (timer >= moveUnitTime)
                    moveAnimationDirectlyEnd();
                break;

            case BeadRunStates.Remove:
                timer += Time.deltaTime;
                if (timer >= removeStartTime && timer < removeEndTime)
                {
                    opacity = Mathf.MoveTowards(opacity, 0.0f, speed * Time.deltaTime);
                }
                else if (timer >= removeEndTime && timer < removeTotalTime)
                {
                    opacity = 0.0f;
                }
                else if (timer >= removeTotalTime)
                {
                    state = BeadRunStates.Idle;
                }
                break;

            case BeadRunStates.Fall:
                timer += Time.deltaTime;

                if (timer >= 0 && timer < fallEndTime)
                { // 從計時開始就落下
                    transform.position = Vector3.MoveTowards(transform.position, destBeadObj.InitPosition, speed * Time.deltaTime);
                }
                else if (timer >= fallTotalTime)
                {
                    removeFlag = false;

                    if (destBeadObj != null)
                    {
                        destBeadObj.copyInRun();
                        destBeadObj = null;
                    }
                    else { Debug.Log("fallAnimationDirectlyEnd Error"); }
                    ResetToInitPosition();
                    state = BeadRunStates.Idle;
                }
                break;

            default:
                break;
        }
    }
    #endregion
}


/*
void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("FingerObj")) return;
    var fingerBeadObj = other.GetComponent<FingerObj>();
}
void OnTriggerExit2D(Collider2D other)
{
    if (!other.CompareTag("FingerObj")) return;
    var fingerBeadObj = other.GetComponent<FingerObj>();
}
*/