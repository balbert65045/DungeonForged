using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum Rarity{
    Common = 0,
    Uncommon = 1,
    Rare = 2
}

public class NewCard : Card, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action[] backActions()
    {
        Action action = new Action(ActionType.Movement, new AOE(), 1, DeBuff.None);
        Action[] actions = new Action[] { action };
        return actions;
    }

    public bool Exaustion = false;
    public Rarity CardRarity;
    public int price;
    public Text EnergyText;
    public GameObject PrefabAssociatedWith;
    public int EnergyAmount;
    public int CurrentEnergyAmount() { return FrontFacing ? EnergyAmount : 0; }

    public CardAbility cardAbility;
    bool Showing = false;
    bool Dragging = false;
    Transform CurrentParent;
    public void SetCurrentParent(Transform t) { CurrentParent = t; }
    Vector3 originalScale;
    Vector3 originalPosition;

    public bool Returning = false;
    public bool Used = false;

    bool playable = true;
    public GameObject UnPlayablePanel;

    public bool FrontFacing = true;
    public GameObject FrontSide;
    public GameObject BackSide;
    public bool flipping = false;

    public void Flip()
    {
        StartCoroutine("FlipCard");
    }

    public void FlipBack()
    {
        FrontSide.SetActive(false);
        BackSide.SetActive(true);
        BackSide.transform.localRotation = Quaternion.identity;
    }

    IEnumerator FlipCard()
    {
        flipping = true;
        bool faceflip = false;
        if (FrontFacing)
        {
            FrontFacing = !FrontFacing;
            while (transform.localRotation.eulerAngles.y <= 180 && flipping)
            {
                transform.Rotate(Vector3.up*5);
                if (transform.localRotation.eulerAngles.y > 90 && faceflip == false)
                {
                    FrontSide.SetActive(false);
                    BackSide.SetActive(true);
                    BackSide.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    faceflip = true;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            FrontFacing = !FrontFacing;
            while (transform.localRotation.eulerAngles.y > 180 && flipping)
            {
                transform.Rotate(Vector3.up*5);
                if (transform.localRotation.eulerAngles.y > 270 && faceflip == false)
                {
                    FrontSide.SetActive(true);
                    BackSide.SetActive(false);
                    faceflip = true;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        flipping = false;
    }


    public void SetUnPlayable() {
        playable = false;
    }
    public void SetPlayable()
    {
        playable = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inHand() && playable && FindObjectOfType<PlayerController>().CardsPlayable)
        {
            if (Exaustion) { return; }
            Dragging = true;
            StartCoroutine("ShowFlipArea");
        }
        else if (inLoot())
        {
            GetComponentInParent<CardLoot>().AddCardToStorage(this);
        }
    }

    IEnumerator ShowFlipArea()
    {
        yield return new WaitForSeconds(.1f);
        if (Dragging) { FindObjectOfType<FlipCardArea>().ShowArea(); }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Dragging = false;
        if (inStaging() && FindObjectOfType<PlayerController>().CardsPlayable)
        {
            FindObjectOfType<StagingArea>().ReturnLastCardToHand();
            return;
        }
        else if (inHand() && playable && FindObjectOfType<PlayerController>().CardsPlayable)
        {
            if (Exaustion) { return; }
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            if (OverFlipArea(raysastResults)) { FrontFacing = false; }
            if (CurrentEnergyAmount() > FindObjectOfType<EnergyAmount>().CurrentEnergyAmount) { unShowCard(); }
            else if (cardAbility.Staging) {
                InTheHand = false;
                FindObjectOfType<NewHand>().PutCardInStaging(this); 
            }
            else{
                InTheHand = false;
                FindObjectOfType<NewHand>().UseCard(this); 
            }
        }
        else
        {
            unShowCard();
        }
        FindObjectOfType<FlipCardArea>().HideArea();
        FindObjectOfType<HexVisualizer>().HexChange();
    }

    bool inLoot() { return GetComponentInParent<CardLoot>() != null; }

    public bool InTheHand = false;
    bool inHand(){ return InTheHand; }
    bool inStaging() { return GetComponentInParent<StagingArea>() != null; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inHand() && playable && FindObjectOfType<PlayerController>().CardsPlayable && !Returning)
        {
            Showing = true;
            showCard();
        }
    }

    public virtual void PointerExited()
    {
        Showing = false;
        unShowCard();
    }

    public void showCard()
    {
        if (Dragging) { return; }
        transform.localScale = new Vector3( 1.3f, 1.3f);
        transform.SetParent(transform.parent.parent);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 2f); 
    }

    public void ShowFront()
    {
        if (Exaustion) { return; }
        FrontFacing = true;
        FrontSide.SetActive(true);
        BackSide.SetActive(false);
    }
    public void unShowCard()
    {
        if (Dragging) { return; }
        if (inStaging()) { return; }
        flipping = false;
        if (GetComponentInParent<CardsPanel>() != null) { return; }
        transform.SetParent(CurrentParent);
        ShowFront();
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(1,1,1);
        transform.localPosition = Vector3.zero;
    }

    bool OverFlipArea(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<FlipCardArea>() != null)
            {
                return true;
            }
        }
        return false;
    }

    bool OverThisCard(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<NewCard>() != null && result.gameObject.GetComponent<NewCard>() == this)
            {
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        SetCurrentParent(transform.parent);
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    void Update () {
        if (Used)
        {
            if (transform.localPosition.magnitude > 80f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, .1f);
                transform.localScale = Vector3.Lerp(originalScale / 4, transform.localScale, .1f);
            }
            else
            {
               gameObject.SetActive(false);
               transform.localScale = originalScale;
                Used = false;
            }
        }
        else if (Returning)
        {
            if (transform.localPosition.magnitude > .1f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, .1f);
            }
            else
            {
                Returning = false;
            }
        }
        else if (Showing)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            if (!OverThisCard(raysastResults)) { PointerExited(); }
        }

        if (Dragging)
        {
            transform.position = Input.mousePosition;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!flipping) { Flip(); }
            }
        }
    }
}
