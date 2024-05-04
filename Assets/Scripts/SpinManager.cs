using UnityEngine;
using EasyUI.PickerWheelUI;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

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
            });

            pickerWheel.Spin();
            spinCounter++;
        });
    }

    void ChooseObjects()
    {
        wheelObjects.Clear(); 

        System.Random rand = new();

        for (int i = 0; i < 8; i++)
        {
            int randomIndex = rand.Next(0, bronzeItems.Count); // Rastgele bir indeks seç
            WheelObject secilenObj = bronzeItems[randomIndex]; // Seçilen objeyi al
            wheelObjects.Add(secilenObj); // Seçilen objeyi yeni listeye ekle
        }
    }
}