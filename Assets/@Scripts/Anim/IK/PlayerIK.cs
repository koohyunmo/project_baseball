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

    [Header(" IK Target ")]
    [SerializeField] private Transform ikTarget;

    // Start is called before the first frame update
    void Start()
    {
        ConfigureIK(null);
    }


    public void ConfigureIK(Transform iktarget)
    {

        if (iktarget == null)
            iktarget = ikTarget;
        // Enable the rig builderr
        rigidBuilder.enabled = true;

        foreach (var item in twoBoneIKConstraints)
        {
            item.data.target = iktarget;
        }
        foreach (var item in multiAimConstraints)
        {
            var weightTr = new WeightedTransformArray();
            weightTr.Add(new WeightedTransform(iktarget, 1));

            item.data.sourceObjects = weightTr;
        }

        rigidBuilder.Build();
    }

    public void DisableIK()
    {
        rigidBuilder.enabled = false;
    }
}
