using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
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

    // panel
    public static IntroManager i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    void Start()
    {
        heroDefaultPosition = new Vector3(heroObject.GetComponent<Image>().transform.position.x, heroObject.GetComponent<Image>().transform.position.y);
        slimeDefaultPosition = new Vector3(slimeObject.GetComponent<Image>().transform.position.x, slimeObject.GetComponent<Image>().transform.position.y);
        StartCoroutine(IntroCutscene());
    }

    void Update()
    {
    }

    public IEnumerator IntroCutscene()
    {
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
        Fader.color = new Color(1f, 1f, 1f, 1f);
        // back 2
        hero.color = new Color(1f, 1f, 1f, 0f);
        slime.color = new Color(1f, 1f, 1f, 0f);
        background.sprite = background2;
        yield return new WaitForSeconds(0.5f);
        Fader.color = new Color(1f, 1f, 1f, 0f);
        yield return new WaitForSeconds(0.5f);
        Fader.color = new Color(1f, 1f, 1f, 1f);
        // back 3
        background.sprite = background3;
        yield return new WaitForSeconds(0.5f);
        Fader.color = new Color(1f, 1f, 1f, 0f);
        yield return new WaitForSeconds(0.5f);
        Fader.color = new Color(1f, 1f, 1f, 1f);
        // back 4
        backgroundObject.SetActive(false);
        pixelBackground.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Fader.color = new Color(1f, 1f, 1f, 0f);
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
        Debug.Log($"name: {objectName}, x: {x}, y: {y}");
        image.transform.position = new Vector3(x, y);
        yield return null;
    }
}