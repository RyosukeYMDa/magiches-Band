using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BattleMainScript : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
