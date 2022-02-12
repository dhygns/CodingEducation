using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    public int level;
    public DataCarrier.StudentInput allowedInput;
    public string map;

    public void OnLevelSelected()
    {
        DataCarrier.RequestSetupInfoToAllStudent(level, allowedInput);
        TeacherManager.SetupInfo(level, allowedInput, map);
    }

    public void OnValidate()
    {
        _name.text = $"{level}";
    }
}
