using UnityEngine;
using EasyUI.PickerWheelUI;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;

public class SpinManager : MonoBehaviour
{
    public static SpinManager Instance;

    [SerializeField] private Button uiSpinButton;
    [SerializeField] private PickerWheel pickerWheel;
    [SerializeField] private RewardManager rewardManager;
    [SerializeField] private GameObject spinningCircle;
    [SerializeField] private GameObject indicator;
    [SerializeField] private TextMeshProUGUI SpinNameText;
    [SerializeField] private TextMeshProUGUI spinCounterText;

    public int spinCounter = 29;

    [Header("RewardUI")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private Image rewardImage;
    [SerializeField] private TextMeshProUGUI rewardAmount;
    [SerializeField] private Button exitButton;

    public List<WheelObject> wheelObjects;

    public List<WheelObject> bronzeItems;
    public List<WheelObject> silverItems;
    public List<WheelObject> goldItems;

    [Header("WheelUI")]
    [SerializeField] private Sprite bronzeWheelUI;
    [SerializeField] private Sprite silverWheelUI;
    [SerializeField] private Sprite goldWheelUI;

    [Header("IndicatorUI")]
    [SerializeField] private Sprite bronzeIndicatorUI;
    [SerializeField] private Sprite silverIndicatorUI;
    [SerializeField] private Sprite goldIndicatorUI;

    private bool startGame = false;

    private void Awake()
    {
        ChangeBronzWheelObjectsAfterSpin();

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        PrepareUI();

        uiSpinButton.onClick.AddListener(() => {

            exitButton.interactable = false;
            uiSpinButton.interactable = false;

            pickerWheel.OnSpinEnd(wheelPiece => {
                Debug.Log(
                   @" <b>Index:</b> " + wheelPiece.Index + "           <b>Label:</b> " + wheelPiece.Label
                   + "\n <b>Amount:</b> " + wheelPiece.Amount + "      <b>Chance:</b> " + wheelPiece.Chance + "%"
                );

                StartCoroutine(OnRewardPanelOpen(2f,wheelPiece));

                rewardManager.RewardCollect(wheelPiece);

                pickerWheel.EmptyWheel();
                StartCoroutine(OnSpinEndWaitTime(3f));
            });

            pickerWheel.Spin();
        });
    }

    private void PrepareUI()
    {
        spinCounterText.text = "Spin Counter: " + spinCounter.ToString();

        if (spinCounter % 30 == 0)
        {
            //Gold Spin
            spinningCircle.GetComponent<Image>().sprite = goldWheelUI;
            indicator.GetComponent<Image>().sprite = goldIndicatorUI;

            SpinNameText.text = "GOLDEN SPIN";
            SpinNameText.color = new Color(1.0f, 0.92f, 0.016f);
        }
        else if (spinCounter % 5 == 0)
        {
            //Silver Spin
            spinningCircle.GetComponent<Image>().sprite = silverWheelUI;
            indicator.GetComponent<Image>().sprite = silverIndicatorUI;

            SpinNameText.text = "SILVER SPIN";
            SpinNameText.color = new Color(1.0f, 1.0f, 1.0f);
        }
        else
        {
            //Bronze Spin
            spinningCircle.GetComponent<Image>().sprite = bronzeWheelUI;
            indicator.GetComponent<Image>().sprite = bronzeIndicatorUI;

            SpinNameText.text = "BRONZE SPIN";
            SpinNameText.color = new Color(1.0f, 0.64f, 0.0f);
        }
    }

    private void PrepareSpin()
    {
        if (spinCounter % 30 == 0)
        {
            ChangeGoldenWheelObjectsAfterSpin();
        }
        else if (spinCounter % 5 == 0)
        {
            ChangeSilverWheelObjectsAfterSpin();
        }
        else
        {
            ChangeBronzWheelObjectsAfterSpin();
        }             
    }

    private void ChangeBronzWheelObjectsAfterSpin()
    {
        wheelObjects.Clear();

        System.Random rand = new();

        for (int i = 0; i < 7; i++)
        {
            int randomIndex = rand.Next(0, bronzeItems.Count); 
            WheelObject selectedObj = bronzeItems[randomIndex];

            if (startGame == true)
            {
                selectedObj.Amount = Mathf.RoundToInt(selectedObj.Amount * 1.4f);
            }
            wheelObjects.Add(selectedObj); 
        }

        startGame = true;
        wheelObjects.Add(bronzeItems[0]); //Bomba eklendi.
    }

    private void ChangeSilverWheelObjectsAfterSpin()
    {
        wheelObjects.Clear();

        System.Random rand = new();

        for (int i = 0; i < 8; i++)
        {
            int randomIndex = rand.Next(0, silverItems.Count); 
            WheelObject selectedObj = silverItems[randomIndex];
            selectedObj.Amount = Mathf.RoundToInt(selectedObj.Amount * 1.4f);
            wheelObjects.Add(selectedObj); 
        }
    }

    private void ChangeGoldenWheelObjectsAfterSpin()
    {
        wheelObjects.Clear();

        System.Random rand = new();

        for (int i = 0; i < 8; i++)
        {
            int randomIndex = rand.Next(0, goldItems.Count); 
            WheelObject selectedObj = goldItems[randomIndex]; 
            wheelObjects.Add(selectedObj); 
        }
    }

    private void ChangeWheelObjectsInEverySpin()
    {
        for (int i = 0; i < wheelObjects.Count; i++)
        {
            pickerWheel.wheelPiecesParent.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = wheelObjects[i].Icon;
            pickerWheel.wheelPiecesParent.GetChild(i).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = wheelObjects[i].Amount.ToString();

            pickerWheel.wheelPiecesParent.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
            pickerWheel.wheelPiecesParent.GetChild(i).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
        }
    }

    private IEnumerator OnSpinEndWaitTime(float time)
    {
        yield return new WaitForSeconds(time);

        spinCounter++;
        spinCounterText.text = "Spin Counter: " + spinCounter.ToString();

        PrepareSpin();
        PrepareUI();
        ChangeWheelObjectsInEverySpin();        

        uiSpinButton.interactable = true;
    }

    private IEnumerator OnRewardPanelOpen(float time, WheelObject reward)
    {
        rewardImage.sprite = reward.Icon;
        rewardAmount.text = reward.Amount.ToString();
        rewardPanel.SetActive(true);

        yield return new WaitForSeconds(time);

        rewardPanel.SetActive(false);
        exitButton.interactable = true;
    }
}