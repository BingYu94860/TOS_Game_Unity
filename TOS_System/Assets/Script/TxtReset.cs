using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class TxtReset : MonoBehaviour {
    private PuzzleSystem puzzleSystem;//PuzzleSystem.cs
    void Start() {
        puzzleSystem = GameObject.Find("PuzzleSystem").GetComponent<PuzzleSystem>();
        GetComponent<Button>().onClick.AddListener(Click);
    }


    void Click() {
        Debug.Log("2.回復版面");
        var text = File.ReadAllLines("OrbsFile.txt");//读取文件的所有行，并将数据读取到定义好的字符数组strs中，一行存一个单元
        var orbs = puzzleSystem.orbs;
        int i = 0;
        foreach (var item in text) {
            if (Enum.TryParse(item, out puzzleSystem.orbs[i].type)) {
                orbs[i].type = (Orb.OrbsType)Enum.Parse(typeof(Orb.OrbsType), item);
                i++;
            }
        }


    }
    void Update() { }
}
