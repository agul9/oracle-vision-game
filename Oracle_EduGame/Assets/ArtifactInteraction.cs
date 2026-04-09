using UnityEngine;

public class ArtifactInteraction : MonoBehaviour
{
    public GameObject muralToShow;
    public GameObject boneCharacter;

    public void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateMural();
        }
    }

    void ActivateMural ()
    {
        muralToShow.SetActive(true);
        if (boneCharacter != null)
        {
         boneCharacter.SetActive(true);   
        }
        
        Debug.Log("The mural should be showing now...");
    }
}
