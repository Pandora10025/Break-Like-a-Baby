using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ADD ALL BREAKABLE OBJECTS MANUALLY OR ELSE WE'RE GOING TO FUCKING DIE
/// </summary>
public class ObjectManager : MonoBehaviourPun
{
    //private vars
    private int pos_index;

    //place all BreakableObjects in the scene will be put in here through code
    [Tooltip("Add all breakable objects to here for them to work!")]
    [SerializeField] private List<GameObject> bObjects = new List<GameObject>();
    private List<GameObject> activeObjects = new List<GameObject>();

    //public vars
    public static ObjectManager instance { get; private set; }

    //serialized fields
    [SerializeField] public int numOfStartObjects;
    [SerializeField] public int numOfActiveObjects;
    [SerializeField] private TextMeshProUGUI tmp;
    [UnityEngine.RangeAttribute(0, 1)]
    [SerializeField] private float breakablePercentage = 0.5f;
    [SerializeField] GameObject[] emptyImageSlots;
    [SerializeField] GameObject iconList;
    [SerializeField] GameObject line;
    private List<GameObject> startingObjects = new List<GameObject>();
    private void Awake()
    {
        instance = this;
        //CarrySoundOver.instance.StopMusic();
       
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("break");
        }
        Debug.Log("break Start");

        //stop prev music



        numOfStartObjects = (int)MathF.Ceiling(breakablePercentage * bObjects.Count);
        numOfActiveObjects = numOfStartObjects;

       
    }
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            activeObjects = Randomize();
            SyncActiveObjects();
            Debug.Log("Num of actual activeObjects: " + activeObjects.Count);
        }


        pos_index = 0;


        //numOfActiveObjects = numOfStartObjects;
        Debug.Log("NumOfStartObjects: " + numOfStartObjects);

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
            if (!g.Contains(bObjects[j]))
            {
                g.Add(bObjects[j]);
            }
            else
            {
                i--;
            }
            //Debug.Log("aa");
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
        startingObjects = new List<GameObject>(activeObjects);
        UpdateString();
        
        AudioManager.instance.Populate();
        
    }

    /// <summary>
    /// Method <c>Activate</c> loops through the list and activates or deactivates all bObjects in it
    /// </summary>
    private void Activate(List<GameObject> gObjects, Boolean b)
    {
        foreach (GameObject g in gObjects)
        {
            if (b)
            {
                g.transform.GetChild(0).GetComponent<BreakableObject>().Active();
                g.transform.GetChild(0).GetComponent<BoxRockerTest>().DisableOutlines();
            }
            else//TODO: why are these more-or-less the same?
            {

                g.transform.GetChild(0).GetComponent<BreakableObject>().Inactive();
                g.transform.GetChild(0).GetComponent<BoxRockerTest>().DisableOutlines();
            }
        }
    }


    /// <summary>
    /// Method <c>Break</c> informs the ObjectManager that this <paramref name="child"/> is broken
    /// and sets flags accordingly
    /// </summary>

    public void Break(GameObject child)
    {
        //sound stuff, fix param 1
        AudioManager.instance.PlaySFX(child.transform.parent.name, child.transform.position);

        //break and remove from List
        child.GetComponent<BreakableObject>().Break();
        activeObjects.Remove(child.transform.parent.gameObject);
        //Debug.Log("Broken Object: " + activeObjects.ToString());
        numOfActiveObjects--;
        if (numOfActiveObjects <= 0)
        {
            GameManager.instance.GameOver(true);
        }
        UpdateString();
    }

    /// <summary>
    /// Method <c>UpdateString</c> keeps the menu string in the format Active Objects: x, then list of remaining objects
    /// </summary>
    private void UpdateString()
    {
        foreach (GameObject slot in emptyImageSlots)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        Debug.Log(startingObjects.Count);
        
        for (int i = 0; i < startingObjects.Count; i++)
        {
            GameObject slot = emptyImageSlots[i];
            GameObject obj = startingObjects[i];

            slot.SetActive(true);

            if (i == 2)
            {
                Debug.Log(startingObjects[i].transform.parent.name);
            }
            Image img = slot.GetComponent<Image>();
            img.sprite = obj.transform.GetChild(0).GetComponent<BreakableObject>().breakImage;

            
            if (!activeObjects.Contains(obj))
            {
                InstantiateCrossOnSlot(slot);
            }
        }

        
        for (int j = startingObjects.Count; j < emptyImageSlots.Length; j++)
        {
            emptyImageSlots[j].SetActive(false);
        }
    }
    private void InstantiateCrossOnSlot(GameObject slot)
    {
        GameObject cross = Instantiate(line, slot.transform); 
        RectTransform crossRect = cross.GetComponent<RectTransform>();

       
        crossRect.anchorMin = Vector2.zero;
        crossRect.anchorMax = Vector2.one;
        crossRect.offsetMin = Vector2.zero;
        crossRect.offsetMax = Vector2.zero;
        crossRect.localScale = Vector3.one;

        cross.transform.SetAsLastSibling();
    }

    /// <summary>
    /// Method <c>ToggleText</c> toggles the text to the boolean input
    /// </summary>
    public void ToggleText(bool b)
    {
        iconList.SetActive(b);
    }

    /// <summary>
    /// Method <c>getBObjs</c> returns all bObjs
    /// </summary>
    /// <returns>List of all bObjs</returns>
    public List<GameObject> getBObjs()
    {
        return activeObjects;
    }

    public bool AreBObjsAwake()
    {
        foreach (GameObject g in activeObjects)
        {
            return g.transform.GetChild(0).GetComponent<BreakableObject>().isAwake();
        }
        return true;
    }
}
