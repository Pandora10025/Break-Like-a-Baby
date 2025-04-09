using UnityEngine;

public class Crib : MonoBehaviour
{
    [SerializeField] GameObject[] babySleeping;
    BreakableObject breakable;
    void Awake()
    {
        GameManager.instance.crib = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(GameObject baby in babySleeping)
        {
            baby.SetActive(false);
        }
        breakable.Inactive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void babyBedded(int baby)
    {
        babySleeping[baby].SetActive(true);
        breakable.Active();

    }

    public void Break()
    {
        GameManager.instance.playerCaught.GetComponent<PlayerCatching>().changeState(PlayerCatching.playerCatchState.free);
    }
}
