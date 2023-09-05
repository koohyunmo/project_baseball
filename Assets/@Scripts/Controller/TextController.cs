using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TextController : InGameObjectController
{

    TextMeshPro tmp;


    public void Init()
    {
        if(tmp == null)
            tmp = GetComponent<TextMeshPro>();

        ResetText();
        TextAnim();
    }

    private void TextAnim()
    {
        tmp.text = "+" + Managers.Game.HitScore.ToString();

        gameObject.transform.DOMoveY(1.5f, 1f).SetEase(Ease.InOutQuad).OnComplete(() => {
            Managers.Object.Despawn<TextController>(ObjId); 
        });
        tmp.DOFade(0, 1f).SetEase(Ease.InOutQuad);
    }

    private void ResetText()
    {
        tmp.color = new Color(0, 0, 0, 1);
    }

    private void OnEnable()
    {
        if (tmp == null)
            Init();
        else
        {
            ResetText();
            TextAnim();
        }

        
    }

    public override void Clear()
    {
        base.Clear();
        tmp.DOKill();
        gameObject.transform.DOKill();
        
    }

}
