using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Action", menuName = "Actions/Ability")]
public class BaseAction : ScriptableObject, IWeaponAttack
{
    public string Name = null;
    public string description = null;
    public float physicalDmg = 0f;
    public float magicDmg = 0f;
    public float manaCost = 0f; // Mana cost
    public float heal = 0f;
    public float physicalDEF = 0f;
    public float magicalDEF = 0f;

    public void PerformAction(MonoBehaviour mono, GameObject go, Transform target)
    {
        Debug.Log($"Action: {Name} performed!");
        mono.StartCoroutine(AnimPlayer(go, target));
    }

    private IEnumerator AnimPlayer(GameObject go, Transform target) 
    {
        Vector3 startPos = go.transform.position;
        Vector3 targetPos = target.position;
        float duration = 0.5f; 
        float elapsedTime = 0f;

        // Move to target
        while (elapsedTime < duration)
        {
            go.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        go.transform.position = targetPos; 

        yield return new WaitForSeconds(1f); 
        
        // Move back to original position
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            go.transform.position = Vector3.Lerp(targetPos, startPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        go.transform.position = startPos; 

        yield break;
    }
}



