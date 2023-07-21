using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDescriptionManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject gameDescriptionObject;
    [SerializeField] GameObject gameplayObject;
    [SerializeField] Image descriptionObject;
    [SerializeField] List<Sprite> images;
    [SerializeField] Sprite endImage;
    void Start()
    {
        StartCoroutine(GameDescription());
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator GameDescription()
    {
        foreach (Sprite img in images)
        {
            descriptionObject.sprite = img;
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
        }
        descriptionObject.sprite = endImage;
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetButtonDown("Submit"));

        gameplayObject.SetActive(true);
        gameDescriptionObject.SetActive(false);
    }
}