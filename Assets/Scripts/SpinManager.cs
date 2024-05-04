using UnityEngine;
using EasyUI.PickerWheelUI;
using UnityEngine.UI;
using TMPro;

public class SpinManager : MonoBehaviour
{
    [SerializeField] private Button uiSpinButton;
    [SerializeField] private PickerWheel pickerWheel;

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
        });
    }
}