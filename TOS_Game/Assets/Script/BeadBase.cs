using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BeadBase : MonoBehaviour
{
    #region 控制縮放比例
    [SerializeField] private Vector2 zoom = Vector2.one; // sizeDelta
    protected void SetZoom()
    {   // 假設 UI座標 和 世界座標 的原點都一樣
        Vector2 position1 = GetComponent<RectTransform>().anchoredPosition; //UI 座標
        Vector2 position2 = GetComponent<RectTransform>().position; // 世界座標
        zoom = position2 / position1;
    }
    #endregion

    #region 座標位置轉換定義
    [SerializeField] private float width = 90f;
    [SerializeField] private float height = 90f;
    public Vector3 GetDirectionDistance(float x, float y)
    {
        SetZoom();
        return new Vector3(zoom.x * width * x, zoom.y * height * y, 0.0f);
    }
    public Vector3 GetPosition(int x, int y) => GetDirectionDistance(x + 0.5f, y + 0.5f);
    #endregion

    #region 初始座標定義
    [SerializeField] protected internal Vector2Int init_pos = Vector2Int.zero;
    private protected Vector3 InitPosition => GetPosition(init_pos.x, init_pos.y);
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

}
