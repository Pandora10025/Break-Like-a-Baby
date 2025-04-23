using UnityEngine;
using UnityEngine.Playables;

public class SkipCutScene : MonoBehaviour
{
    public PlayableDirector timeline;
    public float newSpeed;

  
    private void Update()
    {
        //ChangeTImelineSpeed();
    }
    void ChangeTImelineSpeed()
    {

        timeline.playableGraph.GetRootPlayable(0).SetSpeed(newSpeed);
    }
    
    public void SetSpeed(float speed)
    {
        newSpeed = speed;
        ChangeTImelineSpeed();
    }
}
