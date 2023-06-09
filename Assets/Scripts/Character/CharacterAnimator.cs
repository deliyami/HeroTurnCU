using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> surfSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;

    // param
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }
    public bool IsJumping { get; set; }
    public bool IsSurfing { get; set; }

    // state
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;
    bool isCutsceneEvent;
    Character character;

    // References
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        character = GetComponent<Character>();
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var prevAnim = currentAnim;
        if (MoveX == 1)
            currentAnim = walkRightAnim;
        else if (MoveX == -1)
            currentAnim = walkLeftAnim;
        else if (MoveY == 1)
            currentAnim = walkUpAnim;
        else if (MoveY == -1)
            currentAnim = walkDownAnim;

        // if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
        if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving || isCutsceneEvent)
        {
            if (isCutsceneEvent)
                Debug.Log("siubla");
            isCutsceneEvent = false;
            currentAnim.Start();
        }
        if (IsJumping)
            spriteRenderer.sprite = currentAnim.Frames[currentAnim.Frames.Count - 1];
        else if (IsMoving)
            currentAnim.HandleUpdate();
        // Debug.Log("this is animator");
        else
            spriteRenderer.sprite = currentAnim.Frames[0];

        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir, bool cutsceneEvent = false)
    {
        MoveX = 0;
        MoveY = 0;
        if (dir == FacingDirection.Right)
            MoveX = 1;
        else if (dir == FacingDirection.Left)
            MoveX = -1;
        else if (dir == FacingDirection.Up)
            MoveY = 1;
        else if (dir == FacingDirection.Down)
            MoveY = -1;

        if (cutsceneEvent)
            isCutsceneEvent = cutsceneEvent;
        // currentAnim = walkUpAnim;
        // currentAnim.HandleUpdate();
        // character.HandleUpdate();
    }

    public FacingDirection DefaultDirection
    {
        get => defaultDirection;
    }
}

public enum FacingDirection { Up, Down, Right, Left }