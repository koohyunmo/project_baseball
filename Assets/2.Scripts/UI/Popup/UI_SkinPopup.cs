using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkinPopup : UI_Popup
{

    enum Buttons
    {
        B_Back,
        B_Ball,
        B_Bat,
        B_BackGround
    }

    enum Sliders
    {
        Slider
    }

    enum ScrollViews
    {
        Ball_Scroll_View,
        Bat_Scroll_View,
        Map_Scroll_View,
    }

    public Button B_Ball { get; private set; }
    public Button B_Bat { get; private set; }
    public Button B_Background { get; private set; }
    public Button B_Back { get; private set; } 
    public Slider Slider { get; private set; } 
    public GameObject Ball_Scroll_View { get; private set; }
    public GameObject Bat_Scroll_View { get; private set; }
    public GameObject Map_Scroll_View { get; private set; }


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        BindObject(typeof(ScrollViews));

        B_Back = GetButton((int)Buttons.B_Back);
        B_Ball = GetButton((int)Buttons.B_Ball);
        B_Background = GetButton((int)Buttons.B_BackGround);
        B_Bat = GetButton((int)Buttons.B_Bat);

        Slider = Get<Slider>((int)Sliders.Slider);

        Ball_Scroll_View = GetObject((int)ScrollViews.Ball_Scroll_View);
        Bat_Scroll_View = GetObject((int)ScrollViews.Bat_Scroll_View);
        Map_Scroll_View = GetObject((int)ScrollViews.Map_Scroll_View);  

        // TODO 스크롤뷰 별 컨트롤러

        gameObject.AddComponent<UI_SkinPopupLogic>();

        return true;    
    }
}
