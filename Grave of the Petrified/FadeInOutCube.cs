using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

/**
 * Author: Henri Leppänen
 * <summary>
 * Workaround to blitshader for not working in VR. Fades player vision to black and back.
 * </summary>
 */

public class FadeInOutCube : MonoBehaviour
{
	private bool fading = false;
	private float fadeTimer = 0f;
	private Color fromColor;
	private Color toColor;
	private Material fadeMaterial;
    public Material blitMaterial;

	private UnityAction currentOnFinishFade;

	private float fadeTime = 1f;

	private void Awake()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		ReverseNormals(meshFilter.mesh);
		fadeMaterial = GetComponent<MeshRenderer>().material;

		fromColor = fadeMaterial.color;
		toColor = fadeMaterial.color;

	}

    private void Update()
    {
        if (fading)
        {
            if (fadeTimer >= fadeTime)
            {
                fading = false;
                fadeTimer = 0f;
                if (currentOnFinishFade != null)
                {
                    currentOnFinishFade.Invoke();
                    currentOnFinishFade = null;
                }
            }
            else
            {
                fadeTimer += Time.deltaTime;

                fadeMaterial.color = Color.Lerp(fromColor, toColor, fadeTimer / fadeTime);
            }
        }
    }

    /// <summary>
    /// Starts fade out (screen to black)
    /// </summary>
    /// <param name="fadeDuration">How long does the fade take</param>
    /// <param name="onFinishFade">Method to execute when fade is finished</param>
    public void FadeOut(float fadeDuration, UnityAction onFinishFade)
	{
		if (fadeDuration == 0f)
		{
			Debug.LogWarning("Fade time shouldn't be 0f, use InstantFadeOut instead");
			InstantFadeOut();
			return;
		}
		fadeTime = fadeDuration;
		fromColor.a = 0f;
		toColor.a = 1f;
		fading = true;
		currentOnFinishFade = onFinishFade;
	}

	/// <summary>
	/// Starts fade in (black to screen)
	/// </summary>
	/// <param name="fadeDuration">How long does the fade take</param>
	/// <param name="onFinishFade">Method to execute when fade is finished</param>
	public void FadeIn(float fadeDuration, UnityAction onFinishFade)
	{
		if (fadeDuration == 0f)
		{
			Debug.LogWarning("Fade time shouldn't be 0f, use InstantFadeIn instead");
			InstantFadeIn();
			return;
		}
		fadeTime = fadeDuration;
        
		fromColor.a = 1f;
		toColor.a = 0f;
		fading = true;
		currentOnFinishFade = onFinishFade;
	}

	/// <summary>
	/// Instantly switches off fade overlay
	/// </summary>
	public void InstantFadeIn()
	{
		toColor.a = 0f;
		fadeMaterial.color = toColor;
		fading = false;
		fadeTimer = 0f;
	}

	/// <summary>
	/// Instantly switches on fade overlay
	/// </summary>
	public void InstantFadeOut()
	{
		toColor.a = 1f;
		fadeMaterial.color = toColor;
		fading = false;
		fadeTimer = 0f;
	}

	/// <summary>
	/// Reverses normals and flips triangles so the mesh is rendered on the inside
	/// </summary>
	/// <param name="gameObject"></param>
	void ReverseNormals(Mesh mesh)
	{
		if (mesh == null)
		{
			Debug.Log("Null mesh");
			return;
		}

		// Flip normals
		Vector3[] normals = mesh.normals;
		for (int i = 0; i < normals.Length; i++)
		{
			normals[i] = -normals[i];
		}
		mesh.normals = normals;

		for (int m = 0; m < mesh.subMeshCount; m++)
		{
			// Flip triangles
			int[] triangles = mesh.GetTriangles(m);
			for (int i = 0; i < triangles.Length; i += 3)
			{
				int temp = triangles[i + 0];
				triangles[i + 0] = triangles[i + 1];
				triangles[i + 1] = temp;
			}
			mesh.SetTriangles(triangles, m);
		}
	}
}
