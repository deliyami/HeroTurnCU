using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class IntroManager : MonoBehaviour
{
    [SerializeField] GameObject introObject;
    [SerializeField] GameObject gameDescriptionObject;
    [SerializeField] GameObject gameplayObject;
    [SerializeField] GameObject backgroundObject;
    [SerializeField] Sprite background2;
    [SerializeField] Sprite background3;
    [SerializeField] GameObject pixelBackground;
    [SerializeField] GameObject heroObject;
    [SerializeField] GameObject slimeObject;
    [SerializeField] Image Fader;
    [SerializeField] float imageMoveTime;
    [SerializeField] float moveDistance;
    Vector3 heroDefaultPosition;
    Vector3 slimeDefaultPosition;
    float fadeDuration = 0.75f;
    float originalMusicVol;
    int selectedMenu = 0;
    bool isStart = false;
    float selectionTimer = 0;
    const float selectionSpeed = 5;


    // panel
    public static IntroManager i { get; private set; }
    private void Awake()
    {
        i = this;
        // Screen.SetResolution(1280, 800, false);
    }
    private void Start()
    {
        Debug.Log($"객체 이름: {this.gameObject.transform.name}");
        try
        {
            heroDefaultPosition = new Vector3(heroObject.GetComponent<Image>().transform.position.x, heroObject.GetComponent<Image>().transform.position.y);
            slimeDefaultPosition = new Vector3(slimeObject.GetComponent<Image>().transform.position.x, slimeObject.GetComponent<Image>().transform.position.y);

        }
        catch (System.Exception)
        {
            Debug.Log($"객체 이름2: {this.gameObject.transform.name}");
        }
        StartCoroutine(IntroCutscene());

        AudioManager.i.PlayMusic(AudioId.Intro, fade: true);
    }

    void Update()
    {
        if (!isStart) return;
        HandleUpdate();
    }
    private void HandleUpdate()
    {
        UpdateSelectionTimer();
        // if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        //     selectedMenu++;
        // else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        //     selectedMenu--;
        HandleListSelection();

        selectedMenu = Mathf.Clamp(selectedMenu, 0, GlobalSettings.i.GameLevel == Difficulty.Normal ? 1 : 2);

        // UI 변경

        if (Input.GetButtonDown("Submit"))
        {
            if (selectedMenu == 0)
            {
                gameDescriptionObject.SetActive(true);
            }
            else
            {
                if (GlobalSettings.i.IsClear || selectedMenu == 1)
                {
                    GlobalSettings.i.GameLevel = Difficulty.Hell;
                    gameplayObject.SetActive(true);
                }
                else
                {
                    // 종료
                    Application.Quit();
                }
            }
            introObject.SetActive(false);
        }
    }
    protected void UpdateSelectionTimer()
    {
        if (selectionTimer > 0)
            selectionTimer = Mathf.Clamp(selectionTimer - Time.deltaTime, 0, selectionTimer);
    }
    void HandleListSelection()
    {
        // TODO: 메뉴에는 이게 맞는데, 파티창같이 좌우로도 움직일 수 있는건 다른것으로 고쳐야 함
        float v = Input.GetAxis("Vertical");
        if (selectionTimer == 0 && Mathf.Abs(v) > 0.2f)
        {
            selectedMenu += -(int)Mathf.Sign(v);
            selectionTimer = 1 / selectionSpeed;
        }
    }

    public IEnumerator IntroCutscene()
    {
        SavingSystem.i.Load("yonggi");

        Image hero = heroObject.GetComponent<Image>();
        Image slime = slimeObject.GetComponent<Image>();
        Image background = backgroundObject.GetComponent<Image>();
        yield return new WaitForSeconds(1f);
        while (slime.color.a < 1.0f)
        {
            float deltaTime = Time.deltaTime / imageMoveTime;
            yield return FadeInImage(slime, deltaTime);
            yield return MoveImage(slime, 1, "slime", deltaTime);
        }
        while (hero.color.a < 1.0f)
        {
            float deltaTime = Time.deltaTime / imageMoveTime;
            yield return FadeInImage(hero, deltaTime);
            yield return MoveImage(hero, 3, "hero", deltaTime);
        }
        yield return new WaitForSeconds(1f);
        AudioManager.i.PlaySfx(AudioId.Slice);
        Fader.color = new Color(1f, 1f, 1f, 1f);
        // back 2
        hero.color = new Color(1f, 1f, 1f, 0f);
        slime.color = new Color(1f, 1f, 1f, 0f);
        background.sprite = background2;
        yield return new WaitForSeconds(0.5f);
        Fader.color = new Color(1f, 1f, 1f, 0f);
        yield return new WaitForSeconds(0.5f);
        AudioManager.i.PlaySfx(AudioId.Slice);
        Fader.color = new Color(1f, 1f, 1f, 1f);
        // back 3
        background.sprite = background3;
        yield return new WaitForSeconds(0.5f);
        Fader.color = new Color(1f, 1f, 1f, 0f);
        yield return new WaitForSeconds(0.5f);
        AudioManager.i.PlaySfx(AudioId.Slice2);
        Fader.color = new Color(1f, 1f, 1f, 1f);
        // back 4
        backgroundObject.SetActive(false);
        pixelBackground.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Fader.color = new Color(1f, 1f, 1f, 0f);
        isStart = true;
        heroObject.SetActive(false);
        slimeObject.SetActive(false);
        backgroundObject.SetActive(false);
        yield return null;
    }

    public IEnumerator FadeInImage(Image image, float deltaTime)
    {
        image.color = new Color(1f, 1f, 1f, image.color.a + deltaTime);
        yield return null;
    }
    public IEnumerator MoveImage(Image image, int dir, string objectName, float deltaTime)
    // up 0, right 1, down 2, left 3
    {
        float x = 0;
        float y = 0;
        if (dir == 0)
            y = moveDistance * deltaTime;
        if (dir == 1)
            x = moveDistance * deltaTime;
        if (dir == 2)
            y = -moveDistance * deltaTime;
        if (dir == 3)
            x = -moveDistance * deltaTime;
        // image.transform.position += new Vector3(x, y);
        float defaultX = (objectName == "hero" ? heroDefaultPosition.x : slimeDefaultPosition.x);
        float defaultY = (objectName == "hero" ? heroDefaultPosition.y : slimeDefaultPosition.y);
        x = Mathf.Clamp(image.transform.position.x + x, defaultX - moveDistance, defaultX + moveDistance);
        y = Mathf.Clamp(image.transform.position.y + y, defaultY - moveDistance, defaultY + moveDistance);
        // Debug.Log($"name: {objectName}, x: {x}, y: {y}");
        image.transform.position = new Vector3(x, y);
        yield return null;
    }
}