using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UI_SkinPopup : UI_Popup
{

    enum Buttons
    {
        Ball,
        Bat,
        Back,
        B_Back
    }

    enum Grids
    {
        Content,
    }

    enum Sliders
    {
        Slider
    }

    enum ScollViewType
    {
        Ball,
        Bat,
        Background
    }

    private Color defaultColor = new Color(0.98f, 0.64f, 0.42f, 0.5f);
    private Color pressedColor = new Color(0.98f, 0.64f, 0.42f, 1.0f); // #FCAA4B

    public Button B_Ball { get; private set; }
    public Button B_Bat { get; private set; }
    public Button B_Background { get; private set; }
    public Button B_Back { get; private set; }
    public Slider Slider { get; private set; }

    ScollViewType _type = ScollViewType.Bat;
    GameObject _grid;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;



        BindButton(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        BindObject(typeof(Grids));

        _grid = GetObject((int)Grids.Content);

        B_Ball = GetButton((int)Buttons.Ball);
        B_Background = GetButton((int)Buttons.Back);
        B_Bat = GetButton((int)Buttons.Bat);
        B_Back = GetButton((int)Buttons.B_Back);

        Slider = Get<Slider>((int)Sliders.Slider);

        B_Ball.gameObject.BindEvent(() => { OnClickBallCategory(); });
        B_Background.gameObject.BindEvent(() => { OnClickBackgroundCategory(); });
        B_Bat.gameObject.BindEvent(() => { OnClickBatCategory(); });




        B_Back.gameObject.BindEvent(() => { Managers.UI.ClosePopupUI(this); });


        _type = ScollViewType.Bat;

        switch (_type)
        {
            case ScollViewType.Ball:
                OnClickBallCategory();
                break;
            case ScollViewType.Bat:
                OnClickBatCategory();
                break;
            case ScollViewType.Background:
                OnClickBackgroundCategory();
                break;

        }

        return true;
    }

    private void OnClickBallCategory()
    {
        if (_grid == null)
            return;

        Clear();
        ChangeButtonColor(B_Ball);

        _type = ScollViewType.Ball;
    }

    private void OnClickBackgroundCategory()
    {
        if (_grid == null)
            return;
        Clear();
        ChangeButtonColor(B_Background);
        _type = ScollViewType.Background;
    }

    private void OnClickBatCategory()
    {
        if (_grid == null)
            return;

        Clear();
        ChangeButtonColor(B_Bat);
        MakeItme();
        //StartCoroutine(co_MakeItem());


        _type = ScollViewType.Bat;
    }

    private void MakeItme()
    {
        foreach (var itemID in Managers.Resource.Bats.Keys)
        {
            // "BAT_" 문자열을 포함하지 않는 경우, 다음 반복으로 건너뜁니다.
            if (!itemID.Contains("BAT_"))
                continue;

            var item = Managers.Resource.Instantiate("UI_Skin_Item", _grid.transform);
            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }
            UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
            skinItem.InitData(itemID);
        }
    }

    IEnumerator co_MakeItem()
    {
        foreach (var itemID in Managers.Resource.Bats.Keys)
        {
            // "BAT_" 문자열을 포함하지 않는 경우, 다음 반복으로 건너뜁니다.
            if (!itemID.Contains("BAT_"))
                continue;

            var item = Managers.Resource.Instantiate("UI_Skin_Item", _grid.transform);
            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }
            UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
            skinItem.InitData(itemID);
            yield return null;
        }
    }


    void ChangeButtonColor(Button clickedButton)
    {
        // Reset all button colors
        B_Ball.GetComponent<Image>().color = defaultColor;
        B_Background.GetComponent<Image>().color = defaultColor;
        B_Bat.GetComponent<Image>().color = defaultColor;

        // Set clicked button color
        clickedButton.GetComponent<Image>().color = pressedColor;
    }


    private void Clear()
    {
        // Clear
        foreach (Transform child in _grid.transform)
            Managers.Resource.Destroy(child.gameObject);
    }
}
