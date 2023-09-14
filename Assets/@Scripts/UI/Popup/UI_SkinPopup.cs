using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using static Define;

public class UI_SkinPopup : UI_ContentPopup
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
        Content_1,
        Content_2,
        Content_3,
        Content_4,
        Content_5,
        Content_6,
    }

    enum Sliders
    {
        Slider
    }

    enum Images
    {
        ItemLoadingImage
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
    GameObject[] _grids = new GameObject[6];


    private bool _isDelay = false;
    private Image isLoadingImage;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        BindImage((typeof(Images)));
        Bind<TextMeshProUGUI>((typeof(TMPs)));
        BindButton(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        BindObject(typeof(Grids));

        _grids[0] = GetObject((int)Grids.Content_1);
        _grids[1] = GetObject((int)Grids.Content_2);
        _grids[2] = GetObject((int)Grids.Content_3);
        _grids[3] = GetObject((int)Grids.Content_4);
        _grids[4] = GetObject((int)Grids.Content_5);
        _grids[5] = GetObject((int)Grids.Content_6);


        B_Ball = GetButton((int)Buttons.Ball);
        B_Background = GetButton((int)Buttons.Back);
        B_Bat = GetButton((int)Buttons.Bat);
        B_Back = GetButton((int)Buttons.B_Back);

        Slider = Get<Slider>((int)Sliders.Slider);
        SliderTMP = Get<TextMeshProUGUI>((int)TMPs.SliderTMP);

        B_Ball.gameObject.BindEvent(() => { OnClickCategory(ScollViewType.Ball); });
        B_Background.gameObject.BindEvent(() => { OnClickCategory(ScollViewType.Skill); });
        B_Bat.gameObject.BindEvent(() => { OnClickCategory(ScollViewType.Bat); });

        isLoadingImage = GetImage((int)Images.ItemLoadingImage);
        isLoadingImage.gameObject.SetActive(false);




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
        if (_grids == null)
            return;

        if (_type == type)
            return;

        if (_isDelay == true)
            return;

        _isDelay = true;
        isLoadingImage.transform.rotation = Quaternion.identity;
        isLoadingImage.gameObject.SetActive(true);
        isLoadingImage.transform.DORotate(new Vector3(0,0,-360),0.5f).SetLoops(2);

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
                skinCount = Managers.Resource.ballOrderList.Count;
                hasItemCount = Managers.Game.GameDB.playerBalls.Count;
                break;
            case ScollViewType.Bat:
                skinCount = Managers.Resource.batOrderList.Count;
                hasItemCount = Managers.Game.GameDB.playerBats.Count;
                break;
            case ScollViewType.Skill:
                skinCount = Managers.Resource.skillOrderList.Count;
                hasItemCount = Managers.Game.GameDB.playerSkills.Count;
                break;
        }

        Slider.value = (hasItemCount / (float)skinCount);
        SliderTMP.text = $"{_type} {hasItemCount} / {skinCount}";
    }

    private void MakeItem()
    {
        List<string> makeList = new List<string>();

        switch (_type)
        {
            case ScollViewType.Ball:
                makeList = Managers.Resource.ballOrderList;
                break;
            case ScollViewType.Bat:
                makeList = Managers.Resource.batOrderList;
                break;
            case ScollViewType.Skill:
                makeList = Managers.Resource.skillOrderList;
                break;
        }

        if (true)
        {
            int count = 0;

            foreach (var itemID in makeList)
            {
                if (count == 14)
                    break;

                Debug.Log(count);

                var item = Managers.Resource.Instantiate(Keys.UI_KEY.UI_Skin_Item.ToString(), _grids[0].transform);

                if (item == null)
                {
                    Debug.Log("item is null");
                    continue;
                }

                UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
                skinItem.InitData(itemID, _type);

                count++;
            }

            if (makeList.Count > 14)
            {
                StartCoroutine(co_MakeItem(14, makeList));
            }
            else
            {

                _isDelay = false;
                isLoadingImage.transform.DOKill();
                isLoadingImage.gameObject.SetActive(false);
            }
        }
        else
        {
            makeList.Clear();
            makeList = Managers.Resource.batOrderList_1;

            foreach (var itemID in makeList)
            {


                var item = Managers.Resource.Instantiate(Keys.UI_KEY.UI_Skin_Item.ToString(), _grids[0].transform);

                if (item == null)
                {
                    Debug.Log("item is null");
                    continue;
                }

                UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
                skinItem.InitData(itemID, _type);

            }

            StartCoroutine(co_MakeItem());
        }




    }

    IEnumerator co_MakeItem(int index, List<string> list)
    {
        for (int i = index; i < list.Count; i++)
        {
           
            var item = Managers.Resource.Instantiate(Keys.UI_KEY.UI_Skin_Item.ToString(), _grids[0].transform);

            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }

            UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
            skinItem.InitData(list[i], _type);
            Debug.Log(i);
            yield return null;
        }

        _isDelay = false;
        isLoadingImage.transform.DOKill();
        isLoadingImage.gameObject.SetActive(false);
        

    }
    IEnumerator co_MakeItem()
    {
        List<string> makeList = new List<string>();
        makeList = Managers.Resource.batOrderList_2;

        foreach (var itemID in makeList)
        {

            var item = Managers.Resource.Instantiate(Keys.UI_KEY.UI_Skin_Item.ToString(), _grids[1].transform);

            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }

            UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
            skinItem.InitData(itemID, _type);
            yield return null;
        }

        makeList = Managers.Resource.batOrderList_3;
        makeList.Clear();

        foreach (var itemID in makeList)
        {

            var item = Managers.Resource.Instantiate(Keys.UI_KEY.UI_Skin_Item.ToString(), _grids[2].transform);

            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }

            UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
            skinItem.InitData(itemID, _type);
            yield return null;
        }


        makeList = Managers.Resource.batOrderList_4;
        makeList.Clear();

        foreach (var itemID in makeList)
        {

            var item = Managers.Resource.Instantiate(Keys.UI_KEY.UI_Skin_Item.ToString(), _grids[3].transform);

            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }

            UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
            skinItem.InitData(itemID, _type);
            yield return null;
        }


        makeList = Managers.Resource.batOrderList_5;
        makeList.Clear();

        foreach (var itemID in makeList)
        {

            var item = Managers.Resource.Instantiate(Keys.UI_KEY.UI_Skin_Item.ToString(), _grids[4].transform);

            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }

            UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
            skinItem.InitData(itemID, _type);
            yield return null;
        }

        makeList = Managers.Resource.batOrderList_6;
        makeList.Clear();

        foreach (var itemID in makeList)
        {

            var item = Managers.Resource.Instantiate(Keys.UI_KEY.UI_Skin_Item.ToString(), _grids[5].transform);

            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }

            UI_Skin_Item skinItem = item.GetOrAddComponent<UI_Skin_Item>();
            skinItem.InitData(itemID, _type);
            yield return null;
        }



        _isDelay = false;
        isLoadingImage.transform.DOKill();
        isLoadingImage.gameObject.SetActive(false);


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
        for (int i = 0; i < _grids.Length - 1; i++)
        {
            _grids[i].gameObject.SetActive(true);
            foreach (Transform child in _grids[i].transform)
                Managers.Resource.Destroy(child.gameObject);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Managers.Game.RemoveLobbyUIUpdate(UpdateSlider);
    }
}
