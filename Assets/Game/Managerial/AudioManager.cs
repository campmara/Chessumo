using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour 
{
	public static AudioManager Instance = null;

	public bool SoundEnabled = true;

	[Header("Sources")]
	[SerializeField] AudioSource pianoA;
	[SerializeField] AudioSource pianoB;
	[SerializeField] AudioSource synth;
	[SerializeField] AudioSource drums;

	[Header("Clips")]
	[SerializeField] AudioClip p_start_suspense_1;
	[SerializeField] AudioClip p_start_suspense_2;
	[SerializeField] AudioClip p_start_release;
	[SerializeField] AudioClip p_start_piecespawn;
	[SerializeField] AudioClip p_full_combo;
	[SerializeField] AudioClip p_game_over;
	
	[SerializeField] AudioClip s_npv_enter;
	[SerializeField] AudioClip s_npv_fade;

	[SerializeField] AudioClip d_piece_move;
	[SerializeField] AudioClip d_piece_pickup;
	[SerializeField] AudioClip d_ui_blip;
	[SerializeField] AudioClip d_move_hit;

	[SerializeField] AudioClip p_off_grid;
	[SerializeField] AudioClip[] p_moveNotes;
	[SerializeField] AudioClip[] p_scores;

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

	bool isSuspenseHigh = false;
	public void PlaySuspense()
	{
		if (!SoundEnabled) return;

		// reset pitch
		pianoA.pitch = 1;
		pianoB.pitch = 1;

		if (isSuspenseHigh)
		{
			pianoB.clip = p_start_suspense_2;
			pianoB.Play();
		}
		else
		{
			pianoA.clip = p_start_suspense_1;
			pianoA.Play();
		}

		isSuspenseHigh = !isSuspenseHigh;
	}

	public float ReverseSuspense() // returns the time until full reverse.
	{
		if (!SoundEnabled) return 0f;

		if (isSuspenseHigh)
		{
			// make piano a rev
			pianoA.pitch = -1;
			return pianoA.time;
		}
		else
		{
			pianoB.pitch = -1;
			return pianoB.time;
		}
	}

	public void PlayStartRelease()
	{
		if (!SoundEnabled) return;

		pianoA.pitch = 1;
		pianoB.pitch = 1;

		Play(pianoA, p_start_release);

		isSuspenseHigh = false;
	}

	public void PlayStartPiecesSpawn()
	{
		if (!SoundEnabled) return;
		Play(pianoB, p_start_piecespawn);
	}

	public void PlayGameOver()
	{
		if (!SoundEnabled) return;
		Play(pianoB, p_game_over);
	}

	public void PlayNPVEnter()
	{
		if (!SoundEnabled) return;
		Play(synth, s_npv_enter);
	}

	public void PlayNPVFade()
	{
		if (!SoundEnabled) return;
		Play(synth, s_npv_fade);
	}

	public void PlayPiecePickup()
	{
		if (!SoundEnabled) return;
		drums.pitch = Random.Range(0.95f, 1.05f);
		drums.PlayOneShot(d_piece_pickup);
	}

	public void PlayPieceDrawMove()
	{
		if (!SoundEnabled) return;
		drums.pitch = Random.Range(0.95f, 1.05f);
		drums.PlayOneShot(d_piece_move);
	}

	public void PlayMoveHit()
	{
		if (!SoundEnabled) return;
		drums.pitch = Random.Range(0.95f, 1.05f);
		drums.PlayOneShot(d_move_hit);
	}

	/*
	int currMoveNote = 0;
	public void PlayMoveNote()
	{
		if (currMoveNote >= p_moveNotes.Length) currMoveNote = 0;

		synth.PlayOneShot(p_moveNotes[currMoveNote]);

		currMoveNote++;
	}
	*/

	public void PlayScoreOneNote()
	{
		if (!SoundEnabled) return;
		pianoB.PlayOneShot(p_scores[0]);
	}
	public void PlayScoreTwoNote()
	{
		if (!SoundEnabled) return;
		pianoB.PlayOneShot(p_scores[1]);
	}
	public void PlayScoreThreeNote()
	{
		if (!SoundEnabled) return;
		pianoB.PlayOneShot(p_full_combo);
		pianoB.PlayOneShot(p_scores[2]);
	}

	public void PlayPieceOffGrid()
	{
		if (!SoundEnabled) return;
		drums.pitch = Random.Range(0.9f, 1.1f);
		drums.PlayOneShot(p_off_grid);
	}

	public void PlayUIBlip()
	{
		if (!SoundEnabled) return;
		drums.pitch = Random.Range(0.9f, 1.1f);
		drums.PlayOneShot(d_ui_blip);
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
