using System;
using System.Collections;
using Dialogue.Objects;
using GameSystems.Combat;
using GameSystems.CustomEventSystems;
using GameSystems.CustomEventSystems.Interaction;
using GameSystems.CustomEventSystems.Tutorial;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

namespace PlayerControl
{
public class PlayerController : MonoBehaviour
{
    
    // Public unity stuff
    public Animator animator;
    public Tilemap tilemap;
    public Sprite[] path;
    public Sprite[] idleSprites;
    
    // Private unity stuff
    private Grid _grid;
    private Rigidbody2D _rb;
    private Vector3Int _lPos;
    private Tile _tile;

    // private variables
    [SerializeField]
    private float _moveSpeed = 5;
    private bool _isSprinting = false;
    private GameObject _lookingAt;
    private bool _playerInRange;
    private SceneLoadManager _sceneManager;
    private SceneChangeAnim _sceneChangeAnim;
    private GameObject _player;

    //Input system
    private PlayerActionControls _playerActionControls;
    
    // animator
    private static readonly int Vertical = Animator.StringToHash("MoveY");
    private static readonly int Horizontal = Animator.StringToHash("MoveX");
    private static readonly int Moving = Animator.StringToHash("Moving");

    private void Start()
    {
        if (GameObject.Find("Grid").GetComponent<Grid>() != null &&
            GameObject.Find("Grid").transform.Find("Ground") != null)
        {
            _grid = GameObject.Find("Grid").GetComponent<Grid>();
            tilemap = GameObject.Find("Grid").transform.Find("Ground").GetComponent<Tilemap>();
        }
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        UpdateRef();
        _sceneManager = SceneLoadManager.Instance;
        _sceneChangeAnim = SceneChangeAnim.Instance;
        _playerActionControls = PlayerActionControlsManager.Instance.PlayerControls;
        _playerActionControls.Land.Sprint.performed += _ => SprintAction(true, 9);
        _playerActionControls.Land.Sprint.canceled += _ => SprintAction(false, 5);
        _playerActionControls.Land.Movement.started += ctx => 
            TutorialHandler.Instance.OnTutorialButtonPressed(ctx);
        _playerActionControls.Land.Movement.canceled += ctx => 
            TutorialHandler.Instance.OnTutorialButtonPressed(ctx);
        _playerActionControls.Land.Interact.started += ctx => 
            TutorialHandler.Instance.OnTutorialButtonPressed(ctx);
        _playerActionControls.Land.Interact.canceled += ctx => 
            TutorialHandler.Instance.OnTutorialButtonPressed(ctx);
        _playerActionControls.Land.Interact.performed += Interact;
        InteractionHandler.Instance.LookingAt += LookingAt;
        SceneManager.sceneLoaded += SetPlayerPosition;
    }

    private void UpdateRef()
    {
        _player = GameObject.Find("Player");
    }
    

    private void Update()
    {
        
        if (_lPos != _grid.WorldToCell(transform.position))
        {
            _lPos = _grid.WorldToCell(transform.position);
            _tile = (Tile) tilemap.GetTile(_lPos);
            if (_isSprinting)
            {
                SetSpeed(9,6);
            }
            else
            {
                SetSpeed(5,4);
            }
        }
    }


    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        var change = Vector2.zero;
        // read movement value
        change = _playerActionControls.Land.Movement.ReadValue<Vector2>();
        if (change != Vector2.zero)
        {
            // Move the player
            Vector2 currentPosition = transform.position;
            currentPosition += change * (_moveSpeed * Time.deltaTime);
            _rb.MovePosition(currentPosition);
            //transform.position = currentPosition;


            // Animate player
            animator.SetFloat(Horizontal, change.x);
            animator.SetFloat(Vertical, change.y);
            animator.SetBool(Moving, true);
        }
        else
        {
            animator.SetBool(Moving, false);
        }
        
    }

    private void LookingAt(GameObject lookingAt, bool inRange)
    {
        _lookingAt = lookingAt;
        _playerInRange = inRange;
    }
    
