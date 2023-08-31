using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerIK : MonoBehaviour
{

    [Header(" Elements ")]
    [SerializeField] private RigBuilder rigidBuilder;

    [Header(" Constraints ")]
    [SerializeField] private TwoBoneIKConstraint[] twoBoneIKConstraints;
    [SerializeField] private MultiAimConstraint[] multiAimConstraints;

    // Start is called before the first frame update
    void Start()
    {
        ConfigureIK(null);
    }


    public void ConfigureIK(Transform iKtarget)
    {

        if (iKtarget == null)
            iKtarget = GameObject.Find("IK_Target").transform;
        // Enable the rig builderr
        rigidBuilder.enabled = true;

        foreach (var item in twoBoneIKConstraints)
        {
            item.data.target = iKtarget;
        }
        foreach (var item in multiAimConstraints)
        {
            var weightTr = new WeightedTransformArray();
            weightTr.Add(new WeightedTransform(iKtarget, 1));

            item.data.sourceObjects = weightTr;
        }

        rigidBuilder.Build();
    }

    public void DisableIK()
    {
        rigidBuilder.enabled = false;
    }
}
