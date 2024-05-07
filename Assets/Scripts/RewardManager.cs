using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    [Header("RewardItemPanel")]
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private GameObject pileOfItems;
    [SerializeField] private Vector2[] initialPos;
    [SerializeField] private Quaternion[] initialRotation;
    [SerializeField] private int itemsAmount;
    [SerializeField] private GameObject rewardPrefabGameObject;
    [SerializeField] private GameObject existingRewardPrefabGameObject;

    public List<string> existingRewards =new();
    public Transform rewardObjectsParent;

    void Start()
    {
        if (itemsAmount == 0)
            itemsAmount = 5; 

        initialPos = new Vector2[itemsAmount];
        initialRotation = new Quaternion[itemsAmount];

        for (int i = 0; i < pileOfItems.transform.childCount; i++)
        {
            initialPos[i] = pileOfItems.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
            initialRotation[i] = pileOfItems.transform.GetChild(i).GetComponent<RectTransform>().rotation;
        }
    }

    private void CheckCollectedReward(WheelObject reward)
    {
        if (existingRewards.Count >= 1)
        {
            if (existingRewards.IndexOf(reward.Label) != -1)
            {
                for (int i = 0; i < existingRewards.Count; i++)
                {
                    if (existingRewards[i] == reward.Label)
                    {
                        Debug.Log($"Liste {reward.Label} öðesini içeriyor ve index: {i}"); 

                        existingRewardPrefabGameObject = rewardObjectsParent.GetChild(i).gameObject;
                    }
                }

                CountItems(existingRewardPrefabGameObject, reward);

                string existingText = existingRewardPrefabGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
                int existingAmount = int.Parse(existingText);
                int rewAmount = reward.Amount;
                existingAmount += rewAmount;
                existingRewardPrefabGameObject.GetComponentInChildren<TextMeshProUGUI>().text = existingAmount.ToString();
            }
            else
            {
                rewardPrefabGameObject = Instantiate(rewardPrefab, rewardObjectsParent);

                CountItems(rewardPrefabGameObject, reward);

                rewardPrefabGameObject.GetComponent<Image>().sprite = reward.Icon;
                rewardPrefabGameObject.GetComponentInChildren<TextMeshProUGUI>().text = reward.Amount.ToString();
            }
        }
        else
        {
            rewardPrefabGameObject = Instantiate(rewardPrefab, rewardObjectsParent);

            CountItems(rewardPrefabGameObject, reward);

            rewardPrefabGameObject.GetComponent<Image>().sprite = reward.Icon;
            rewardPrefabGameObject.GetComponentInChildren<TextMeshProUGUI>().text = reward.Amount.ToString();
        }

        if (!existingRewards.Contains(reward.Label))
        {
            existingRewards.Add(reward.Label);
        }
    }

    public void RewardCollect(WheelObject reward)
    {
        foreach (Transform child in pileOfItems.transform)
        {
            child.GetComponent<Image>().sprite = reward.Icon;
        }

        CheckCollectedReward(reward);

        for (int i = 0; i < pileOfItems.transform.childCount; i++)
        {
            pileOfItems.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = initialPos[i];
            pileOfItems.transform.GetChild(i).GetComponent<RectTransform>().rotation = initialRotation[i];
        }
    }

    public void CountItems(GameObject rewardObject, WheelObject reward)
    {
        pileOfItems.SetActive(true);
        var delay = 0f;

        for (int i = 0; i < pileOfItems.transform.childCount; i++)
        {
            pileOfItems.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            pileOfItems.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-750f, 100f), 0.8f)
                .SetDelay(delay + 0.5f).SetEase(Ease.InBack);

            pileOfItems.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f)
                .SetEase(Ease.Flash);

            pileOfItems.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);

            delay += 0.1f;

            rewardObject.transform.GetChild(0).transform.DOScale(1.1f, 0.1f).SetLoops(10, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(1.2f);
        }

        StartCoroutine(SaveItems(rewardObject, reward));
    }

    IEnumerator SaveItems(GameObject rewardObject, WheelObject reward)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        //rewardObject.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt(reward.Label).ToString();
        //PlayerPrefs.SetInt(rewardObject.GetComponentInChildren<TextMeshProUGUI>().text, reward.Amount);
    }
}
