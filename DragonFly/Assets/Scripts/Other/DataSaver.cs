using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataSaver : MonoBehaviour
{
    string rankingDataPath = "/Data/RankingData.csv";
    string latestDataPath = "/Data/LatestData.csv";

    /// <summary>
    /// ファイル・ディレクトリの存在確認
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="length">ランキングデータの配列数</param>
    void FileCheck(string fileName, int length)
    {
        string directoryName = Application.persistentDataPath + "/Data";
        string latestFile = Application.persistentDataPath + latestDataPath;
        string rankingFile = Application.persistentDataPath + rankingDataPath;

        while (!Directory.Exists(directoryName)) //ディレクトリがなかったら
        {
            Directory.CreateDirectory(directoryName); //ディレクトリを作成
        }

        while (!File.Exists(fileName)) // ファイルがなかったら
        {
            FileStream fs = File.Create(fileName); // ファイルを作成
            fs.Close(); // ファイルを閉じる

            if(fileName == latestFile)
            {
                // 仮データ保存
                saveLatestData(0);
            }

            if(fileName == rankingFile)
            {
                // 仮データ保存
                DataInitialize(length);
            }
        }
    }

    /// <summary>
    /// 最新スコア　セーブ
    /// </summary>
    public void saveLatestData(float score)
    {
        string fileName = Application.persistentDataPath + latestDataPath;

        StreamWriter writer;

        FileCheck(fileName, 0);

        string s = Mathf.Floor(score).ToString();

        writer = new StreamWriter(fileName, false); // 上書き
        writer.WriteLine(s);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// 最新スコア ロード
    /// </summary>
    /// <returns></returns>
    public float loadLatestData()
    {
        string fileName = Application.persistentDataPath + latestDataPath;
        string dataStr = "";
        float score = 0;

        FileCheck(fileName, 0);

        StreamReader reader = new StreamReader(fileName);
        dataStr = reader.ReadToEnd(); // 読み込み
        reader.Close();

        // stringがfloatか確認して代入
        if (float.TryParse(dataStr, out float s))
        {
            score = s;
        }

        return score;
    }

    /// <summary>
    /// ランキングデータ セーブ
    /// </summary>
    /// <param name="score">セーブするfloat型の配列</param>
    public void saveRankingData(float[] score)
    {
        StreamWriter writer;
        string fileName = Application.persistentDataPath + rankingDataPath;

        string[] s = new string[score.Length];

        FileCheck(fileName, 0);

        //スコア挿入
        for (int i = 0; i < score.Length; i++)
        {
            s[i] = Mathf.Floor(score[i]).ToString();
        }

        //文字列を","で区切る
        string s2 = string.Join(",", s);

        writer = new StreamWriter(fileName, false); // 上書き
        writer.WriteLine(s2);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// ランキングデータ ロード
    /// </summary>
    /// <param name="length">データ内の配列数</param>
    /// <returns></returns>
    public float[] loadRankingData(int length)
    {
        string directoryName = Application.persistentDataPath + "/Data";
        string fileName = Application.persistentDataPath + rankingDataPath;
        string dataStr = "";
        
        float[] score = new float[length];

        FileCheck(fileName, length);

        StreamReader reader = new StreamReader(fileName);
        dataStr = reader.ReadToEnd(); // 読み込み
        reader.Close();

        score = DataTrans(dataStr, length);
        
        return score;
    }

    /// <summary>
    /// テキストデータをfloatの配列に変換
    /// </summary>
    /// <param name="data">対象の文字列</param>
    /// <param name="length">データ内の配列数</param>
    /// <returns></returns>
    float[] DataTrans(string data, int length)
    {
        float[] score = new float[length];

        //コンマで区切る
        var line = data.Split(",");

        // データを配列に入れる
        for (int i = 0; i < length; i++)
        {
            // stringがfloatか確認して代入
            if(float.TryParse(line[i], out float s))
            {
                score[i] = s;
            }
        }

        return score;
    }

    /// <summary>
    /// データ全消去
    /// </summary>
    /// <param name="length">データ内の配列数</param>
    public void DataInitialize(int length)
    {
        float[] score = new float[length];

        // 0にする
        for(int i = 0; i < score.Length; i++)
        {
            score[i] = 0;
        }

        // 保存
        saveRankingData(score);
        saveLatestData(0);
    }
}
