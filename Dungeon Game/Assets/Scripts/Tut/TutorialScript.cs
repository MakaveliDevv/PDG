using System.Collections;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    // At the start of the game
    public GameObject welcomeText;
    public GameObject descriptionAboutGameText;
    public GameObject movementInputUI;

    // At chest
    public GameObject interactUI;
    public GameObject descriptionAboutTheChest;

    // At enemy
    public GameObject descriptionOnEncounter;
    
    // In battle
    // public GameObject introTextForBattle;
    // public GameObject selectHeroText;
    
    public GameObject confirm_Text;
    public GameObject selectTarget_Text;
    public GameObject selectActionType_Text;
    public GameObject selectAttackType_Text;
    public GameObject selectDefenseAction_Text;
    public GameObject selectWattAction_Text;
    public GameObject selectMattAction_Text;

    public GameObject defeatedEnemy;


    void Start()
    {
        if(GameManager.instance.gamePlay == GamePlay.TUTORIAL) 
        {
            // StartCoroutine(DisplayWelcomeText());
            StartCoroutine(DisplayText(defeatedEnemy, 2f));
        }
    }

    public IEnumerator DisplayWelcomeText() 
    {
        welcomeText.SetActive(true);

        yield return new WaitForSeconds(2f);

        descriptionAboutGameText.SetActive(true);
        
        yield return new WaitForSeconds(5f);

        welcomeText.SetActive(false);
        descriptionAboutGameText.SetActive(false);

        yield return new WaitForSeconds(1f);

        movementInputUI.SetActive(true);

        yield return new WaitForSeconds(5f);

        movementInputUI.SetActive(false);
        
        yield break;
    }

    public IEnumerator DisplayChestDescUI() 
    {
        yield return new WaitForSeconds(.25f);

        descriptionAboutTheChest.SetActive(true);
        
        yield return new WaitForSeconds(3f);

        descriptionAboutTheChest.SetActive(false);
        interactUI.SetActive(true);

        yield return new WaitForSeconds(1.25f);
        interactUI.SetActive(false);

        yield break;
    }

    public IEnumerator DisplayEnemyEncounterUI() 
    {
        descriptionOnEncounter.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        descriptionOnEncounter.SetActive(false);

        yield break;
    }

    public IEnumerator DisplayBattleText(GameObject introTextForBattle, GameObject selectHeroText) 
    {
        yield return new WaitForSeconds(.5f);

        introTextForBattle.SetActive(true);

        yield return new WaitForSeconds(5f);

        introTextForBattle.SetActive(false);

        yield return new WaitForSeconds(.5f);
        
        selectHeroText.SetActive(true);

        yield return new WaitForSeconds(3f);

        selectHeroText.SetActive(false);

        yield break;
    }

    public IEnumerator DisplayText(GameObject go, float waitingTime) 
    {
        yield return new WaitForSeconds(.5f);

        go.SetActive(true);
        
        yield return new WaitForSeconds(waitingTime);

        go.SetActive(false);

        yield break;
    }
}
