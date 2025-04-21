using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    [SerializeField] private string nextScene;
 
    public void loadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }


}
