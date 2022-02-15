using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static DataCarrier;

public class SubmittedStudentDataView : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] private SubmittedStudentDataPreview _previewPrefab;

    [Header("Profile")]
    [SerializeField] private TMP_Text _studentName; // TODO : replace student number profile picture
    [SerializeField] private Button _playButton;
    [Header("List")]
    [SerializeField] private Transform _previewLayout;

    private IEnumerable<StudentInput> _inputs;

    public void Setup(int actorNumber, IEnumerable<StudentInput> inputs)
    {
        _studentName.text = TeacherManager.GetStudentName(actorNumber);
        _inputs = inputs;

        foreach(StudentInput input in inputs)
        {
            Instantiate(_previewPrefab, _previewLayout).Setup(input);
        }
    }

    public void OnPlayButtonClicked()
    {
        TeacherManager.PlayInputs(this);
    }
}
