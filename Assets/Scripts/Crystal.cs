using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

public enum CrystalColor
{
    Black = 0,
    Blue = 1,
    Green = 2,
    Grey = 3,
    LightBlue = 4,
    Orange = 5,
    Red = 6,
    Violet = 7,
    Yellow = 8,
}

public class Crystal : MonoBehaviour
{
    [SerializeField] private RectTransform prefabRectTransform;
    [SerializeField] private CrystalSprites spriteContainer;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image sparkleImage;

    [Space]
    [SerializeField] private CrystalColor crystalColor = 0;
    [SerializeField] private float crystalFadeTime = 0.2f; // Animation time in seconds
    [SerializeField] private float sparkleFadeTime = 0.1f; // Animation time in seconds

    public event Action<Crystal> clickEvent;

    public CrystalColor CrystalColor
    {
        get => crystalColor;
    }

    public Crystal Create(Transform parent)
    {
        Crystal crystal = Instantiate(this, parent);
        crystal.SetRandomColor();
        return crystal;
    }

    public void SetRandomColor()
    {
        int randomNumber = Random.Range(0, spriteContainer.sprites.Count);
        crystalColor = (CrystalColor)randomNumber;
        crystalImage.sprite = spriteContainer.sprites[randomNumber];
    }

    public void SetSize(float size)
    {
        prefabRectTransform.sizeDelta = new Vector2(size, size);
    }

    public void SetPosition(float x, float y)
    {
        transform.localPosition = new Vector2(x, y);
    }

    public void Clear(Action<Crystal> CallBack)
    {
        sparkleImage.color = new Color(sparkleImage.color.r, sparkleImage.color.g, sparkleImage.color.b, 0); // Make image transparent

        Sequence sequence = DOTween.Sequence();
        sequence.Append(crystalImage.DOFade(0, crystalFadeTime))
        .Append(sparkleImage.DOFade(1, sparkleFadeTime))
        .Append(sparkleImage.DOFade(0, sparkleFadeTime))
        .OnComplete(() => CallBack?.Invoke(this));
    }

    public void Refill(Action CallBack)
    {
        SetRandomColor();
        crystalImage.DOFade(1, crystalFadeTime)
        .OnComplete(() => CallBack?.Invoke());
    }

    /// <summary>Recursively looking for nearest neighbor with the same color.</summary>
    public void CheckNeighbor(List<Crystal> list, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);

        if (hit.collider?.GetComponent<Crystal>().CrystalColor == this.CrystalColor)
        {
            hit.collider.GetComponent<Crystal>().CheckNeighbor(list, direction);
            list.Add(this);
        }

        else
        {
            list.Add(this);
        }
    }

    public void OnClick()
    {
        SetBorderActive(true);
        clickEvent?.Invoke(this);
    }

    public void SetBorderActive(bool isActive)
    {
        borderImage.gameObject.SetActive(isActive);
    }
}
