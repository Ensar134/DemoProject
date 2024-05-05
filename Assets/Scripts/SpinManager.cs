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

    public int spinCounter = 0;

    public List<WheelObject> wheelObjects;

    public List<WheelObject> bronzeItems;
    public List<WheelObject> silverItems;
    public List<WheelObject> goldItems;

    private void Awake()
    {
        ChooseObjects();

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
        uiSpinButton.onClick.AddListener(() => {

            uiSpinButton.interactable = false;

            pickerWheel.OnSpinEnd(wheelPiece => {
                Debug.Log(
                   @" <b>Index:</b> " + wheelPiece.Index + "           <b>Label:</b> " + wheelPiece.Label
                   + "\n <b>Amount:</b> " + wheelPiece.Amount + "      <b>Chance:</b> " + wheelPiece.Chance + "%"
                );

                uiSpinButton.interactable = true;

                ChangeWheelObjectsAfterSpin();
                pickerWheel.EmptyWheel();

                StartCoroutine(OnSpinEndWaitTime(3f));
            });

            pickerWheel.Spin();
            spinCounter++;
        });
    }

    private void ChooseObjects()
    {
        wheelObjects.Clear(); 

        System.Random rand = new();

        for (int i = 0; i < 7; i++)
        {
            int randomIndex = rand.Next(0, bronzeItems.Count); // Rastgele bir indeks seç
            WheelObject secilenObj = bronzeItems[randomIndex]; // Seçilen objeyi al
            wheelObjects.Add(secilenObj); // Seçilen objeyi yeni listeye ekle
        }

        wheelObjects.Add(bronzeItems[0]); //Bomba eklendi.
    }

    public void ChangeWheelObjectsAfterSpin()
    {
        wheelObjects.Clear();

        System.Random rand = new();

        for (int i = 0; i < 7; i++)
        {
            int randomIndex = rand.Next(0, bronzeItems.Count); // Rastgele bir indeks seç
            WheelObject selectedObj = bronzeItems[randomIndex]; // Seçilen objeyi al
            //selectedObj.Amount *= 1.2f;
            wheelObjects.Add(selectedObj); // Seçilen objeyi yeni listeye ekle
        }

        wheelObjects.Add(bronzeItems[0]); //Bomba eklendi.
    }

    private IEnumerator OnSpinEndWaitTime(float time)
    {
        yield return new WaitForSeconds(time);

        for (int i = 0; i < wheelObjects.Count; i++)
        {
            pickerWheel.wheelPiecesParent.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = wheelObjects[i].Icon;
            pickerWheel.wheelPiecesParent.GetChild(i).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = wheelObjects[i].Amount.ToString();

            pickerWheel.wheelPiecesParent.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().gameObject.SetActive(true);
            pickerWheel.wheelPiecesParent.GetChild(i).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
            //pickerWheel.DrawNextPieces(wheelObjects[i]);
        }
    }
}