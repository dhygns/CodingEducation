using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StudentPanel : MonoBehaviour
{
    public string studentName => _studentName.text;
    [SerializeField] private TMP_Text _studentName;
    
    public void Setup(string studentName)
    {
        _studentName.text = studentName;
    }
    
}
