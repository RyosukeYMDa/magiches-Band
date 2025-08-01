using System;
using Spine.Unity;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Game;
using TechC.MagichesBand.Item;
using TechC.MagichesBand.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// プレイヤーキャラクターのコントローラー
    /// </summary>
    [RequireComponent(typeof(SkeletonAnimation))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Walk
        }

        //playerのstate
        private State state = State.Idle;
    
        //playerの動き関連
        [SerializeField] private string testAnimationName = "walk";
        private const float MoveSpeed = 10f; // プレイヤーの移動速度
        private Vector3 moveInput; // 入力された移動方向
        private Quaternion targetRotation; // 回転先
        private const float RotationSpeed = 10f; // 回転速度
        private Vector3 moveDirection; // 移動方向
        private Vector3 playerDirection; // プレイヤーの向いている方向
        
        private PlayerInput playerInput;
        private const float PlayerWarpOffset = 4f; // エリア移動時のプレイヤー移動量

        //spineAnimation関連
        private SkeletonAnimation skeletonAnimation;
        private Spine.AnimationState spAnimationState;

        //カメラの位置
        [SerializeField] private Transform cameraTransform;
        [FormerlySerializedAs("disolveController")] [FormerlySerializedAs("loadingShaderController")] [SerializeField] private DissolveController dissolveController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private GameObject menuBar;
        [SerializeField] private GameMenu gameMenu;

        private Rigidbody rb;
    
        public bool isShown; //menubarが出ているかどうか
        
        // 足音関連
        private float footstepTimer = 0f;
        private const float FootstepInterval = 0.4f; // 秒ごとに足音
        
        //キャッシュ
        private GameManager gameManager;
        private Sound sound;
        private MessageWindow messageWindow;

        private void Awake()
        {
            //キャッシュ
            gameManager = GameManager.Instance;
            sound = Sound.Instance;
            messageWindow = MessageWindow.Instance;
        }
        
        private void Start()
        {
            isShown = false;
            
            rb =  GetComponent<Rigidbody>();
            //空気抵抗
            rb.linearDamping = 10f;
            
            // 前回セーブされた位置・角度に復元
            gameObject.transform.position = GameManager.Instance.playerPosition;
            gameObject.transform.rotation = GameManager.Instance.playerRotation;
            
            //これが無いとspineのanimationが動かない
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            spAnimationState = skeletonAnimation.AnimationState;
        
            // Inputの設定
            playerInput = GetComponent<PlayerInput>();
            var moveAction = playerInput.actions["Move"];
            var menuAction = playerInput.actions["Menu"];
            
            moveAction.performed += OnMovePerformed;
            moveAction.canceled += OnMoveCanceled;
            menuAction.performed += Menu;
        }
    
        private void OnDisable()
        {
            if (!playerInput || !playerInput.actions) return;
            
            // Inputイベント解除
            var moveAction = playerInput.actions["Move"];
            var menuAction = playerInput.actions["Menu"];
                
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
            menuAction.performed -= Menu;
        }

        private void Update()
        {
            //state管理
            switch (state)
            {
                case State.Idle:
                    if(moveInput != Vector3.zero)
                        ChangeState(State.Walk);
                    break;
            
                case State.Walk:
                    if (moveInput == Vector3.zero)
                        ChangeState(State.Idle);
                    else
                    {
                        // 歩行中だけ足音のタイマーを更新
                        footstepTimer += Time.deltaTime;
                        if (footstepTimer >= FootstepInterval)
                        {
                            PlayFootstepSound();
                            footstepTimer = 0f;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        
            UpdateDirection(); // 向きの更新
        }

        /// <summary>
        /// 入力に応じてプレイヤーの向きを変える処理
        /// </summary>
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

                targetRotation = moveInput.x switch
                {
                    > 0 => Quaternion.LookRotation(camForward),
                    < 0 => Quaternion.LookRotation(-camForward),
                    _ => targetRotation
                };
            }
            else
            {
                moveDirection = Vector3.zero;
            }

            // プレイヤーの回転補間
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
        
        /// <summary>
        /// RigidbodyのlinearVelocityを使ってプレイヤーを移動させる処理
        /// FixedUpdateで実行し物理演算に対応させる
        /// moveDirectionベクトルと移動速度を乗算して移動量とする
        /// </summary>
        private void FixedUpdate()
        {
            rb.linearVelocity = moveDirection * MoveSpeed;
        }
    
        /// <summary>
        /// 移動入力が行われたときに呼ばれる処理
        /// 入力ベクトルを取得しY軸を除外したXZ平面の移動ベクトルとして保存する
        /// </summary>
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (isShown || inventoryUI.isInventory || dissolveController.nowLoading) return;

            var input = context.ReadValue<Vector2>();
            moveInput = new Vector3(input.x, 0f, input.y);
        }

        /// <summary>
        /// 移動入力がキャンセルされたときに呼ばれる処理
        /// 移動ベクトルをゼロに設定し停止状態にする
        /// </summary>
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            moveInput = Vector3.zero;
        }
        
        /// <summary>
        /// メニューを開閉する処理
        /// </summary>
        public void Menu(InputAction.CallbackContext context)
        {
            if(inventoryUI.isInventory)return;
            
            Debug.Log("Menu");

            if (!isShown)
            {
                menuBar.SetActive(true);
            }
            else
            {
                gameMenu.CloseMenu();
            }
        }

        /// <summary>
        /// プレイヤーの足音を再生する処理
        /// SoundManagerを通してFootstepサウンドを再生する
        /// </summary>
        private void PlayFootstepSound()
        {
            Sound.Instance.Play(SoundType.PlayerFootstep);
        }
        
        /// <summary>
        /// Stateを変える関数
        /// </summary>
        /// <param name="newState"></param>
        private void ChangeState(State newState)
        {
            if (state == newState) return;

            state = newState;

            //stateの状態によって再生するSpineAnimationを変える
            switch (newState)
            {
                case State.Idle:
                    spAnimationState.SetAnimation(0, "idle", true);
                    break;
                case State.Walk:
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
                dissolveController.PlayEffect();
            
                switch (dissolveController.areaState)
                {
                    //Playerが今いるAreaのStateによって処理を変える
                    case DissolveController.AreaState.ResidenceArea:
                        cameraController.CamRotation(90f);
                        Debug.Log("Areaを移動");
                        transform.position = new Vector3(collision.transform.position.x + PlayerWarpOffset, transform.position.y,
                            transform.position.z);
                        dissolveController.areaState = DissolveController.AreaState.RuinsArea;
                        break;
                    case DissolveController.AreaState.RuinsArea:
                        cameraController.CamRotation(-90f);
                        Debug.Log("RuinsArea");
                        transform.position = new Vector3(collision.transform.position.x - PlayerWarpOffset, transform.position.y,
                            transform.position.z);
                        dissolveController.areaState = DissolveController.AreaState.ResidenceArea;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                MessageWindow.Instance.DisplayMessage("Areaを移動", () =>
                {
                    Sound.Instance.Play(SoundType.AreaMovement);
                    dissolveController.StopEffect();
                });
            }
            else if (collision.gameObject.CompareTag("Enemy1"))
            {
                //Scene遷移時に再生する
                dissolveController.PlayEffect();

                //scene遷移時にplayerの座標を記録させる
                GameManager.Instance.playerPosition = collision.transform.position;

                GameManager.Instance.enemyType = GameManager.EnemyType.Enemy1;

                SavePlayerPosition();
                
                MessageWindow.Instance.DisplayMessage("敵が出てきた", () =>
                {
                    Sound.Instance.Play(SoundType.AreaMovement);
                    dissolveController.StopEffect();
                    SceneManager.LoadScene("Battle");
                });
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
            GameManager.Instance.playerRotation = gameObject.transform.rotation;
        
            var playerData = new PlayerData(GameManager.Instance.playerPosition, GameManager.Instance.playerRotation);
            var cameraDate = new CameraData(GameManager.Instance.cameraOffset,GameManager.Instance.cameraRotation);
            var inventory = GameManager.Instance.inventory;

            SaveManager.SavePlayerData(playerData, inventory,cameraDate);
            Sound.Instance.Play(SoundType.ButtonSelect);
            MessageWindow.Instance.DisplayMessage("Save");
        }
    }
}