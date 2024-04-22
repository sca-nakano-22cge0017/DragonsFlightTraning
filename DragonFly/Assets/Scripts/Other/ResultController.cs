using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField] DataSaver dataSaver;
    [SerializeField] SceneChange sceneChange;

    [SerializeField] Text distance;
    float dis = 0;
    float d = 0;
    [SerializeField] Text newScoreText;

    bool canMove = false;

    [SerializeField] Text[] rankingScore;
    float[] scores = new float[4];
    bool isUpdated = false;

    private void Awake()
    {
        // �������̃X�R�A���擾
        dis = dataSaver.loadLatestData();

        newScoreText.enabled = false;

        sceneChange.FadeIn();
        StartCoroutine(FadeEndCheck());

        // �����L���O�擾
        scores = dataSaver.loadRankingData(scores.Length);

        for (int i = 0; i < rankingScore.Length; i++)
        {
            rankingScore[i].enabled = false;
        }
    }

    /// <summary>
    /// �t�F�[�h�C���������������`�F�b�N����
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeEndCheck()
    {
        yield return new WaitUntil(() => sceneChange.IsFadeInEnd);
        canMove = true;
    }

    void Update()
    {
        if(canMove)
        {
            if (d < dis)
            {
                d++;

                //�X�y�[�X/�G���^�[����������X�R�A�̃J�E���g�A�b�v���X�L�b�v����
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    d = dis;
                }

                distance.text = Mathf.Floor(d).ToString() + "m";
            }
            else if (d >= dis)
            {
                d = dis;

                //�P�ʃX�R�A��荂��������
                if (scores[0] < dis)
                {
                    //�V�L�^�̕\��
                    newScoreText.enabled = true;
                }

                if(!isUpdated) //�����L���O�X�V�E�\��
                {
                    isUpdated = true;
                    scores[scores.Length - 1] = dis;

                    //�~���\�[�g
                    System.Array.Sort(scores);
                    System.Array.Reverse(scores);

                    //�����L���O�ۑ�
                    dataSaver.saveRankingData(scores);

                    //�����L���O�\��
                    for (int i = 0; i < rankingScore.Length; i++)
                    {
                        rankingScore[i].enabled = true;
                        rankingScore[i].text = Mathf.Floor(scores[i]).ToString() + "m";
                    }
                }
            }
        }

        //�f�[�^�폜�R�}���h
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            // �f�[�^������
            dataSaver.DataInitialize(scores.Length);
        }
    }
}