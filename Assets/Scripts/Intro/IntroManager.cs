using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Image hero;
    [SerializeField] Image slime;
    [SerializeField] Image Fader;
    [SerializeField] float imageMoveTime = 1.5f;
    [SerializeField] float moveDistance = 150f;
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
        hero.color = new Color(1f, 1f, 1f, 0f);
        slime.color = new Color(1f, 1f, 1f, 0f);
        Fader.color = new Color(1f, 1f, 1f, 0f);
        heroDefaultPosition = new Vector3(hero.transform.position.x, hero.transform.position.y);
        slimeDefaultPosition = new Vector3(slime.transform.position.x, slime.transform.position.y);
        StartCoroutine(IntroCutscene());
    }

    void Update()
    {
    }

    public IEnumerator IntroCutscene()
    {
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
        yield return new WaitForSeconds(0.75f);
        Fader.color = new Color(1f, 1f, 1f, 0f);
        yield return new WaitForSeconds(0.75f);
        Fader.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.75f);
        Fader.color = new Color(1f, 1f, 1f, 0f);
        yield return new WaitForSeconds(0.75f);
        Fader.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.75f);
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
        x = Mathf.Clamp(image.transform.position.x + x, defaultX - 150f, defaultX + 150f);
        y = Mathf.Clamp(image.transform.position.y + y, defaultY - 150f, defaultY + 150f);
        Debug.Log($"name: {objectName}, x: {x}, y: {y}");
        image.transform.position = new Vector3(x, y);
        yield return null;
    }
}