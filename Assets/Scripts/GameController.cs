using UnityEngine;
using UnityEngine.SceneManagement;
namespace Game{
public class GameController : MonoBehaviour
{
   
   public void OnRePlayClick()
   {
    SceneManager.LoadScene("Gameplay");
   }
}
}