using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    /// 定義
    // 配列を配置
    int[] map;

    private void PrintArray()
    {
        string debugText = "";
        for (int i = 0; i < map.Length; i++)
        {
            // 文字列に変換して結合
            debugText += map[i].ToString() + ",";
        }
        // 結合した文字列を出力
        Debug.Log(debugText);
    }

    int GetPlayerIndex()
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == 1)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 移動する数値を指定のインデックスに移動させる
    /// </summary>
    /// <param name="number">移動したい数字</param>
    /// <param name="box">ボックスの数字</param>
    /// <param name="moveFrom">どこから移動</param>
    /// <param name="moveTo">移動したい場所</param>
    /// <returns></returns>
    bool MoveNumber(int number, int box, int moveFrom, int moveTo)
    {
        // 移動先が範囲外なら移動不可
        if (moveTo < 0 || moveTo >= map.Length) { return false; }
        // 移動先に2(箱)が居たら
        if (map[moveTo] == box)
        {
            // どの方向へ移動するかを算出
            int velocity = moveTo - moveFrom;
            // プレイヤーの移動先から、さらに先へ2(箱)を移動させる。
            // 箱の移動処理。MoveNumberメソッド内でMoveNumberメソッドを
            // 呼び、処理が再帰している。移動不可かをboolで記録
            bool sussess = MoveNumber(box, box, moveTo, moveTo + velocity);
            // もし箱が移動失敗したら、プレイヤーの移動も失敗
            if (!sussess) { return false; }
        }
        // プレイヤー・箱関わらずの移動処理
        map[moveTo] = number;
        map[moveFrom] = 0;
        return true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 配列の実態の作成と初期化
        map = new int[] { 0, 0, 2, 1, 0, 2, 0, 2, 0 }; // 配列の初期化
        PrintArray();

        //// Debug.Log("Hello world"); // 配列の初期化確認
        //string debugText = "";
        //for (int i = 0; i < map.Length; i++)
        //{g
        //    ////　要素数を一つずつ出力
        //    //Debug.Log(map[i] + ",");
        //
        //    // 文字列に変換して結合
        //    debugText += map[i].ToString() + ",";
        //}
        //
        //// 結合した文字列を出力
        //Debug.Log(debugText);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 1. をここから記載
            // 見つからなかったのために-1で初期化
            int playerIndex = GetPlayerIndex();
            //　要素数をmap.Lengthで取得
            for (int i = 0; i < map.Length; i++)
            {
                if (map[i] == 1)
                {
                    playerIndex = i;
                    break;

                }
            }

            /*
               playerIndex+1のインデックスの物と交換するので、
               playerIndex-1よりさらに小さいインデックスの時
               のみ交換処理を行なう
             */
            MoveNumber(1, 2, playerIndex, playerIndex + 1);
            PrintArray();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            int playerIndex = GetPlayerIndex();
            for (int i = 0; i < map.Length; i++)
            {
                if (map[i] == 1)
                {
                    playerIndex = i;
                    break;

                }
            }

            /*
               playerIndex+1のインデックスの物と交換するので、
               playerIndex-1よりさらに小さいインデックスの時
               のみ交換処理を行なう
             */
            MoveNumber(1, 2, playerIndex, playerIndex - 1);
            PrintArray();
        }
    }


}
