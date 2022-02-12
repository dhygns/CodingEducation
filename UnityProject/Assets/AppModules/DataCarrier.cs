using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DataCarrier : MonoBehaviourPunCallbacks
{
    static public DataCarrier instance => _instance;
    static private DataCarrier _instance;

    [Flags]
    public enum StudentInput { Up = 1 << 0, Down = 1 << 1, Left = 1 << 2, Right = 1 << 3 }

    public class StudentData
    {
        public int actorNumber;
        public List<StudentInput> inputs;
    }    

    public void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    static public void RequestSetupInfoToAllStudent(int level = -1, StudentInput allowedInput = (StudentInput)0)
    {
        _instance.photonView.RPC(nameof(RPC_SetupInfo), RpcTarget.Others, level, allowedInput as object);
    }

    static public void RequestSetupInfoToStudent(Player player, int level, StudentInput allowedInput)
    {
        _instance.photonView.RPC(nameof(RPC_SetupInfo), player, level, allowedInput as object);
    }

    static public void RequestSendInputsToTeacher(params StudentInput[] inputs)
    {
        object[] datas = inputs.Select(item => (object)item).ToArray();
        _instance.photonView.RPC(nameof(RPC_GetInputs), PhotonNetwork.MasterClient, datas as object);
    }

    static public void RequestSetupStudentInfo(string studentName)
    {
        _instance.photonView.RPC(nameof(RPC_SetupStudentInfo), PhotonNetwork.MasterClient, studentName);
    }

    #region RPCs

    [PunRPC]
    public void RPC_SetupInfo(int level, object allowedInputs)
    {
        StudentManager.SetupInfo(level, (StudentInput)allowedInputs);
    }

    [PunRPC]
    public void RPC_GetInputs(object[] inputs, PhotonMessageInfo info)
    {
        TeacherManager.EnqueueStudentRequest(info.Sender.ActorNumber, inputs.Select(item => (StudentInput)item));
    }

    [PunRPC]
    public void RPC_SetupStudentInfo(string studentName, PhotonMessageInfo info)
    {
        TeacherManager.AddStudent(info.Sender.ActorNumber, studentName);
    }

    #endregion
}