    private void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && _lookingAt != null && _playerInRange)
        {
            InteractionHandler.Instance.OnInteract();
        }
    }
    
    private void SprintAction(bool isSprinting, int speed)
    {
        _isSprinting = isSprinting;
        _moveSpeed = speed;
    }

    private void SetPlayerPosition(Scene arg0, LoadSceneMode loadSceneMode)
    {
        UpdateRef();
        
        var lastSceneName = SceneLoadHandler.Instance.OnGetLastSceneName();
        if (lastSceneName == "CombatScene" && SceneManager.GetActiveScene().name == "SaveThePrincessForrest")
        {
            Debug.Log(GameObject.Find("CutsceneTrigger"));
            Debug.Log(GameObject.Find("Goblin"));
            GameObject.Find("CutsceneTrigger").SetActive(false);
            GameObject.Find("Goblin").SetActive(false);
        }
        var lastPosition = SceneLoadHandler.Instance.OnGetLastPosition();
        if (arg0.name == lastSceneName)
        {
            if (lastPosition != null)
            {
                switch (SceneLoadHandler.Instance.OnGetLastLeaveDirection())
                {
                    case LeaveDirection.Up:
                        var lastUp = new Vector3(lastPosition.Value.x, lastPosition.Value.y + 1,0);
                        _player.transform.position = lastUp;
                        SetCamera(lastUp);
                        break;
                    case LeaveDirection.Down:
                        var lastDown = new Vector3(lastPosition.Value.x, lastPosition.Value.y- 1,0);
                        _player.transform.position = lastDown;
                        SetCamera(lastDown);
                        break;
                    case LeaveDirection.Left:
                        var lastLeft = new Vector3(lastPosition.Value.x - 1, lastPosition.Value.y,0);
                        _player.transform.position = lastLeft;
                        SetCamera(lastLeft);
                        break;
                    case LeaveDirection.Right:
                        var lastRight = new Vector3(lastPosition.Value.x - 1, lastPosition.Value.y,0);
                        _player.transform.position = lastRight;
                        SetCamera(lastRight);
                        break;
                    case null:
                        return;
                }
                
            }
        }
    }

    private void SetCamera(Vector3 position)
    {
        var camerapos = SceneLoadHandler.Instance.OnGetCamera();
        if (camerapos != null)
        {
            GameObject.Find("Main Camera").transform.position = new Vector3(position.x, position.y, camerapos.Value.z);
        }
    }
    
    
    private void SetSpeed(int onPath, int offPath)
    {
        _moveSpeed = Array.Exists(path,
            element =>
            {
                if (_tile!=null)
                {
                 return element.name == _tile.name;
                }
                return false;
            })
            ? onPath
            : offPath;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Leave"))
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "OpeningCutscene":
                    LoadLevel(3);
                    break;
                case "Library":
                    LoadPrevious();
                    break;
                case "MiniBossBattlePart2": 
                    LoadLevel(16);
                    break;
                case "MeetingCatDogPart2": 
                    LoadLevel(17);
                    break;
                case "MeetingEnemyCutscene":
                    LoadLevel(5);
                        break;
                case "MeetingTheKing": 
                    LoadLevel(13);
                    break;
                case "StartingArea": 
                    LoadLevel(20);
                    break;
                case "SaveThePrincessForrestPart3":
                    LoadLevel(15);
                    break;
                default:
                    LoadLevel();
                    return;
            }
        }

        if (other.CompareTag("Enter") && "OutsideHerosHome 1" == SceneManager.GetActiveScene().name)
        {
            LoadLevel(0);
        }
        
        if (other.CompareTag("Enter") && "StartingArea" == SceneManager.GetActiveScene().name)
        {
            LoadLevel(7);
        }
    }

    private void LoadLevel(int levelIndex = 10)
    {
        InteractionHandler.Instance.OnLevelAnimInt(levelIndex);
    }
    
    private void LoadLevel(string levelName = "WorldMap")
    {
        InteractionHandler.Instance.OnLevelAnimName(levelName);
    }
    private void LoadLevel()
    {
        InteractionHandler.Instance.OnLevelAnimInt(10);
    }

    private void LoadPrevious()
    {
        InteractionHandler.Instance.OnLevelAnimPrevious();
    }
}
    
}



