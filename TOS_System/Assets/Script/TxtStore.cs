using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TxtStore : MonoBehaviour {
    private PuzzleSystem puzzleSystem;//PuzzleSystem.cs
    private const int ROWS = 5;
    private const int COLS = 6;
    // Start is called before the first frame update 在第一幀更新之前調用開始
    void Start() {
        puzzleSystem = GameObject.Find("PuzzleSystem").GetComponent<PuzzleSystem>();
        GetComponent<Button>().onClick.AddListener(Click);
    }
    public void Click() {
        Debug.Log("1.儲存版面");

        string[] orbsStrArr = new string[ROWS];
        for (int i = 0; i < puzzleSystem.orbs.Count; i++) {
            var inv_row = (ROWS - 1) - (i / COLS);
            Orb orb = puzzleSystem.orbs[i];
            orbsStrArr[inv_row] += (orb.type + " ");
        }
        using (StreamWriter sw = new StreamWriter("OrbsFileView.txt")) {
            foreach (var orbsStr in orbsStrArr) {
                sw.WriteLine(orbsStr);
            }            
        }
        using (StreamWriter sw = new StreamWriter("OrbsFile.txt")) {//"Assets/OrbsFile.txt"
            foreach (Orb o in puzzleSystem.orbs) {
                sw.WriteLine(o.type);
            }
        }

    }
    void Update() { }
}
