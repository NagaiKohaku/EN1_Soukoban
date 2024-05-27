using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GamaManagerScript : MonoBehaviour
{
    //プレイヤーのプレハブ
    public GameObject playerPrefab;

    //箱のプレハブ
    public GameObject boxPrefab;

    //格納場所のプレハブ
    public GameObject goalPrefab;

    //パーティクルのプレハブ
    public GameObject particlePrefab;

    //壁のプレハブ
    public GameObject wallPrefab;

    //csvファイル
    public TextAsset csvFile;

    //ゴールテキストのプレハブ
    public GameObject clearText;

    //レベルデザイン用のマップ情報
    int[,] map;

    //処理用のマップ情報
    GameObject[,] field;

    bool isClear = false;

    //初期化
    void Start()
    {

        //スクリーンモードの設定
        Screen.SetResolution(1280, 720, false);

        LoadMap();

        //処理用マップにマップ情報を書き込む
        field = new GameObject
            [
            map.GetLength(0),
            map.GetLength(1)
            ];

        Reset();
    }

    //更新
    void Update()
    {

        if (!isClear)
        {
            //右矢印キーが押された場合
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                //プレイヤーの現在の位置を取得
                Vector2Int playerIndex = GetPlayerIndex();

                for (int i = 0; i < 5; i++)
                {
                    //パーティクルを発生させる
                    Instantiate(
                        particlePrefab,
                        IndexToPosition(new Vector2Int(GetPlayerIndex().x, GetPlayerIndex().y)),
                        Quaternion.identity
                    );
                }

                //プレイヤーを右側に一つ動かす
                MoveNumber(playerPrefab.tag, playerIndex, playerIndex + new Vector2Int(1, 0));
            }

            //左矢印キーが押された場合
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                //プレイヤーの現在の位置を取得
                Vector2Int playerIndex = GetPlayerIndex();

                for (int i = 0; i < 5; i++)
                {
                    //パーティクルを発生させる
                    Instantiate(
                        particlePrefab,
                        IndexToPosition(new Vector2Int(GetPlayerIndex().x, GetPlayerIndex().y)),
                        Quaternion.identity
                    );
                }

                //プレイヤーを左側に一つ動かす
                MoveNumber(playerPrefab.tag, playerIndex, playerIndex + new Vector2Int(-1, 0));
            }

            //上矢印キーが押された場合
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                //プレイヤーの現在の位置を取得
                Vector2Int playerIndex = GetPlayerIndex();

                for (int i = 0; i < 5; i++)
                {
                    //パーティクルを発生させる
                    Instantiate(
                        particlePrefab,
                        IndexToPosition(new Vector2Int(GetPlayerIndex().x, GetPlayerIndex().y)),
                        Quaternion.identity
                    );
                }

                //プレイヤーを下側に一つ動かす
                MoveNumber(playerPrefab.tag, playerIndex, playerIndex + new Vector2Int(0, -1));
            }

            //下矢印キーが押された場合
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                //プレイヤーの現在の位置を取得
                Vector2Int playerIndex = GetPlayerIndex();

                for (int i = 0; i < 5; i++)
                {
                    //パーティクルを発生させる
                    Instantiate(
                        particlePrefab,
                        IndexToPosition(new Vector2Int(GetPlayerIndex().x, GetPlayerIndex().y)),
                        Quaternion.identity
                    );
                }

                //プレイヤーを上側に一つ動かす
                MoveNumber(playerPrefab.tag, playerIndex, playerIndex + new Vector2Int(0, 1));
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {

                for (int y = 0; y < map.GetLength(0); y++)
                {
                    for (int x = 0; x < map.GetLength(1); x++)
                    {
                        Destroy(field[y, x]);
                        field[y, x] = null;
                    }
                }

                Reset();
            }
        }

        //もしクリアしたら
        if (IsCleard())
        {
            //ゲームオブジェクトのSetActiveメソッドを使い有効化
            clearText.SetActive(true);
            isClear = true;
        }

    }

    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(index.x - (map.GetLength(1) / 2) - 0.5f, -index.y + (map.GetLength(0) / 2), 0);
    }

    /// <summary>
    /// プレイヤーの現在地を探すメソッド
    /// </summary>
    /// <returns></returns>
    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                //何もない場合はスキップ
                if (field[y, x] is null)
                {
                    continue;
                }
                //タグがプレイヤーの場合のみ現在地を返す
                else if (field[y, x].tag == "Player")
                {
                    //現在地を返す
                    return new Vector2Int(x, y);
                }
            }
        }

        //プレイヤーがいなかったら-1を返す
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// オブジェクトを動かすメソッド
    /// </summary>
    /// <param name="tag">移動させるもののタグ</param>
    /// <param name="moveFrom">移動開始地点</param>
    /// <param name="moveTo">移動終了地点</param>
    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {
        //移動先がマップ範囲外であれば動かさない
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0))
        {
            return false;
        }

        //移動先がマップ範囲外であれば動かさない
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1))
        {
            return false;
        }

        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall")
        {
            return false;
        }

        //移動先に"2(箱)"があれば
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            //どの方向へ移動するかを算出
            Vector2Int velocity = moveTo - moveFrom;

            //プレイヤーの移動先から、さらに先へ2(箱)を移動させる
            bool success = MoveNumber(boxPrefab.tag, moveTo, moveTo + velocity);

            //もし2(箱)が移動失敗したら、プレイヤーも移動させない
            if (!success)
            {
                return false;
            }
        }

        //オブジェクトの座標を動かす
        Vector3 moveToPosition = IndexToPosition(new Vector2Int(moveTo.x, moveTo.y));

        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);

        //マップ情報内でも動かす
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        return true;
    }

    bool IsCleard()
    {
        //Vector2Int型の可変長配列の作成
        List<Vector2Int> goals = new List<Vector2Int>();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                //もし"3(格納場所)"であれば
                if (map[y, x] == 3)
                {
                    //格納場所のインデックスを控えておく
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {

                //ひとつでも箱がなかったら条件未達成
                return false;
            }
        }

        //条件未達成でなければ条件達成
        return true;
    }

    void Reset()
    {

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                //マップ情報の"1"の場所にプレイヤーのインスタンスを作成
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(
                        playerPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                    );
                }

                //マップ情報の"2"の場所にプレイヤーのインスタンスを作成
                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                        boxPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                    );
                }

                //マップ情報の"3"の場所に格納場所のインスタンスを作成
                if (map[y, x] == 3)
                {

                    field[y, x] = Instantiate(
                        goalPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                    );
                }

                if (map[y, x] == 4)
                {

                    field[y, x] = Instantiate(
                        wallPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                        );
                }
            }
        }
    }

    void LoadMap()
    {

        List<string[]> csvDatas = new List<string[]>();

        StringReader reader = new StringReader(csvFile.text);

        while(reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
        }

        map = new int[csvDatas.Count, csvDatas.Count];

        for(int i = 0; i < map.GetLength(0); i++)
        {
            for(int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j] = int.Parse(csvDatas[i][j]);
            }
        }
    }
}