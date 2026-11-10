

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    bool acceptInput;

    void Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(EnableInputAfterRelease());
    }

    IEnumerator EnableInputAfterRelease()
    {
        yield return null;
        while (Input.anyKeyDown)
        {
            yield return null;
        }

        acceptInput = true;
    }

    void Update()
    {
        if (!acceptInput)
        {
            return;
        }

        if (Input.anyKey)
        {
            SceneManager.LoadScene(GameScenes.Game);
        }
    }

}
