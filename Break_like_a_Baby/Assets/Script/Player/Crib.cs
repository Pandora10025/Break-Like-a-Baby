using UnityEngine;

public class Crib : MonoBehaviour
{
    [SerializeField] Sprite[] babySleeping;
    [SerializeField] GameObject baby;
    BreakableObject breakable;
    void Awake()
    {
        GameManager.instance.crib = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

      
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
