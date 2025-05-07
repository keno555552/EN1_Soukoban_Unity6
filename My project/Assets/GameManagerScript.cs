using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour
{
    /// 定義
    // 配列を配置
    private int[,] map;
    private GameObject[,] objectMap;
    public GameObject playerPrefab;
    //[SerializeField, Range(1, 5)] private float speed;
    //[SerializeField] private float attack;
    //[SerializeField] private float health;
    public GameObject boxPrefab;
    public GameObject TargetBox = null;

    // パーティクルプレハブ割り当て用。パーティクルプレハブには
    // Particleコンポーネントが割り当てられてるはずなので、
    // Particle型で宣言する。
    public Particle particlePrefab;

    public GameObject clearText;

    //private void PrintArray()
    //{
    //    string debugText = "";
    //    for (int y = 0; y < map.GetLength(0); y++)
    //    {
    //        for (int x = 0; x < map.GetLength(1); x++)
    //        {
    //            // 文字列に変換して結合
    //            debugText += map[y, x].ToString() + ",";
    //        }
    //        // 改行を追加
    //        debugText += "\n";
    //    }
    //    // 結合した文字列を出力
    //    Debug.Log(debugText);
    //}

    bool IsCleard()
    {
        // Vector2Int形の可変長配列を作成
        List<Vector2Int> goals = new List<Vector2Int>();
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                // 格納場所が否かを判断
                if (map[y, x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        // 要素数はGoals.Countで取得
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = objectMap[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                // 一つでも箱がなかったら条件未達成
                return false;
            }
        }
        // 条件未達成でなければ条件達成
        return true;
    }

    private void CheckGameClearStatus()
    {
        // ゲームクリア判定
        if (IsCleard())
        {
            // ゲームオブジェクトのSetActiveメソッドを使い有効化
            clearText.SetActive(true);
        }
        else
        {
            // ゲームオブジェクトのSetActiveメソッドを使い無効化
            clearText.SetActive(false);
        }
    }

    /// <summary>
    /// 二次元配列のインデックスから、ワールド座標を取得する
    /// </summary>
    /// <param name="xIndex">インデックスのX</param>
    /// <param name="yIndex">インデックスのY</param>
    /// <returns>世界空間上の座標</returns>
    Vector3 GetPositionByIndex(int xIndex, int yIndex)
    {
        return new Vector3(xIndex, map.GetLength(0) - yIndex, 0);
    }


    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < objectMap.GetLength(0); y++)
        {
            for (int x = 0; x < objectMap.GetLength(1); x++)
            {
                if (objectMap[y, x] == null) { continue; }
                if (objectMap[y, x].tag == "Player")
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    bool MoveObject(Vector2Int moveFrom, Vector2Int moveTo)
    {
        // 二次元配列に対応
        if (moveTo.y < 0 || moveTo.y >= objectMap.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= objectMap.GetLength(1)) { return false; }

        // 配列外参照防止
        // Boxタグを持つていたら再帰関数
        if (objectMap[moveTo.y, moveTo.x] != null && objectMap[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveObject(moveTo, moveTo + velocity);
            if (!success) { return false; }
        }

        // 移動処理
        //objectMap[moveFrom.y, moveFrom.x].transform.position = GetPositionByIndex(moveTo.x, moveTo.y);
        Vector3 moveToPosition = new Vector3(moveTo.x, map.GetLength(0) - moveTo.y, 0);
        objectMap[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);

        objectMap[moveTo.y, moveTo.x] = objectMap[moveFrom.y, moveFrom.x];
        objectMap[moveFrom.y, moveFrom.x] = null;

        // パーティクルを5個生成
        for (int i = 0; i < 5; i++)
        {
            Particle particle = Instantiate(
                particlePrefab,
                GetPositionByIndex(moveFrom.x, moveFrom.y),
                Quaternion.identity);
        }

        return true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 解像度、ウィンドウ/フルスクリーンモードを設定
        Screen.SetResolution(1280, 720, false);

        //public GameObject playerPrefab;
        // 配列の実態の作成と初期化
        map = new int[,] {
                { 2,2,2,2,2,2,2},
                { 2,0,0,0,0,0,2},
                { 0,0,3,0,3,0,0},
                { 2,0,2,1,2,0,2},
                { 2,0,0,2,0,0,2},
                { 2,0,0,3,0,0,2},
                { 2,2,2,2,2,2,2} };

        //PrintArray();

        objectMap = new GameObject
        [
            map.GetLength(0),
            map.GetLength(1)
        ];

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    objectMap[y, x] = Instantiate(
                        playerPrefab,
                       GetPositionByIndex(x, y),
                        Quaternion.identity
                    );
                }
                if (map[y, x] == 2)
                {
                    objectMap[y, x] = Instantiate(
                        boxPrefab,
                        GetPositionByIndex(x, y),
                        Quaternion.identity
                    );
                }
                if (map[y, x] == 3)
                {
                    objectMap[y, x] = Instantiate(
                        TargetBox,
                        GetPositionByIndex(x, y),
                        Quaternion.identity
                    );
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveObject(
                playerIndex,
                playerIndex + new Vector2Int(1, 0));
            CheckGameClearStatus();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveObject(
                playerIndex,
                playerIndex - new Vector2Int(1, 0));
            CheckGameClearStatus();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveObject(
                playerIndex,
                playerIndex - new Vector2Int(0, 1));
            CheckGameClearStatus();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveObject(
                playerIndex,
                playerIndex + new Vector2Int(0, 1));
            CheckGameClearStatus();
        }

    }


}
