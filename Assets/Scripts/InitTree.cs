using UnityEngine;

public class InitTree : MonoBehaviour
{
    Wood thisWood;
    void Start() // This script activates on RunTime when tree is dropped on the map.
    {
        thisWood = new Wood(this.gameObject);
        NavCharacterController.instance.AddWoodToCut(thisWood);
        // NavCharacterController.instance.FindTreeToCut();
    }

    private void OnDestroy() => NavCharacterController.instance.RemoveWoodToCut(thisWood); // Simple trick to remove this object from list if object was returned from the map.
}
