using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    [Range(0f, 1f)]
    public float MasterVolume;
    public bool AimHelp;

    public Texture2D CursorTexture;

    public Animator ImageAnim;

    void Start()
    {
        Cursor.SetCursor(CursorTexture, new Vector2(CursorTexture.width / 2f, CursorTexture.height / 2f), CursorMode.Auto);

        Button btn = null;
        foreach (var button in FindObjectsOfType<Button>())
        {
            if (button.name.Equals("Exit"))
                btn = button;
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer && btn != null)
            btn.enabled = false;

        MasterVolume = StaticOptions.MasterVolume;
        AimHelp = StaticOptions.AimHelp;

        FindObjectOfType<Slider>().value = MasterVolume * 100f;
        FindObjectOfType<Toggle>().isOn = AimHelp;
    }

    public void OnClickStart()
    {
        StartCoroutine(StaticOptions.LoadLevel(1, ImageAnim));
    }


    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         StartCoroutine(ExitDelayed());
#endif
    }

    IEnumerator ExitDelayed()
    {
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    public void OnSliderChange(float sing)
    {
        MasterVolume = sing/100;
        StaticOptions.MasterVolume = MasterVolume;
    }

    public void OnAimChange(bool value)
    {
        AimHelp = value;
        StaticOptions.AimHelp = AimHelp;
    }
}
