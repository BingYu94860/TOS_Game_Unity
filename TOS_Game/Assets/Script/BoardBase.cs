using System.Collections.Generic;
using UnityEngine;

public class BoardBase : MonoBehaviour
{
    protected List<BeadObj> GetCreateBeadsInRect(RectTransform BGRect, int ROWS, int COLS)
    {   // 創建所有珠子
        List<BeadObj> beads = new List<BeadObj>();
        var prefabsObj = Resources.Load<GameObject>("Prefabs/BeadObj");
        for (var ir = 0; ir < ROWS; ir++) //y ROWS=5
        {
            for (var ic = 0; ic < COLS; ic++) //x COLS=6
            {
                var beadObj = Instantiate(prefabsObj, BGRect);
                beadObj.name = "bead_c" + ic + "_r" + ir;
                var bead = beadObj.GetComponent<BeadObj>();
                bead.SetInitPosition(ic, ir);
                bead.CreateNewRandomBead();
                beads.Add(bead);
            }
        }
        return beads;
    }

    protected BeadObj[,] GetBoardLinkBeads(List<BeadObj> beads, int ROWS, int COLS)
    {
        var board = new BeadObj[ROWS, COLS];
        foreach (var ibead in beads)
            board[ibead.init_pos.y, ibead.init_pos.x] = ibead;
        return board;
    }

    protected BeadType[,] GetBeadType2d(BeadObj[,] board)
    {
        var ROWS = board.GetLength(0);
        var COLS = board.GetLength(1);
        var beadType2d = new BeadType[ROWS, COLS];
        for (var ir = 0; ir < ROWS; ir++) //y ROWS=5
            for (var ic = 0; ic < COLS; ic++) //x COLS=6
                beadType2d[ir, ic] = board[ir, ic].beadType;
        return beadType2d;
    }
}
