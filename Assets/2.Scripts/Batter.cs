using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Batter : MonoBehaviour
{
    [SerializeField] Bat leftBat;
    [SerializeField] Bat rightBat;
    [SerializeField] Define.BatPosition batPosition;


    private void Start()
    {
        BatInit();
    }

    private void BatInit()
    {
        //batPosition = Managers.Game.BatPosition;

        BatChange();

        Managers.Game.SetBatPositionSetting(SetBatPosition);
        Managers.Game.SetBatPosition(Define.BatPosition.Left);

        SetBatPosition();
        AdjustBatPositionBasedOnPadding();
    }

    public void SetBatPosition()
    {
        if (Managers.Game.BatPosition == Define.BatPosition.Right)
        {
            leftBat.gameObject.SetActive(false);
            rightBat.gameObject.SetActive(true);
        }
        else
        {
            leftBat.gameObject.SetActive(true);
            rightBat.gameObject.SetActive(false);
        }
    }

    private void BatChange()
    {
        string batId = Managers.Game.GameDB.playerInfo.equipBatId;
        if (Managers.Resource.Bats[batId] is ItemScriptableObject so)
        {
            var mat = so.model.GetComponent<MeshRenderer>().sharedMaterial;
            var mesh = so.model.GetComponent<MeshFilter>().sharedMesh;

            // 매테리얼
            leftBat.ChangeBatMat(mat);
            rightBat.ChangeBatMat(mat);
            // 매쉬
            leftBat.ChangeBatMesh(mesh);
            rightBat.ChangeBatMesh(mesh);
        }

    }

    private void AdjustBatPositionBasedOnPadding()
    {
        float paddingPercentage = 0.9f;

        // TODO
        {
            // 왼쪽 패딩만큼 x 좌표 조정
            float targetX = Camera.main.ViewportToWorldPoint(new Vector3(paddingPercentage, 1f, 3)).x;
            leftBat.transform.position = new Vector3(targetX, leftBat.transform.position.y, leftBat.transform.position.z);
        }

        {
            // 오른쪽 패딩만큼 x 좌표 조정
            float targetX = Camera.main.ViewportToWorldPoint(new Vector3(1f - paddingPercentage, 1f, 3)).x;
            rightBat.transform.position = new Vector3(targetX, rightBat.transform.position.y, rightBat.transform.position.z);
        }
    }

}
