using UnityEngine;

public class BoardData // 定義需要覆寫的資料
{
    public int ROWS = 5;
    public int COLS = 6;
    public BeadType[] beadTypes;

    public BoardData(BeadObj[,] board)
    {
        ROWS = board.GetLength(0);
        COLS = board.GetLength(1);
        beadTypes = new BeadType[ROWS * COLS];
        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS; c++)
                beadTypes[r * COLS + c] = board[r, c].beadType;
    }
    public BoardData(string saveDataStr)
    {
        JsonUtility.FromJsonOverwrite(saveDataStr, this);
    }
    public string GetSaveJsonString()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadToBoard(BeadObj[,] board)
    {
        for (int r = 0; r < ROWS; r++)
            for (int c = 0; c < COLS; c++)
                board[r, c].beadType = beadTypes[r * COLS + c];
    }
}
