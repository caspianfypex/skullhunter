using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{

    [SerializeField] private GameObject tabButton;    

    //Makes shop usable if player is in its trigger
    private void OnTriggerEnter(Collider other)
    {
        GameManager.GetGameManager().canOpenShop = !GameManager.GetGameManager().canEnemiesSpawn;
        tabButton.SetActive(GameManager.GetGameManager().canOpenShop);
    }

    //Makes shop unusable if player gets far away from it
    private void OnTriggerExit(Collider other)
    {
        tabButton.SetActive(false);
        GameManager.GetGameManager().canOpenShop = false;
        GameManager.GetGameManager().shopMenu.SetActive(false);
    }

}
