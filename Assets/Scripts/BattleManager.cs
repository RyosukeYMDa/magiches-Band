using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public EnemyFactory factory;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var enemy1 = factory.CreateEnemy(EnemyTypeEnum.Slime, new Vector3(0, 0, 0));
        var enemy2 = factory.CreateEnemy(EnemyTypeEnum.Dragon, new Vector3(2, 0, 0));

        enemy1.Attack();
        enemy2.Attack(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            SceneManager.LoadScene("MainScene");
        } 
    }
}
