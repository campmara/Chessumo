using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour 
{
	public static AudioManager Instance = null;

	[Header("Sources")]
	[SerializeField] AudioSource blipSource;
	[SerializeField] AudioSource chordSource;

	[Header("Clips")]
	[SerializeField] AudioClip blip;
	[SerializeField] AudioClip chord1;
	[SerializeField] AudioClip chord2;
	[SerializeField] AudioClip chord3;
	[SerializeField] AudioClip chord4;

	[Header("Curves")]
	[SerializeField] AnimationCurve playCurve;
	[SerializeField] AnimationCurve quietCurve;
	[SerializeField] float curveTime = 0.05f;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void PlayBlip(float pitch)
	{
		blipSource.pitch = pitch;
		Play(blipSource, blip);
	}

	public void PlayChordOne()
	{
		Play(chordSource, chord1);
	}

	public void PlayChordTwo()
	{
		Play(chordSource, chord2);
	}

	public void PlayChordThree()
	{
		Play(chordSource, chord3);
	}

	public void PlayChordFour()
	{
		Play(chordSource, chord4);
	}

	private void Play(AudioSource source, AudioClip clip)
	{
		StartCoroutine(PlayRoutine(source, clip));
	}

	private IEnumerator PlayRoutine(AudioSource source, AudioClip clip)
	{
		if (source.isPlaying)
		{
			yield return StartCoroutine(QuietRoutine(source));
		}

		source.volume = 0f;

		// Load and play clip.
		source.clip = clip;
		source.Play();

		// bring up the volume quickly.
		float timer = 0f;
		while (timer < curveTime)
		{
			timer += Time.deltaTime;
			source.volume = playCurve.Evaluate(timer / curveTime);
			yield return null;
		}
	}

	private IEnumerator QuietRoutine(AudioSource source)
	{
		float timer = 0f;
		while (timer < curveTime)
		{
			timer += Time.deltaTime;
			source.volume = quietCurve.Evaluate(timer / curveTime);
			yield return null;
		}
	}
}
