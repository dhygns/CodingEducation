using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DataCarrier;

public class SubmittedStudentDataPreview : MonoBehaviour
{
    [SerializeField] private TMP_Text _previewText;
    private StudentInput _inputType;
    public StudentInput inputType => _inputType;

    public void Setup(StudentInput inputType)
    {
        _previewText.text = inputType.ToString();
        _inputType = inputType;
    }
}
