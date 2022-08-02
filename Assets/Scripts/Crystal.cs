using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

public class Crystal : MonoBehaviour
{
    [SerializeField] private RectTransform crystalRectTransform;
    [SerializeField] private CrystalSprites crystalSprites;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image sparkleImage;
    private float animationTime = 0.1f;
    public int CrystalType = 0;
    public event Action CrystalClickEvent;

    public Crystal Create(Transform parent)
    {
        sparkleImage.gameObject.SetActive(false);
        borderImage.gameObject.SetActive(false);

        var crystal = Instantiate(this, parent);
        crystal.SetRandomType();
        return crystal;
    }

    public void SetSize(float size)
    {
        crystalRectTransform.sizeDelta = new Vector2(size, size);
    }

    public void SetRandomType()
    {
        CrystalType = Random.Range(0, crystalSprites.Sprites.Count);
        crystalImage.sprite = crystalSprites.Sprites[CrystalType];
    }

    public void CheckNeighbor(List<Crystal> list, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);

        if (hit.collider?.GetComponent<Crystal>().CrystalType == this.CrystalType)
        {
            hit.collider.GetComponent<Crystal>().CheckNeighbor(list, direction);
            list.Add(this);
        }
        else list.Add(this);
    }

    private void Disappear()
    {
        borderImage.gameObject.SetActive(false);
        var disappearingCrystal = crystalImage.DOFade(0, animationTime);
        disappearingCrystal.OnComplete(() => { this.Sparkle(); });
    }

    private void Sparkle()
    {
        sparkleImage.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        var appearingSparkle = sparkleImage.DOFade(1, animationTime);
        var disappearingSparkle = sparkleImage.DOFade(0, animationTime);

        sequence.Append(appearingSparkle).Append(disappearingSparkle);
        sequence.OnComplete(() => { sparkleImage.gameObject.SetActive(false); });
    }

    public void ShowUp()
    {
        SetRandomType();
        crystalImage.DOFade(1, animationTime);
    }

    public void Click()
    {
        borderImage.gameObject.SetActive(true);
        CrystalClickEvent?.Invoke();
    }
}
