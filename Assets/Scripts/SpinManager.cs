using UnityEngine;
using EasyUI.PickerWheelUI;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System;

public class SpinManager : MonoBehaviour
{
    public static SpinManager Instance;

    [Header("SpinUI")]
    [SerializeField] private Button uiSpinButton;
    [SerializeField] private PickerWheel pickerWheel;
    [SerializeField] private RewardManager rewardManager;
    [SerializeField] private GameObject spinningCircle;
    [SerializeField] private GameObject indicator;
    [SerializeField] private TextMeshProUGUI SpinNameText;
    [SerializeField] private TextMeshProUGUI spinCounterText;
    [SerializeField] private int spinCounter = 1;

    [Header("RewardUI")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private Image rewardImage;
    [SerializeField] private TextMeshProUGUI rewardAmount;
    [SerializeField] private Button exitButton;

    [Header("DefeatUI")]
    [SerializeField] private Button giveUpButton;
    [SerializeField] private Button reviveButton;
    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private TextMeshProUGUI currencyText;

    [Header("EndGameUI")]
    [SerializeField] private GameObject collectRewardsScreen;
    [SerializeField] private GameObject endGameRewardsParent;
    [SerializeField] private Button restartGameButton;

    [Header("WheelObjects(Selected/Bronz/Silver/Gold)")]
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
        PrepareButtons();

        uiSpinButton.onClick.AddListener(() => {

            exitButton.interactable = false;
            uiSpinButton.interactable = false;

            pickerWheel.OnSpinEnd(wheelPiece => {
                Debug.Log(
                   @" <b>Index:</b> " + wheelPiece.Index + "           <b>Label:</b> " + wheelPiece.Label
                   + "\n <b>Amount:</b> " + wheelPiece.Amount + "      <b>Chance:</b> " + wheelPiece.Chance + "%"
                );

                CheckDeath(wheelPiece);
            });

            pickerWheel.Spin();
        });
    }

    private void RestartGame()
    {
        defeatScreen.SetActive(false);
        collectRewardsScreen.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ReviveTask()
    {
        int existingCurrency = int.Parse(currencyText.GetComponentInChildren<TextMeshProUGUI>().text);
        existingCurrency -= 25;
        currencyText.GetComponentInChildren<TextMeshProUGUI>().text = existingCurrency.ToString();

        PrepareUI();
        PrepareSpin();
        ChangeWheelObjectsInEverySpin();

        defeatScreen.SetActive(false);

        uiSpinButton.interactable = true;
    }

    private void GiveUpTask()
    {
        collectRewardsScreen.SetActive(true);

        foreach(Transform reward in rewardManager.rewardObjectsParent)
        {
            Instantiate(reward, endGameRewardsParent.transform);
        }
    }

    private void CheckDeath(WheelObject reward)
    {
        if (reward.Label == "Bomb")
        {
            defeatScreen.SetActive(true);
        }
        else
        {
            StartCoroutine(OnRewardPanelOpen(2f, reward));

            rewardManager.RewardCollect(reward);

            pickerWheel.EmptyWheel();
            StartCoroutine(OnSpinEndWaitTime(3f));
        }
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

    private void PrepareButtons()
    {
        exitButton.onClick.AddListener(ExitGame);
        giveUpButton.onClick.AddListener(GiveUpTask);
        reviveButton.onClick.AddListener(ReviveTask);
        restartGameButton.onClick.AddListener(RestartGame);
    }

    private void ExitGame()
    {
        collectRewardsScreen.SetActive(true);

        foreach (Transform reward in rewardManager.rewardObjectsParent)
        {
            Instantiate(reward, endGameRewardsParent.transform);
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
                selectedObj.Amount = Mathf.RoundToInt(selectedObj.Amount * 1.2f);
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
            selectedObj.Amount = Mathf.RoundToInt(selectedObj.Amount * 1.2f);
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