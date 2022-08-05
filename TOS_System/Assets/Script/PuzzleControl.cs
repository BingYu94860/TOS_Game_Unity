using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PuzzleControl : MonoBehaviour {
    private PuzzleSystem puzzleSystem;//PuzzleSystem.cs
    
    
    public enum PuzzleState { Standby, Start, Move, End, Confirm };
    private readonly string[] stateString = {"待機中...", "準備開始", "移動中...","轉珠結束", "Combo計算" };
    public PuzzleState state;
    public Canvas canvas;
    public RectTransform BGRect;
    public RectTransform fingerRect;
    public Finger finger;//Finger.cs


    public Text statusText;
    public Text timerText;
    public Text scoreText;
    public Text textText;
    public Text pathText;
    public Text positionText;//顯示轉珠位置

    private Vector3 inputPos;
    private Vector3 fingerPos;
    private bool touchState = false;

    /*
    public float baseWidth = 720;       //開發時定義的基礎解析度 寬度 //540
    public float baseHeight = 1280;     //開發時定義的基礎解析度 高度 //960
    private float baseAspect;           //開發時定義的基礎解析度 長寬比
    private float targetWidth;          //實際顯示畫面的解析度 寬度
    private float targetHeight;         //實際顯示畫面的解析度 高度
    private float targetAspect;         //實際顯示畫面的 長寬比
    //https://godstamps.blogspot.com/2014/05/unityViewAutoScale.html?fbclid=IwAR1zZ0CZ6NeHXWL5AU72x1kmL2-6S8uT2bf0pKmthgxykwKoTkZcb6dlgMc
    //未完成 長寬比 的 縮放
    void Awake() {
        targetWidth = (float)Screen.width;
        targetHeight = (float)Screen.height;

        this.baseAspect = this.baseWidth / this.baseHeight;// = 720/1280 = 9 / 16
        this.targetAspect = targetWidth / targetHeight;

        float factor = this.targetAspect > this.baseAspect ? targetHeight / this.baseHeight : targetWidth / this.baseWidth;
    }
    */

    //【1.Standby】
    void StandbyControl() {
        //Debug.Log("\t\t\t\t\t1.【StandbyControl】");
        if (touchState) {
            if (fingerPos.x < BGRect.sizeDelta.x && fingerPos.x > 0 &&
                fingerPos.y < BGRect.sizeDelta.y && fingerPos.y > 0)
                state = PuzzleState.Start;
        }
    }
    //【2.Start】
    void StartControl() {
        //Debug.Log("\t\t\t\t\t2.【StartControl】");
        if (touchState) {
            
            fingerRect.anchoredPosition = new Vector2(
                          Mathf.Clamp(fingerPos.x, 5, BGRect.sizeDelta.x - 5),
                          Mathf.Clamp(fingerPos.y, 5, BGRect.sizeDelta.y - 5));
                          
            if (finger.moveState)
                state = PuzzleState.Move;
        } else {
            finger.FingerInit();
            state = PuzzleState.Standby;
        }

    }

    public float puzzleTime = 4;
    public float moveTimer = 0;
    //【3.Move】
    void MoveControl() {
        //Debug.Log("\t\t\t\t\t3.【MoveControl】");
        moveTimer += Time.deltaTime;
        //Debug.Log("\t\t\t\t\t3.【MoveControl】" + Time.deltaTime + " | " + moveTimer);
        if (touchState) {
            //Debug.Log("\t\t\t\t\t fingerRect.anchoredPosition = (" + fingerRect.anchoredPosition.x + ", " + fingerRect.anchoredPosition.y + ")");
            //Debug.Log("\t\t\t\t\t inputPos = (" + inputPos.x + ", " + inputPos.y + ") ");
            //Debug.Log("\t\t\t\t\t fingerPos = (" + fingerPos.x + ", " + fingerPos.y + ") ");
            //Debug.Log("\t\t\t\t\t BGRect.sizeDelta = (" + BGRect.sizeDelta.x + ", " + BGRect.sizeDelta.y + ")");
            // fingerRect.anchoredPosition = 限制在 (5, 5) ~ (715, 595)
            // fingerPos = inputPos * 2 約= fingerRect.anchoredPosition
            // BGRect.sizeDelta = (720, 600)
            Vector2 pos = Vector2.MoveTowards(
                fingerRect.anchoredPosition,
                new Vector2(Mathf.Clamp(fingerPos.x, 5, BGRect.sizeDelta.x - 5),
                            Mathf.Clamp(fingerPos.y, 5, BGRect.sizeDelta.y - 5)),
                100);
            fingerRect.anchoredPosition = pos;
        } else {
            //GameObject.Find("PuzzleSystem").GetComponent<PuzzleSystem>().orbs[finger.nowOrb.id].GetComponent<Orb>().image.color = Color.white;
            puzzleSystem.orbs[finger.cursorOrb.id].GetComponent<Orb>().image.color = Color.white;
            finger.FingerInit();
            state = PuzzleState.End;
            moveTimer = 0;
        }
        if (moveTimer >= puzzleTime) {
            finger.FingerInit();
            state = PuzzleState.End;
            moveTimer = 0;
        }
    }
    //【4-1.End】
    void FindCombo() {
        //Debug.Log("\t\t\t\t\t4.【FindCombo】");
        puzzleSystem.OrbInit();//clear clear orb's state(group removed)
        puzzleSystem.BoardCombo();
        Invoke("OrbReset", puzzleSystem.removeTime);//remove and init orb when animation finish
        state = PuzzleState.Confirm;
    }
    //【4-2.Reset】
    void OrbReset() {
        //Debug.Log("\t\t\t\t\t0.【OrbReset】");
        puzzleSystem.OrbRemove(false);
        puzzleSystem.OrbInit();
        if (puzzleSystem.hasRemove) {
            Invoke("FindCombo", 0.5f);
            puzzleSystem.hasRemove = false;
        } else state = PuzzleState.Standby;
    }

    void Start () {
        //Debug.Log("\t\t\t\t\t【Start】");
        puzzleSystem = GameObject.Find("PuzzleSystem").GetComponent<PuzzleSystem>();
        state = PuzzleState.Standby;
    }
    void FixedUpdate() {
        //Debug.Log("\t\t\t\t\t【FixedUpdate】");
        switch (state) {
            case PuzzleState.Standby:
                StandbyControl();
                break;
            case PuzzleState.Start:
                StartControl();
                break;
            case PuzzleState.Move:
                MoveControl();
                break;
            case PuzzleState.End:
                FindCombo();
                break;
        }
    }
    /*
    //版面 對應轉換座標
    Vector2 WorldToRect() {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, inputPos);
        var x1 = screenPoint.x;
        var y1 = screenPoint.y;
        var xx = BGRect.sizeDelta.x;
        var yy = BGRect.sizeDelta.y;
        Debug.Log("\t\t\t Camera = " + Camera.main);
        Debug.Log("\t\t\t screenPoint x = " + x1 + " , y = " + y1);
        Debug.Log("\t\t\t BGRect x = " + xx + " , y = " + yy);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(BGRect, screenPoint, Camera.main, out Vector2 Pos);
        return Pos;
    }

    public  Vector2 WorldToRect2() {
        
        //UIRoot root = GameObject.FindObjectOfType<UIRoot>();
        //var h = (float)root.activeHeight;
        //var w = (float)root.activeWidth;
        //Debug.Log("\t\t\t 大小 w = " + f + " , h = " + w);
        
        Debug.Log("\t\t\t 螢幕大小 w = " + Screen.width + " , h = " + Screen.height + "\t| BGRect x = " + BGRect.sizeDelta.x + " , y = " + BGRect.sizeDelta.y);
        Debug.Log("\t\t\t 輸入座標 x = " + Input.mousePosition.x + " , y = " + Input.mousePosition.y);
        Vector2 v11 = Camera.main.WorldToScreenPoint(Input.mousePosition);
        Vector2 v12 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("\t\t\t 世界to螢幕 x = " + v11.x + " , y = " + v11.y + "\t| 螢幕to世界 x = " + v12.x + " , y = " + v12.y);
        Vector2 v21 = Camera.main.WorldToViewportPoint(Input.mousePosition);
        Vector2 v22 = Camera.main.ViewportToWorldPoint(Input.mousePosition);
        Debug.Log("\t\t\t 世界to視窗 x = " + v21.x + " , y = " + v21.y + "\t| 視窗to世界 x = " + v22.x + " , y = " + v22.y);
        Vector2 v31 = Camera.main.ViewportToScreenPoint(Input.mousePosition);
        Vector2 v32 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Debug.Log("\t\t\t 視窗to螢幕 x = " + v31.x + " , y = " + v31.y + "\t| 螢幕to視窗 x = " + v32.x + " , y = " + v32.y);

        //世界to螢幕
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, Input.mousePosition);
        
        Debug.Log("\t\t\t screenPoint x = " + screenPoint.x + " , y = " + screenPoint.y);
        //Debug.Log("\t\t\t Get = " + GetComponentsInParents<Canvas>());
        Vector2 Pos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(BGRect, screenPoint, Camera.main, out Pos);
        Debug.Log("\t\t\t Pos x = " + Pos.x + " , y = " + Pos.y);
        return Pos;
    }
    */
    /*
    public static Vector3 WorldToScreenPoint(Vector3 SetWorldPostion) {
        //Vector2 UICameraPostion = Vector2.zero;  // 先宣告一個回傳Vector2.
        RectTransform CanvasRectTransform = m_CanvasObj.GetComponent();  // 取得Canvas的RectTransform.
        Vector2 ScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, SetWorldPostion);
        // 將照射3D物件的社Camera指定進去， 並將要轉換的3D座標輸入進去
        // 就可以得到3D Camera轉換為該Space下的Screen Space.

        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRectTransform, ScreenPos, UICamera, out Vector2 UICameraPostion);
        // 最後用這個API將Screen Space轉換為UGUI的座標，分別將UI Canvas的RectTransform傳入
        // 後面依序是要轉換的ScreenPostion跟UI Camera以及輸到到那一個變數.　

        return UICameraPostion;
    }*/
    void Update() {
        //Debug.Log("\t\t\t\t\t【Update】");
        statusText.text = "狀態:" + stateString[(int)state];
        timerText.text = "剩餘時間:" + (puzzleTime - moveTimer).ToString("00.00") + "秒";
        scoreText.text = "得分:" + puzzleSystem.point;
        pathText.text = "已走" + finger.setpCount + "步";
        if (finger.cursorOrb != null)
            positionText.text = "位置:(" + finger.cursorOrb.col + ", " + finger.cursorOrb.row + ")";
        else { positionText.text = "位置:(?, ?)"; }
        /*Debug.Log("\t\t\t\t\t【Update】" +
                          " inputPos = (" + inputPos.x + ", " + inputPos.y + ", " + inputPos.z + ")" +
                          " fingerPos = (" + fingerPos.x + ", " + fingerPos.y + ", " + fingerPos.z + ")");*/

#if UNITY_EDITOR || UNITY_EDITOR_WIN

        if (Input.GetMouseButton(0)) {
            touchState = true;
            inputPos = Input.mousePosition;
            fingerPos = Input.mousePosition;
            //fingerPos = WorldToRect2();
            
        } else if (Input.GetMouseButtonUp(0)) {
            //Debug.Log("\t\t\t\t\t【Update】-1-2");
            touchState = false;
        } else {
            //Debug.Log("\t\t\t\t\t【Update】-1-3");
        }
        /*
        Debug.Log("\t\t\t\t\t【Update】-1-1" +
                              " inputPos = (" + inputPos.x + ", " + inputPos.y + ", " + inputPos.z + ")" +
                              " fingerPos = (" + fingerPos.x + ", " + fingerPos.y + ", " + fingerPos.z + ")");*/
        textText.text = "Play in Windows 平台上的Unity 編輯器.";
#elif UNITY_STANDALONE_WIN
        if (Input.GetMouseButton(0)) {
            touchState = true;
            inputPos = Input.mousePosition;
            //fingerPos = WorldToRect();
            fingerPos = Input.mousePosition;
            //fingerPos.y = fingerPos.y * BGRect.sizeDelta.y / Input.mousePosition.y;
            //fingerPos.y = fingerPos.y * Input.mousePosition.y / BGRect.sizeDelta.y;
            //fingerPos.y = fingerPos.y * BGRect.sizeDelta.x / Input.mousePosition.x;
            //fingerPos.y = fingerPos.y * Input.mousePosition.x / BGRect.sizeDelta.x;
        } else if (Input.GetMouseButtonUp(0)) {
            touchState = false;
        } else {
        }
        //Play in Windows 應用.
        /*
        textText.text = ("(" + Screen.width + "," + Screen.height + ")(" +
        Input.mousePosition.x + "," + Input.mousePosition.y + ")(" + 
        fingerPos.x + "," + fingerPos.y + ")(" + 
        BGRect.sizeDelta.x + "," + BGRect.sizeDelta.y + ")"
        );
        */
        textText.text = "Play in Windows 應用.";
#elif UNITY_WSA
        textText.text = "Play in Windows 商城應用.";
#elif UNITY_ANDROID
        textText.text = "Play in Android.";
#elif UNITY_IOS
        textText.text = "Play in IOS.";
#else
        textText.text = "Play is Error.";
#endif
        //textText.text = "(" + inputPos.x + ", " + inputPos.y + ", " + inputPos.z + ")";
    }
}
