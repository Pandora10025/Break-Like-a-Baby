using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UI;
using TMPro;

public class ObjectManager : MonoBehaviourPun
{
    //private vars
    //place all BreakableObjects in the scene will be put in here through code
    private List<GameObject> bObjects = new List<GameObject>();
    private List<GameObject> activeObjects = new List<GameObject>();

    //public vars
    public static ObjectManager instance { get; private set; }

    //serialized fields
    [SerializeField] private int numOfStartObjects;
    [SerializeField] private int numOfActiveObjects;
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private float breakablePercentage=0.5f;
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("break");
        }
        Debug.Log("break Start");
        instance = this;

        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Breakable"))
        {
            bObjects.Add(g);
        }
        numOfStartObjects = (int)MathF.Ceiling(breakablePercentage * bObjects.Count);
        numOfActiveObjects = numOfStartObjects;

        //numOfActiveObjects = numOfStartObjects;
        Debug.Log("NumOfStartObjects: " + numOfStartObjects);

        

        if (PhotonNetwork.IsMasterClient)
        {
            activeObjects = Randomize();
            SyncActiveObjects();
            Debug.Log("Num of actual activeObjects: " + activeObjects.Count);
        }
        //deactivate all objects, then activate the ones we want
       
        //Activate(activeObjects, true);
    

        
    }

   

    /// <summary>
    /// Method <c>Randomize</c> picks numOfStartObjects amount of 
    /// breakableObjects from a list of all breakableObjects in the scene
    /// </summary>
    /// <returns>A randomized GameObject List stored in activeObjects</returns>
    private List<GameObject> Randomize()
    {
        List<GameObject> g = new List<GameObject>();
        for (int i = 0; i < numOfStartObjects; i++)
        {
            int j = UnityEngine.Random.Range(0, bObjects.Count);
            g.Add(bObjects[j]);
            Debug.Log("aa");
        }
        return g;
        
    }

    private void SyncActiveObjects()
    {
        
        int[] activeObjectIndexes = new int[activeObjects.Count];
        for (int i = 0; i < activeObjects.Count; i++)
        {
            activeObjectIndexes[i] = bObjects.IndexOf(activeObjects[i]);
        }

       
        photonView.RPC("ReceiveActiveObjects", RpcTarget.AllBuffered, activeObjectIndexes);
    }

    [PunRPC]
    private void ReceiveActiveObjects(int[] activeObjectIndexes)
    {
       
        Activate(bObjects, false);
        activeObjects.Clear();
        foreach (int index in activeObjectIndexes)
        {
            if (index >= 0 && index < bObjects.Count)
            {
                activeObjects.Add(bObjects[index]);
            }
        }

        Activate(activeObjects, true);
        UpdateString();
    }

    /// <summary>
    /// Method <c>Activate</c> loops through the list and activates or deactivates all bObjects in it
    /// </summary>
    private void Activate(List<GameObject> gObjects, Boolean b)
    {
        foreach(GameObject g in gObjects)
        {
            g.transform.GetChild(0).GetComponent<BreakableObject>().SetCollider(b);
        }
    }


    /// <summary>
    /// Method <c>Break</c> informs the ObjectManager that this <param>child</param> is broken
    /// and sets flags accordingly
    /// </summary>

    public void Break(GameObject child)
    {
        activeObjects.Remove(child.transform.parent.gameObject);
        Debug.Log("Broken Object: " + activeObjects.ToString());
        numOfActiveObjects--;
        UpdateString();
    }

    /// <summary>
    /// Method <c>UpdateString</c> keeps the menu string in the format Active Objects: x, then list of remaining objects
    /// </summary>
    private void UpdateString()
    {
        
        String s = "";
        s += "Objects remaining: " + numOfActiveObjects + "\n";
        foreach(GameObject g in activeObjects)
        {
            s += g.name + "\n";
        }

        tmp.text = s;
    }

    /// <summary>
    /// Method <c>ToggleText</c> toggles the text to the boolean input
    /// </summary>
    public void ToggleText(bool b)
    {
        tmp.enabled = b;
    }


}
