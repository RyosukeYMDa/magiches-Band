using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

namespace TechC.MagichesBand.Core
{
    public class ClearGame : MonoBehaviour
    {
        private void Start()
        {
            Sound.Instance.Play(SoundType.EndingBGM,true);
        }
        
        private void Update()
        {
            if(Gamepad.current == null)
                return;

            if (Gamepad.current.allControls.Any(control => control is ButtonControl { wasPressedThisFrame: true }))
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
}