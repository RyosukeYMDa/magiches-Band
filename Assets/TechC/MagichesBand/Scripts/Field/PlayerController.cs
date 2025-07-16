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
    ///     プレイヤーキャラクターのコントローラー
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
        private const float MoveSpeed = 10f;
        private Vector3 moveInput;
        [SerializeField] private string testAnimationName = "walk";
        private Quaternion targetRotation;
        private const float RotationSpeed = 10f;
        private Vector3 moveDirection; // 移動方向
        private Vector3 playerDirection; // プレイヤーの向いている方向
        private PlayerInput playerInput; 
    
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
        
        private float footstepTimer = 0f;
        private const float FootstepInterval = 0.4f; // 秒ごとに足音


        private void Start()
        {
            isShown = false;
            
            rb =  GetComponent<Rigidbody>();
            //空気抵抗
            rb.linearDamping = 10f;
            gameObject.transform.position = GameManager.Instance.playerPosition;
            gameObject.transform.rotation = GameManager.Instance.playerRotation;
            
            //これが無いとspineのanimationが動かない
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            spAnimationState = skeletonAnimation.AnimationState;
        
            playerInput = GetComponent<PlayerInput>();
            var moveAction = playerInput.actions["Move"];
            var menuAction = playerInput.actions["Menu"];
            
            moveAction.performed += OnMovePerformed;
            moveAction.canceled += OnMoveCanceled;
            menuAction.performed += Menu;
        }
    
        private void OnDisable()
        {
            if (playerInput && playerInput.actions)
            {
                var moveAction = playerInput.actions["Move"];
                var menuAction = playerInput.actions["Menu"];
                
                moveAction.performed -= OnMovePerformed;
                moveAction.canceled -= OnMoveCanceled;
                menuAction.performed -= Menu;
            }
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
    
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (isShown || inventoryUI.isInventory || dissolveController.nowLoading) return;

            Vector2 input = context.ReadValue<Vector2>();
            moveInput = new Vector3(input.x, 0f, input.y);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            moveInput = Vector3.zero;
        }
        
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
                        Debug.Log("Area移動");
                        transform.position = new Vector3(collision.transform.position.x + 3f, transform.position.y,
                            transform.position.z);
                        dissolveController.areaState = DissolveController.AreaState.RuinsArea;
                        break;
                    case DissolveController.AreaState.RuinsArea:
                        cameraController.CamRotation(-90f);
                        Debug.Log("RuinsArea");
                        transform.position = new Vector3(collision.transform.position.x - 4f, transform.position.y,
                            transform.position.z);
                        dissolveController.areaState = DissolveController.AreaState.ResidenceArea;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                MessageWindow.Instance.DisplayMessage("Area Movement", () =>
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
                
                MessageWindow.Instance.DisplayMessage("Enemy Encount", () =>
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

        public void LogMessage()
        {
            Debug.Log("walk");
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