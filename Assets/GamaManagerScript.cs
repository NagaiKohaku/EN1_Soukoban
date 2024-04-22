using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamaManagerScript : MonoBehaviour
{
    //プレイヤーのプレハブ
    public GameObject playerPrefab;

    //レベルデザイン用のマップ情報
    int[,] map;

    //処理用のマップ情報
    GameObject[,] field;

    //初期化
    void Start()
    {
        //マップの作成と初期化
        map = new int[,] {
            { 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        //処理用マップにマップ情報を書き込む
        field = new GameObject
            [
            map.GetLength(0),
            map.GetLength(1)
            ];

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                //マップ情報の"1"の場所にプレイヤーのインスタンスを作成
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(
                        playerPrefab,
                        new Vector3(
                            x - (map.GetLength(1) / 2) - 0.5f,
                            -y + (map.GetLength(0) / 2),
                            0),
                        Quaternion.identity
                        );
                }
            }
        }
    }

    //更新
    void Update()
    {
        //右矢印キーが押された場合
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //プレイヤーの現在の位置を取得
            Vector2Int playerIndex = GetPlayerIndex();

            //プレイヤーを右側に一つ動かす
            MoveNumber(playerPrefab.tag, playerIndex, playerIndex + new Vector2Int(1, 0));
        }

        //左矢印キーが押された場合
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //プレイヤーの現在の位置を取得
            Vector2Int playerIndex = GetPlayerIndex();

            //プレイヤーを左側に一つ動かす
            MoveNumber(playerPrefab.tag, playerIndex, playerIndex + new Vector2Int(-1, 0));
        }

        //上矢印キーが押された場合
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //プレイヤーの現在の位置を取得
            Vector2Int playerIndex = GetPlayerIndex();

            //プレイヤーを上側に一つ動かす
            MoveNumber(playerPrefab.tag, playerIndex, playerIndex + new Vector2Int(0, 1));
        }

        //下矢印キーが押された場合
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //プレイヤーの現在の位置を取得
            Vector2Int playerIndex = GetPlayerIndex();

            //プレイヤーを下側に一つ動かす
            MoveNumber(playerPrefab.tag, playerIndex, playerIndex + new Vector2Int(0, -1));
        }


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
    /// <param name="number">移動量</param>
    /// <param name="moveFrom">移動開始地点</param>
    /// <param name="moveTo">移動終了地点</param>
    /// <returns></returns>
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

        ////移動先に"2(箱)"があれば
        //if (map[moveTo] == 2)
        //{
        //    //どの方向へ移動するかを算出
        //    int velocity = moveTo - moveFrom;

        //    //プレイヤーの移動先から、さらに先へ2(箱)を移動させる
        //    bool success = MoveNumber(2, moveTo, moveTo + velocity);

        //    //もし2(箱)が移動失敗したら、プレイヤーも移動させない
        //    if (!success)
        //    {
        //        return false;
        //    }
        //}

        //オブジェクトの座標を動かす
        field[moveFrom.y, moveFrom.x].transform.position = new Vector3(moveTo.x - (map.GetLength(1) / 2) - 0.5f, moveTo.y - (map.GetLength(0) / 2), 0);

        //マップ情報内でも動かす
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        return true;
    }
}