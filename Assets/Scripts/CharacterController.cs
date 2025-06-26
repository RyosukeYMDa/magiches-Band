using System;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//コンポーネント付け忘れ防止用Attribute(属性)
[RequireComponent(typeof(SkeletonAnimation))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    private enum PlayerState
    {
        Idle,
        Walk
    }

    //playerのstate
    private PlayerState playerState = PlayerState.Idle;
    
    //playerの動き関連
    private const float MoveSpeed = 10f;
    private Vector3 moveInput;
    [SerializeField] private string testAnimationName = "walk";
    private Quaternion targetRotation;
    private const float RotationSpeed = 10f;
    private Vector3 moveDirection; // 移動方向
    private Vector3 playerDirection; // プレイヤーの向いている方向

    //spineAnimation関連
    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState spAnimationState;

    //カメラの位置
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private LoadingShaderController loadingShaderController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private ButtonNavigator buttonNavigator;

    private Rigidbody rb;
    
    public bool isShown; //menubarが出ているかどうか

    private void Start()
    {
        rb =  GetComponent<Rigidbody>();
        //空気抵抗
        rb.linearDamping = 10f;
        gameObject.transform.position = GameManager.Instance.playerPosition;

        //これが無いとspineのanimationが動かない
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spAnimationState = skeletonAnimation.AnimationState;
    }

    private void Update()
    {
        //state管理
        switch (playerState)
        {
            case PlayerState.Idle:
                if(moveInput != Vector3.zero)
                    ChangeState(PlayerState.Walk);
                break;
            
            case PlayerState.Walk:
                if (moveInput == Vector3.zero)
                    ChangeState(PlayerState.Idle);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        UpdateDirection();
    }

    // 向き（回転）の更新
    private void UpdateDirection()
    {
        // 入力があった時だけ向きを更新
        if (moveInput != Vector3.zero)
        {
            //カメラの回転関連
            moveDirection = cameraTransform.rotation * moveInput;

            var camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();

            if (moveInput.x > 0)
                targetRotation = Quaternion.LookRotation(camForward);
            else if (moveInput.x < 0)
                targetRotation = Quaternion.LookRotation(-camForward);
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * MoveSpeed;
    }

    //InputSystemから呼び出されるplayerの動きの入力情報更新
    public void UpdateMoveInput(InputAction.CallbackContext context)
    {
        if (context.performed && !isShown && !buttonNavigator.isInventory)
        {
            //playerの移動
            var input = context.ReadValue<Vector2>();
            moveInput = new Vector3(input.x, 0f, input.y);
        }

        // 入力が終了したときは移動を止める
        if (context.canceled) moveInput = Vector3.zero;
    }

    /// <summary>
    /// Stateを変える関数
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeState(PlayerState newState)
    {
        if (playerState == newState) return;

        playerState = newState;

        //stateの状態によって再生するSpineAnimationを変える
        switch (newState)
        {
            case PlayerState.Idle:
                spAnimationState.SetAnimation(0, "idle", true);
                break;
            case PlayerState.Walk:
                spAnimationState.SetAnimation(0, "walk", true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }


    /// <summary>
    /// 今はAreaを移動した際の処理を書いている
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {  
        //Area境にあるBoxColliderに当たった時のみ処理
        if (collision.gameObject.CompareTag("CameraRotation"))
        {
            //loading用のShaderを再生
            StartCoroutine(loadingShaderController.PlayEffect());
            
            switch (loadingShaderController.areaState)
            {
                //Playerが今いるAreaのStateによって処理を変える
                case LoadingShaderController.AreaState.ResidenceArea:
                    cameraController.CamRotation(90f);
                    Debug.Log("Area移動");
                    transform.position = new Vector3(collision.transform.position.x + 3f, transform.position.y,
                        transform.position.z);
                    loadingShaderController.areaState = LoadingShaderController.AreaState.RuinsArea;
                    break;
                case LoadingShaderController.AreaState.RuinsArea:
                    cameraController.CamRotation(-90f);
                    Debug.Log("RuinsArea");
                    transform.position = new Vector3(collision.transform.position.x - 4f, transform.position.y,
                        transform.position.z);
                    loadingShaderController.areaState = LoadingShaderController.AreaState.ResidenceArea;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else if (collision.gameObject.CompareTag("Enemy1"))
        {
            //Scene遷移時に再生する
            StartCoroutine(loadingShaderController.PlayEffect());

            //scene遷移時にplayerの座標を記録させる
            GameManager.Instance.playerPosition = collision.transform.position;

            GameManager.Instance.enemyType = GameManager.EnemyType.Enemy1;

            SavePlayerPosition();
            
            SceneManager.LoadScene("BattleScene");
        }
        else
        {
            Debug.Log("壁");
        }
    }

    /// <summary>
    /// playerのpositionを記録
    /// </summary>
    public void SavePlayerPosition()
    {
        GameManager.Instance.playerPosition = transform.position;
        
         var playerData = new PlayerData(GameManager.Instance.playerPosition);
         var inventory = GameManager.Instance.inventory;

         SaveManager.SavePlayerData(playerData, inventory);
        Debug.Log("Save"); 
    }
}