using UnityEngine;
using UnityEngine.UI;

public class Orb : MonoBehaviour {
    public enum OrbsType : sbyte { R, B, G, H, Null };
    //宣告列舉OrbsType，包含所有珠子的屬性以及Null。
    public OrbsType type;           //宣告珠子的類型。

    public float width;
    public float height;
    
    public int row;
    public int col;
    public int id;

    public bool removed = false;//消除記號

    public enum OrbsState { Create, Change, Remove, Stay };
    public OrbsState state;

    void OnTriggerEnter2D(Collider2D other) {
        //當另一個對象進入連接到該對象的觸發對撞機時發送（僅限2D物理）
        if (other.CompareTag("Finger")) {
            image.color = new Color(1, 1, 1, 0);//起珠 原位置 隱藏
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Finger") && other.GetComponent<Finger>().moveState == false) {
            image.color = Color.white;
        }
    }

    public float removeAlpha;
    public float removeTime;
    private float timer = 0;
    //【3.Remove】
    public void RemoveAni() {
        timer += Time.deltaTime;
        if (timer > removeTime) {
            if (image.color.a > 0) {
                image.color = new Color(1, 1, 1, image.color.a - removeAlpha * Time.deltaTime);
            } else {
                state = OrbsState.Stay;//【4.Stay】
                timer = 0;
            }
        }        
    }
    public void SetAniPos(Vector2 dir, OrbsState state) {
        imageRect.anchoredPosition = new Vector2(width * dir.x, height * dir.y);
        this.state = state;
    }
    //【2.Change = Move】
    public void MoveAni() {
        //Debug.Log("\t\t\t MoveAni");
        image.color = Color.white;
        imageRect.anchoredPosition = Vector2.MoveTowards(imageRect.anchoredPosition, 
                                                         Vector2.zero, 
                                                         2000 * Time.deltaTime);
        if (imageRect.anchoredPosition == Vector2.zero)
            state = OrbsState.Stay;//【4.Stay】
    }

    private Sprite[] typeImage;
    public RectTransform imageRect;
    public Image image;
    void Start() {
        transform.name = "orb" + row + col;//物件名稱

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, height);
        
        typeImage = new Sprite[(int)OrbsType.Null];
        for (int i = 0; i < typeImage.Length; i++) {
            typeImage[i] = Resources.Load<Sprite>("Image/" + (OrbsType)i);//透過珠子的type更換圖片。
        }

        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.offset = new Vector2(width / 2, height / 2);
        boxCollider2D.size = new Vector2(width, height);

        imageRect.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        state = OrbsState.Create;//【1.Create】
    }
    void Update() {
        image.sprite = typeImage[(int)type];
        switch (state) {
            case OrbsState.Create:
            case OrbsState.Change:
                MoveAni();
                break;
            case OrbsState.Remove:
                RemoveAni();
                break;
        }
    }
}
