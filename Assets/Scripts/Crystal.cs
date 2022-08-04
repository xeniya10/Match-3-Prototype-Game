using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;
using System.Collections;

public class Crystal : MonoBehaviour
{
    [SerializeField] private RectTransform crystalRectTransform;
    [SerializeField] private CrystalSprites crystalSprites;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image sparkleImage;

    [Space]
    public int CrystalType = 0;
    [SerializeField] private float fadeTime = 0.1f;
    public event Action<Crystal> ClickEvent;
    public event Action<Crystal> DisappearEvent;

    public Crystal Create(Transform parent)
    {
        sparkleImage.gameObject.SetActive(false);
        borderImage.gameObject.SetActive(false);

        var crystal = Instantiate(this, parent);
        crystal.SetRandomType();
        return crystal;
    }

    public void SetRandomType()
    {
        CrystalType = Random.Range(0, crystalSprites.Sprites.Count);
        crystalImage.sprite = crystalSprites.Sprites[CrystalType];
    }

    public void SetSize(float size)
    {
        crystalRectTransform.sizeDelta = new Vector2(size, size);
    }

    public void SetPosition(float x, float y)
    {
        transform.localPosition = new Vector2(x, y);
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

    public void Disappear()
    {
        borderImage.gameObject.SetActive(false);
        var disappearingCrystal = crystalImage.DOFade(0, fadeTime);
        disappearingCrystal.OnComplete(() => this.Sparkle());
    }

    private void Sparkle()
    {
        sparkleImage.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(sparkleImage.DOFade(1, fadeTime)).
        Append(sparkleImage.DOFade(0, fadeTime)).
        OnComplete(() =>
        {
            sparkleImage.gameObject.SetActive(false);
            gameObject.SetActive(false);
            DisappearEvent?.Invoke(this);
        });
    }

    public IEnumerator MoveToPosition(Vector3 position)
    {
        var move = transform.DOLocalMove(position, fadeTime);
        yield return move.WaitForCompletion();
    }

    public void ShowUp(Action CallBack)
    {
        SetRandomType();
        gameObject.SetActive(true);
        var appearingCrystal = crystalImage.DOFade(1, fadeTime);
        appearingCrystal.OnComplete(() => CallBack?.Invoke());
    }

    private void Select()
    {
        borderImage.gameObject.SetActive(true);
    }

    public void Deselect()
    {
        borderImage.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        Select();
        ClickEvent?.Invoke(this);
    }
}
