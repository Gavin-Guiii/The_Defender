using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mathd;

// This script is used to display the list of the POIs.
public class FactoryListItem : MonoBehaviour
{
    public int index;
    public Text placeName;
    public bool isBoss;
    public Sprite bossImage;
    public Sprite normalImage;
    public Sprite collectibleSprite;
    public Image enemyImage;
    public Text coinNumber;
    public Button directionButton;
    public Vector2d TargetCoordinates;
    public GameObject normalStatus;
    public GameObject claimedStatus;
    public GameObject RewardPrompt;

    // Read current status of the POI and display accordingly
    public void UpdateStatus(bool hasDefeated, bool hasClaimedReward)
    {
        // Set different POI images according to whether the enemy is a boss
        if (isBoss)
        {
            enemyImage.sprite = bossImage;
            coinNumber.text = "200";
        }
        else
        {
            enemyImage.sprite = normalImage;
            coinNumber.text = "100";
        }
        
        // Set "Claim Reward" button clickable/non-clickable according to whether it has been defeated and whether the reward has been claimed
        if (!hasDefeated)
        {
            normalStatus.SetActive(true);
            claimedStatus.SetActive(false);
            normalStatus.GetComponent<Button>().interactable = false;
        }
        else
        {
            if (hasClaimedReward)
            {
                normalStatus.SetActive(false);
                claimedStatus.SetActive(true);
            }
            else
            {
                normalStatus.SetActive(true);
                claimedStatus.SetActive(false);
                normalStatus.GetComponent<Button>().interactable = true;
                normalStatus.GetComponent<Image>().sprite = collectibleSprite;
            }
        }
    }

    // Implementment the "Claim Reward" button
    public void ClaimReward()
    {
        GameObject newPrompt = Instantiate(RewardPrompt);
        newPrompt.transform.SetParent(GameObject.Find("MissionPanel").transform, false);
        if (isBoss)
        {
            newPrompt.GetComponent<LevelupPopup>().CoinText.text = "200";
            GameManager.Instance.AddCoin(200);
        }
        else
        {
            newPrompt.GetComponent<LevelupPopup>().CoinText.text = "100";
            GameManager.Instance.AddCoin(100);
        }
        normalStatus.SetActive(false);
        claimedStatus.SetActive(true);
        POIDataHelper.instance.SetHasClaimedReward(index, true);
    }

    // Implementment the "Google Map Direction" button
    public void CallGoogleMapDirection()
    {
        Application.OpenURL("https://www.google.com/maps/search/?api=1&query=" + TargetCoordinates.x + "%2C" + TargetCoordinates.y);
    }
}
