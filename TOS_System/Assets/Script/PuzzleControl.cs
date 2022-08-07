using UnityEngine;
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
