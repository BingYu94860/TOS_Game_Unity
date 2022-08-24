using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

    protected bool[,] FindCanRemove2d(BeadType[,] beadType2d)
    {
        var ROWS = beadType2d.GetLength(0);
        var COLS = beadType2d.GetLength(1);
        var beadNull = BeadType.Null;
        var canRemove2d = new bool[ROWS, COLS];
        // 水平掃描 x
        for (var ir = 0; ir < ROWS - 2; ir++) //y ROWS=5
            for (var ic = 0; ic < COLS; ic++) //x COLS=6
            {
                var b0 = beadType2d[ir, ic];
                var b1 = beadType2d[ir + 1, ic];
                var b2 = beadType2d[ir + 2, ic];
                if (b0 == b1 && b1 == b2 && b0 != beadNull)
                {
                    canRemove2d[ir, ic] = true;
                    canRemove2d[ir + 1, ic] = true;
                    canRemove2d[ir + 2, ic] = true;
                }
            }
        // 垂直掃描 y
        for (int ir = 0; ir < ROWS; ir++) //y ROWS=5
            for (int ic = 0; ic < COLS - 2; ic++) //x COLS=6
            {
                var b0 = beadType2d[ir, ic];
                var b1 = beadType2d[ir, ic + 1];
                var b2 = beadType2d[ir, ic + 2];
                if (b0 == b1 && b1 == b2 && b0 != beadNull)
                {
                    canRemove2d[ir, ic] = true;
                    canRemove2d[ir, ic + 1] = true;
                    canRemove2d[ir, ic + 2] = true;
                }
            }
        return canRemove2d;
    }

    protected List<List<(int r, int c)>> FindSingleMatches(BeadType[,] beadType2d, bool[,] canRemove2d)
    {
        var isRemove2d = (bool[,])canRemove2d.Clone();
        var ROWS = beadType2d.GetLength(0);
        var COLS = beadType2d.GetLength(1);
        // Flood fill
        var matches = new List<List<(int r, int c)>>();
        for (var ir = 0; ir < ROWS; ir++) //y ROWS=5
        {
            for (var ic = 0; ic < COLS; ic++) //x COLS=6
            {
                if (isRemove2d[ir, ic] == false) continue;
                // 以下 canRemove2d[r, c] 皆是標記可移除的
                var matche = new List<(int r, int c)>();
                var stack = new Stack<(int r, int c)>();
                stack.Push((ir, ic));
                var orb_cur = beadType2d[ir, ic];
                while (stack.Count != 0)
                {
                    var p = stack.Pop();
                    if (beadType2d[p.r, p.c] != orb_cur || isRemove2d[p.r, p.c] == false) continue;
                    // 以下 orb2d[p.r, p.c] 皆是同珠色, canRemove2d[p.r, p.c] 皆是標記可移除的
                    matche.Add(p);
                    isRemove2d[p.r, p.c] = false;
                    // 將上下左右的4個位置加入stack
                    if (p.r < ROWS - 1)//↑
                        stack.Push((p.r + 1, p.c));
                    if (p.r > 0)//↓
                        stack.Push((p.r - 1, p.c));
                    if (p.c > 0)//←
                        stack.Push((p.r, p.c - 1));
                    if (p.c < COLS - 1)//→
                        stack.Push((p.r, p.c + 1));
                }// End of while(stack is not Empty)
                matches.Add(matche);
            }
        }
        return matches;
    }

}
