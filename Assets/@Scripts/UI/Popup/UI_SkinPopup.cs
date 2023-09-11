using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using static Define;

public class UI_SkinPopup : UI_Popup
{

    enum TMPs
    {
        SliderTMP,
    }

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



    private Color defaultColor = new Color(0.98f, 0.64f, 0.42f, 0.5f);
    private Color pressedColor = new Color(0.98f, 0.64f, 0.42f, 1.0f); // #FCAA4B

    public Button B_Ball { get; private set; }
    public Button B_Bat { get; private set; }
    public Button B_Background { get; private set; }
    public Button B_Back { get; private set; }
    public Slider Slider { get; private set; }
    public TextMeshProUGUI SliderTMP { get; private set; }

    ScollViewType _type = ScollViewType.Bat;
    GameObject _grid;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        Bind<TextMeshProUGUI>((typeof(TMPs)));
        BindButton(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        BindObject(typeof(Grids));

        _grid = GetObject((int)Grids.Content);

        B_Ball = GetButton((int)Buttons.Ball);
        B_Background = GetButton((int)Buttons.Back);
        B_Bat = GetButton((int)Buttons.Bat);
        B_Back = GetButton((int)Buttons.B_Back);

        Slider = Get<Slider>((int)Sliders.Slider);
        SliderTMP = Get<TextMeshProUGUI>((int)TMPs.SliderTMP);

        B_Ball.gameObject.BindEvent(() => { OnClickCategory(ScollViewType.Ball); });
        B_Background.gameObject.BindEvent(() => { OnClickCategory(ScollViewType.Skill); });
        B_Bat.gameObject.BindEvent(() => { OnClickCategory(ScollViewType.Bat); });




        B_Back.gameObject.BindEvent(() => { Managers.UI.ClosePopupUI(this); });

        Managers.Game.SetLobbyUIUpdate(UpdateSlider);


        _type = ScollViewType.Skill;

        switch (_type)
        {
            case ScollViewType.Ball:
                FirstItemUpdate();
                break;
            case ScollViewType.Bat:
                FirstItemUpdate();
                break;
            case ScollViewType.Skill:
                FirstItemUpdate();
                break;
        }

        return true;
    }

    private void FirstItemUpdate()
    {
        Clear();
        switch (_type)
        {
            case ScollViewType.Ball:
                ChangeButtonColor(B_Ball);
                break;
            case ScollViewType.Bat:
                ChangeButtonColor(B_Bat);
                break;
            case ScollViewType.Skill:
                ChangeButtonColor(B_Background);
                break;

        }
        UpdateSlider();
        MakeItem();
    }

    private void OnClickCategory(ScollViewType type)
    {
        if (_grid == null)
            return;

        if (_type == type)
            return;

        _type = type;

        UpdateSlider();
        Clear();
        MakeItem();


        switch (_type)
        {
            case ScollViewType.Ball:
                ChangeButtonColor(B_Ball);
                break;
            case ScollViewType.Bat:
                ChangeButtonColor(B_Bat);
                break;
            case ScollViewType.Skill:
                ChangeButtonColor(B_Background);
                break;

        }
    }

    private void UpdateSlider()
    {
        int skinCount = -1;
        int hasItemCount = -1;

        switch (_type)
        {
            case ScollViewType.Ball:
                skinCount = Managers.Game.BallCount;
                hasItemCount = Managers.Game.GameDB.playerBalls.Count;
                break;
            case ScollViewType.Bat:
                skinCount = Managers.Game.BatCount;
                hasItemCount = Managers.Game.GameDB.playerBats.Count;
                break;
            case ScollViewType.Skill:
                skinCount = Managers.Game.SkillCount;
                hasItemCount = Managers.Game.GameDB.playerSkills.Count;
                break;
        }

        if (skinCount == 0)
        {
            Debug.LogWarning("Skin Count is Zero");
            return;
        }
            

        Slider.value = (hasItemCount / (float)skinCount);
        SliderTMP.text = $"{_type} {hasItemCount} / {skinCount}";
    }

    private void MakeItem()
    {
        string key = "";

        switch (_type)
        {
            case ScollViewType.Ball:
                key = "BALL_";
                break;
            case ScollViewType.Bat:
                key = "BAT_";
                break;
            case ScollViewType.Skill:
                key = "SKILL_";
                break;
                
            default:
                break;
        }


        foreach (var itemID in Managers.Resource.Resources.Keys)
        {
            // "BAT_" 문자열을 포함하지 않는 경우, 다음 반복으로 건너뜁니다.
            if (!itemID.Contains(key))
                continue;

            var item = Managers.Resource.Instantiate(Keys.UI_KEY.UI_Skin_Item.ToString(), _grid.transform);
            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }
            UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
            skinItem.InitData(itemID, _type);
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

    private void OnDestroy()
    {
        Managers.Game.RemoveLobbyUIUpdate(UpdateSlider);
    }
}
