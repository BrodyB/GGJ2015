using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[ExecuteInEditMode]
public class PlayerInventory : MonoBehaviour {

    public int securityLevel = 0;
    public GameObject equippedItem;

    public List<GameObject> items = new List<GameObject>();

    public void Acquire(GameObject target)
    {
        items.Add(target);
        target.SetActive(false);
        target.transform.SetParent(transform);
        target.transform.localPosition = Vector3.zero;
        Debug.Log("Acquired " + target.name + "!");

        //Default to newest item
        equippedItem = items[items.Count - 1];
    }

    public void Drop()
    {
        if (equippedItem != null)
        {
            equippedItem.transform.SetParent(null);
            equippedItem.SetActive(true);
            items.Remove(equippedItem);
        }

        //Default to previously acquired item
        if (items.Count > 0)
            equippedItem = items[items.Count - 1];
        else
            equippedItem = null;
    }
}
