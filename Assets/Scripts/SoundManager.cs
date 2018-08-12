using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
	AudioSource aus;

	public AudioClip[] paperSounds;
	public AudioClip[] ritchSounds;
	public AudioClip[] bumpSounds;

	public enum Soundtypes
	{
		Paper,
		Ritch,
		Bump
	}

	void Start()
	{
		aus = GetComponent<AudioSource>();
	}

	public void Play(Soundtypes st)
	{
		aus.pitch = Random.Range(0.8f, 1.25f);
		switch (st)
		{
			case Soundtypes.Paper:
				aus.PlayOneShot(paperSounds[Random.Range(0, paperSounds.Length)]);
				break;
			case Soundtypes.Ritch:
				aus.pitch = aus.pitch * 0.9f;
				aus.PlayOneShot(ritchSounds[Random.Range(0, ritchSounds.Length)]);
				break;
			case Soundtypes.Bump:
				aus.PlayOneShot(bumpSounds[Random.Range(0, bumpSounds.Length)]);
				break;
		}
	}
}
