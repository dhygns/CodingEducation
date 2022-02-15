using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DataCarrier;

public class StudentInputPreviewButton : MonoBehaviour
{
    [SerializeField] TMP_Text _buttonText;
    private StudentInput _inputType;
    
    public void Setup(StudentInput inputType)
    {
        _inputType = inputType;
        _buttonText.text = inputType.ToString();
    }

    public void OnClicked()
    {
        StudentManager.DeregisterInput(gameObject);
    }
}
