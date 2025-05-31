using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletInfoPanelController : MonoBehaviour
{
    [SerializeField] List<GameObject> BulletsObjects;
    [SerializeField] GameObject LastBullet;
    [SerializeField] TMP_Text BulletInfo;
    [SerializeField] AudioSource AudioSource;


    private int LeftBulletCount = 12;

    public void Reload()
    {
        foreach (var bullet in BulletsObjects)
        {
            bullet.SetActive(true);
        }
        LastBullet.SetActive(true);
        LeftBulletCount = 12;
        BulletInfo.text = $"{LeftBulletCount}/12";
    }

    public void Shoot()
    {
        if (LeftBulletCount > 0)
        {
            LeftBulletCount--;
            BulletsObjects[LeftBulletCount].SetActive(false);

            if (LeftBulletCount == 0)
            {
                LastBullet.SetActive(false);
                BulletInfo.text = $"Reloading";
                AudioSource.Play();
                return;
            }

            BulletInfo.text = $"{LeftBulletCount}/12";
        }
    }

    public bool HasBullet()
    {
        return LeftBulletCount > 0;
    }
}
