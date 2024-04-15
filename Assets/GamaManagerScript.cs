using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamaManagerScript : MonoBehaviour
{
    //配列の宣言
    int[] map;

    // Start is called before the first frame update
    void Start()
    {
        //配列の実態の作成と初期化
        map = new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0 };

        //文字列の宣言と初期化
        string debugText = "";

        for (int i = 0;i < map.Length; i++)
        {
            debugText += map[i].ToString() + ",";
        }

        //要素数を一つずつ出力
        Debug.Log(debugText);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //見つからなかった時のために-1で初期化
            int playerIndex = -1;

            //要素数はmap.Lengthで取得
            for(int i = 0;i < map.Length; i++)
            {
                if (map[i] == 1)
                {
                    playerIndex = i;
                    break;
                }
            }
        }
    }
}
