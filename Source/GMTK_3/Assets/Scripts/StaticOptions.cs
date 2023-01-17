using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticOptions
{
    public static bool AimHelp { get; set; } = true;
    public static float MasterVolume { get; set; } = 1f;



    public static IEnumerator LoadLevel(int levelIndex, Animator animator)
    {
        animator.SetTrigger("Start");

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(levelIndex);
    }

    public static IEnumerator FastLoadLevel(int levelIndex)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(levelIndex);
    }
}
