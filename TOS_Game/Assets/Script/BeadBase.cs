using UnityEngine;
using UnityEngine.UI;

public class BeadBase : MonoBehaviour
{
    public void SetWorldPosition(Vector3 position)
    {
        transform.position = position;
    }

    #region 座標位置轉換定義
    public GameObject background = null;

    public Vector3 GetPosition(int x, int y)
    {
        var zoom = Mathf.Min(Screen.width / 540f, Screen.height / 960f);
        Vector2 bg_wh = new Vector2(540f, 450f);

        Vector2 bg_wp = ((Vector2)transform.position / zoom - (Vector2)transform.localPosition) * zoom;
        if (background != null)
        {
            bg_wp = (Vector2)background.transform.position;
        }
        return bg_wp - 0.5f * bg_wh * zoom + new Vector2(90f * (x + 0.5f), 90f * (y + 0.5f)) * zoom;
    }
    #endregion

    #region 初始座標定義
    [SerializeField] protected internal Vector2Int init_pos = Vector2Int.zero;
    public Vector3 InitPosition => GetPosition(init_pos.x, init_pos.y);
    public void SetInitPosition(int x, int y)
    {
        init_pos = new Vector2Int(x, y);
        transform.position = InitPosition;
    }
    private protected void ResetToInitPosition() => transform.position = InitPosition;
    #endregion

    #region 珠子顏色
    [SerializeField] private BeadType _beadType = BeadType.R;
    public BeadType beadType
    {
        get => _beadType;
        set
        {
            if (_beadType != value)
            {
                GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/" + value);
                _beadType = value;
            }
        }
    }
    protected void setRandomBeadType() => beadType = (BeadType)Random.Range(0, (int)BeadType.Null);
    #endregion

    #region 珠子不透明度
    [SerializeField] private float _opacity = 1.0f;
    public float opacity
    {
        get => _opacity;
        set
        {
            if (_opacity != value)
            {
                GetComponent<Image>().color = new Color(1, 1, 1, value);
                _opacity = value;
            }
        }
    }
    #endregion
    void Start()
    {
        //background = GameObject.Find("Background");
    }
}
/*
畫布 -> 背景 -> 珠子

螢幕 (w:1080,h:960)
畫布 (w:1080,h:960) wp(x:540, y:480) lp(x: 540, y: 480)
背景 (w: 540,h:450) wp(x:540, y:225) lp(x:   0, y:-255)
珠子 (w:  90,h: 90) wp(x:315, y: 45) lp(x:-225, y:-180) (r:0, c:0)
珠子 (w:  90,h: 90) wp(x:405, y: 45) lp(x:-135, y:-180) (r:0, c:1)

螢幕 (w: 540,h:960)
畫布 (w: 540,h:960) wp(x:270, y:480) lp(x: 270, y: 480)
背景 (w: 540,h:450) wp(x:270, y:225) lp(x:   0, y:-255)
珠子 (w:  90,h: 90) wp(x:  0, y:  0) lp(x:-270, y:-225) (r:-0.5, c:-0.5) 最左下角
珠子 (w:  90,h: 90) wp(x: 45, y: 45) lp(x:-225, y:-180) (r:0, c:0)
珠子 (w:  90,h: 90) wp(x:135, y: 45) lp(x:-135, y:-180) (r:0, c:1)

螢幕 (w: 270,h:480)
畫布 (w: 540,h:960) wp(x:135.0, y:240.0) lp(x: 135, y: 240) 縮放(0.5, 0.5)
背景 (w: 540,h:450) wp(x:135.0, y:112.5) lp(x:   0, y:-255)
珠子 (w:  90,h: 90) wp(x: 22.5, y: 22.5) lp(x:-225, y:-180) (r:0, c:0)
珠子 (w:  90,h: 90) wp(x:112.5, y: 22.5) lp(x:- 45, y:-180) (r:0, c:2)

//lp子=(wp子-wp父)/z; wp父=wp子-lp子*z; wp子=wp父+lp子*z;

新wp子 = wp父 + 新lp子*z
新wp子 =   (舊wp子-舊lp子*z) + 新lp子*z
新wp子 = z*(舊wp子/z-舊lp子) + 新lp子*z
新wp子 = z*(舊wp子/z-舊lp子) + [-0.5*父wh, +0.5*父wh]*z
新wp子 = z*(舊wp子/z-舊lp子) -0.5*父wh*z + [0*父wh, 1*父wh]*z
新wp子 = z*(舊wp子/z-舊lp子) -0.5*父wh*z + [0*父wh, 1*父wh]*z

bg_wp=(247.50, 202.50, 0.00) bg_wh=(540.00, 450.00, 0.00)
0.5*[2*( 22.5, 22.5)-(-225,-180)] = (135.0, 112.5)
0.5*[2*(112.5, 22.5)-(- 45,-180)] = (135.0, 112.5)

(22.5, 22.5)=0.5*[2*(135.0, 112.5)+(-225,-180)]
*/