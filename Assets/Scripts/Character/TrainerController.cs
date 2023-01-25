using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Trainer -> Combatant
public class TrainerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start() {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        // 머리 위 상태 느낌표
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // player까지 걸어감
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y)); 

        yield return character.Move(moveVec);

        // 대화창 생성
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => 
        {
            GameController.Instance.StartTrainerBattle(this);
        }));
    }
    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270f;
        
        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public string Name {
        get => name;
    }    

    public Sprite Sprite {
        get => sprite;
    }
}
