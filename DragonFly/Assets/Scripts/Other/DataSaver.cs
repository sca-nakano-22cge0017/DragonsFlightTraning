using UnityEngine;
using System.IO;

public class DataSaver : MonoBehaviour
{
    string rankingDataPath = "/Data/RankingData.csv";
    string latestDataPath = "/Data/LatestData.csv";

    /// <summary>
    /// �t�@�C���E�f�B���N�g���̑��݊m�F
    /// </summary>
    /// <param name="fileName">�t�@�C����</param>
    /// <param name="length">�����L���O�f�[�^�̔z��</param>
    void FileCheck(string fileName, int length)
    {
        string directoryName = Application.persistentDataPath + "/Data";
        string latestFile = Application.persistentDataPath + latestDataPath;
        string rankingFile = Application.persistentDataPath + rankingDataPath;

        while (!Directory.Exists(directoryName)) //�f�B���N�g�����Ȃ�������
        {
            Directory.CreateDirectory(directoryName); //�f�B���N�g�����쐬
        }

        while (!File.Exists(fileName)) // �t�@�C�����Ȃ�������
        {
            FileStream fs = File.Create(fileName); // �t�@�C�����쐬
            fs.Close(); // �t�@�C�������

            if(fileName == latestFile)
            {
                // ���f�[�^�ۑ�
                saveLatestData(0);
            }

            if(fileName == rankingFile)
            {
                // ���f�[�^�ۑ�
                DataInitialize(length);
            }
        }
    }

    /// <summary>
    /// �ŐV�X�R�A�@�Z�[�u
    /// </summary>
    public void saveLatestData(float score)
    {
        string fileName = Application.persistentDataPath + latestDataPath;

        StreamWriter writer;

        FileCheck(fileName, 0);

        string s = Mathf.Floor(score).ToString();

        writer = new StreamWriter(fileName, false); // �㏑��
        writer.WriteLine(s);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// �ŐV�X�R�A ���[�h
    /// </summary>
    /// <returns></returns>
    public float loadLatestData()
    {
        string fileName = Application.persistentDataPath + latestDataPath;
        string dataStr = "";
        float score = 0;

        FileCheck(fileName, 0);

        StreamReader reader = new StreamReader(fileName);
        dataStr = reader.ReadToEnd(); // �ǂݍ���
        reader.Close();

        // string��float���m�F���đ��
        if (float.TryParse(dataStr, out float s))
        {
            score = s;
        }

        return score;
    }

    /// <summary>
    /// �����L���O�f�[�^ �Z�[�u
    /// </summary>
    /// <param name="score">�Z�[�u����float�^�̔z��</param>
    public void saveRankingData(float[] score)
    {
        StreamWriter writer;
        string fileName = Application.persistentDataPath + rankingDataPath;

        string[] s = new string[score.Length];

        FileCheck(fileName, 0);

        //�X�R�A�}��
        for (int i = 0; i < score.Length; i++)
        {
            s[i] = Mathf.Floor(score[i]).ToString();
        }

        //�������","�ŋ�؂�
        string s2 = string.Join(",", s);

        writer = new StreamWriter(fileName, false); // �㏑��
        writer.WriteLine(s2);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// �����L���O�f�[�^ ���[�h
    /// </summary>
    /// <param name="length">�f�[�^���̔z��</param>
    /// <returns></returns>
    public float[] loadRankingData(int length)
    {
        string directoryName = Application.persistentDataPath + "/Data";
        string fileName = Application.persistentDataPath + rankingDataPath;
        string dataStr = "";
        
        float[] score = new float[length];

        FileCheck(fileName, length);

        StreamReader reader = new StreamReader(fileName);
        dataStr = reader.ReadToEnd(); // �ǂݍ���
        reader.Close();

        score = DataTrans(dataStr, length);
        
        return score;
    }

    /// <summary>
    /// �e�L�X�g�f�[�^��float�̔z��ɕϊ�
    /// </summary>
    /// <param name="data">�Ώۂ̕�����</param>
    /// <param name="length">�f�[�^���̔z��</param>
    /// <returns></returns>
    float[] DataTrans(string data, int length)
    {
        float[] score = new float[length];

        //�R���}�ŋ�؂�
        var line = data.Split(",");

        // �f�[�^��z��ɓ����
        for (int i = 0; i < length; i++)
        {
            // string��float���m�F���đ��
            if(float.TryParse(line[i], out float s))
            {
                score[i] = s;
            }
        }

        return score;
    }

    /// <summary>
    /// �f�[�^�S����
    /// </summary>
    /// <param name="length">�f�[�^���̔z��</param>
    public void DataInitialize(int length)
    {
        float[] score = new float[length];

        // 0�ɂ���
        for(int i = 0; i < score.Length; i++)
        {
            score[i] = 0;
        }

        // �ۑ�
        saveRankingData(score);
        saveLatestData(0);
    }
}
