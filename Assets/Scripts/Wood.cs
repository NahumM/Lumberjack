using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood
{
    public GameObject treeGameObject;
    List<GameObject> trunks = new List<GameObject>();
    int hitsLeft;

    public Wood(GameObject tree)
    {
        hitsLeft = 3;
        treeGameObject = tree;
        for (int i = 0; i < 3; i++)
            trunks.Add(treeGameObject.transform.GetChild(i).gameObject); // Lists all 3 trunks from the tree
    }

    public void TreeHit() // Each tree have 3 hits before it falls. Each hit casted from CharController script;
    {
        hitsLeft -= 1;
        if (hitsLeft == 0)
        {
            FallTheTree();

            foreach (GameObject trunk in trunks)
                NavCharacterController.instance.trunksToPickUp.Add(trunk);
        }
    }

    void FallTheTree() // If tree is falling - activates rigidbodies to make it fall.
    {
        for (int i = treeGameObject.transform.childCount - 1; i >= 0; i--)
        {
            var rb = treeGameObject.transform.GetChild(i).GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = false;
            treeGameObject.transform.GetChild(i).parent = null;
        }
        NavCharacterController.instance.RemoveWoodToCut(this);
    }
}
