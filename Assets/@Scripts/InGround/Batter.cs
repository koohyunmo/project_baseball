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

        Managers.Game.SetBatPosition(batPosition);

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
        if (Managers.Resource.Resources[batId] is ItemScriptableObject so)
        {
            List<Material> mats = new List<Material>();

            var modelMats = so.model.GetComponent<MeshRenderer>().sharedMaterials;
            mats.AddRange(modelMats);

            var mesh = so.model.GetComponent<MeshFilter>().sharedMesh;

            // ���׸���
            leftBat.ChangeBatMat(mats);
            rightBat.ChangeBatMat(mats);
            // �Ž�
            leftBat.ChangeBatMesh(mesh);
            rightBat.ChangeBatMesh(mesh);
            // �ݶ��̴� �Ž�
        }

    }

    private void AdjustBatPositionBasedOnPadding()
    {
        float paddingPercentage = 0.9f;

        // TODO
        {
            // ���� �е���ŭ x ��ǥ ����
            float targetX = Camera.main.ViewportToWorldPoint(new Vector3(paddingPercentage, 1f, 3)).x;
            leftBat.transform.position = new Vector3(targetX, leftBat.transform.position.y, leftBat.transform.position.z);
        }

        {
            // ������ �е���ŭ x ��ǥ ����
            float targetX = Camera.main.ViewportToWorldPoint(new Vector3(1f - paddingPercentage, 1f, 3)).x;
            rightBat.transform.position = new Vector3(targetX, rightBat.transform.position.y, rightBat.transform.position.z);
        }
    }

}
