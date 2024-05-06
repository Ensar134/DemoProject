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
    [SerializeField] private Transform rewardObjectsParent;

    [SerializeField] private GameObject pileOfItems;
    [SerializeField] private Vector2[] initialPos;
    [SerializeField] private Quaternion[] initialRotation;
    [SerializeField] private int itemsAmount;

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

    public void RewardCollect(WheelObject reward)
    {       
        foreach(Transform child in pileOfItems.transform)
        {
            child.GetComponent<Image>().sprite = reward.Icon;
        }

        GameObject rewardPrefabGameObject = Instantiate(rewardPrefab, rewardObjectsParent);

        CountItems(rewardPrefabGameObject, reward);

        rewardPrefabGameObject.GetComponent<Image>().sprite = reward.Icon;
        rewardPrefabGameObject.GetComponentInChildren<TextMeshProUGUI>().text = reward.Amount.ToString();

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
        PlayerPrefs.SetInt("CountDollar", reward.Amount);
        rewardObject.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetInt("CountDollar").ToString();
    }
}
