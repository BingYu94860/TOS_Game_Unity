using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class RandomButton : MonoBehaviour {
    private PuzzleSystem puzzleSystem;//PuzzleSystem.cs
    Button btn;
    Text btnText;
    void Start() {
        puzzleSystem = GameObject.Find("PuzzleSystem").GetComponent<PuzzleSystem>();
        btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
        btnText = btn.GetComponentInChildren<Text>();
    }

    private void BtnClick() {
        foreach (var orb in puzzleSystem.orbs) {
            orb.type = Orb.OrbsType.Null;
        }
        do {
            puzzleSystem.hasRemove = false;
            puzzleSystem.OrbInit();//【1】
            puzzleSystem.BoardCombo();//【2】
            puzzleSystem.OrbRemove(true);//【4】
        } while (puzzleSystem.hasRemove);//if orb was removed
    }
}
