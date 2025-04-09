using UnityEngine;

public class Crib : MonoBehaviour
{
    [SerializeField] Sprite[] babySleeping;
    [SerializeField] GameObject baby;
    BreakableObject breakable;
    void Awake()
    {
       
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.crib = this;
        breakable = GetComponent<BreakableObject>();
        baby.SetActive(false);
        
        breakable.Inactive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void babyBedded(int colorID)
    {
        baby.SetActive(true);
        baby.GetComponent<SpriteRenderer>().sprite = babySleeping[colorID];
        breakable.Active();

    }

    public void Break()
    {
        GameManager.instance.playerCaught.GetComponent<PlayerCatching>().changeState(PlayerCatching.playerCatchState.free);
    }
}
