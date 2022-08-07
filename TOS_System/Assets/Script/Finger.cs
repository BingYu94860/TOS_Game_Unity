using UnityEngine;
using UnityEngine.UI;
public class Finger : MonoBehaviour {
    private PuzzleSystem puzzleSystem;//PuzzleSystem.cs

    private RectTransform rect;
    public Vector2 initPos; // = (-1, -1)
    private Image image;
    public int setpCount = 0;
    /*
      image = GetComponent<Image>();
      image.sprite = Resources.Load<Sprite>("Image/" + otherOrb.type);
      image.color = display;
         */

    private Color display = new Color(1, 1, 1, 0.5f);
    private Color hide = new Color(1, 1, 1, 0);

    public Orb cursorOrb;
    public bool moveState;

    void OnTriggerEnter2D(Collider2D other) {
        var otherOrb = other.GetComponent<Orb>();
        //Debug.Log("name = " + otherOrb.name + ", row = " + otherOrb.row + ", col = " + otherOrb.col);
        
        if (other.CompareTag("Orb")) {
            if (cursorOrb == null || cursorOrb == otherOrb) {
                cursorOrb = otherOrb;
                setpCount = 0;
            } else {
                setpCount++;
                moveState = true;

                var temp = otherOrb.type;
                otherOrb.type = puzzleSystem.orbs[cursorOrb.id].type;
                puzzleSystem.orbs[cursorOrb.id].type = temp;

                //新位置 - 舊位置
                //Debug.Log("x = " + otherOrb.col + "-" + nowOrb.col + ", y = " + otherOrb.row + "-" + nowOrb.row);
                cursorOrb.SetAniPos(new Vector2(otherOrb.col - cursorOrb.col,
                                             otherOrb.row - cursorOrb.row), 
                                 Orb.OrbsState.Change);
                
                cursorOrb = otherOrb;
                
            }
            //Debug.Log("(" + cursorOrb.row + ", " + cursorOrb.col + ")");
            image.sprite = Resources.Load<Sprite>("Image/" + otherOrb.type);
            image.color = display;
        }
    }
    public void FingerInit() {
        rect.anchoredPosition = initPos;
        image.color = hide;
        cursorOrb = null;
        moveState = false;
    }
    void Start() {
        puzzleSystem = GameObject.Find("PuzzleSystem").GetComponent<PuzzleSystem>();

        rect = GetComponent<RectTransform>();

        var width = puzzleSystem.BGRect.sizeDelta.x / puzzleSystem.COLS;// = 720 / 6 = 80;
        var height = puzzleSystem.BGRect.sizeDelta.y / puzzleSystem.ROWS;// = 400 / 5 = 80;
        rect.sizeDelta = new Vector2(width, height);

        image = GetComponent<Image>();
        FingerInit();
    }
}
