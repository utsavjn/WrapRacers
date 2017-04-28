using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Game.Util;
using GameData;

public class SoundManager
{
	#region Enum and constant

	public enum PlayMusicMode
	{
		Single,
		Repeat,
		Order,
		RepeatOrder,
		Random
	}

	public static string defaultSoundSourceName = "SoundSource";
	public static string defaultMusicSourceName = "MusicSource";
	public static uint defaultBackgroundInterval = 4000;

	#endregion

	#region variables

	protected static AudioListener listener;

	protected static AudioSource defaultSoundSource;
	protected static AudioSource defaultMusicSource;

	public static Dictionary<int, AudioClip> audioClipBuffer = new Dictionary<int, AudioClip>();

	protected static uint nextPlayMusicTimer;

	protected static PlayMusicMode musicMode;
	protected static List<int> backgroundMusicOrder = new List<int>();
	protected static int orderIndex;
	protected static int curMusic = -1;

	#endregion

	#region initialize

	static SoundManager()
	{
		
	}

	public static void Init()
	{
		defaultSoundSource = GameManager._instance.m_soundAud;
		defaultMusicSource = GameManager._instance.m_musicAud;

		audioClipBuffer = new Dictionary<int, AudioClip>();

		musicMode = PlayMusicMode.Repeat;
		backgroundMusicOrder = new List<int>();

		orderIndex = 0;
	}

	#endregion

	#region download and remove sound

	public static void LoadAudioClip(int soundID, Action<UnityEngine.Object> action = null)
	{
		if (audioClipBuffer.ContainsKey(soundID))
		{
			if (action != null)
				action(audioClipBuffer[soundID]);
			return;
		}

		if (!SoundData.dataMap.ContainsKey(soundID))
			return;
		
		AssetCacheMgr.GetResourceAutoRelease(SoundData.dataMap[soundID].stringfullpath, (obj) =>
			{
				UnityEngine.Object.DontDestroyOnLoad(obj);
				if (action != null)
					action(obj);

				if (obj is AudioClip && !audioClipBuffer.ContainsKey(soundID))
					audioClipBuffer.Add(soundID, obj as AudioClip);
			});
	}

	public static void LoadAudioClip(int soundID, AudioSource source, bool isLoop, Action<AudioSource, UnityEngine.Object, bool> action = null)
	{
		if (audioClipBuffer.ContainsKey(soundID) && action != null)
			action(source, audioClipBuffer[soundID], isLoop);
		
		if (!SoundData.dataMap.ContainsKey(soundID))
			return;
		
		AssetCacheMgr.GetResourceAutoRelease(SoundData.dataMap[soundID].stringfullpath, (obj) =>
			{
				UnityEngine.Object.DontDestroyOnLoad(obj);
				if (action != null)
					action(source, obj, isLoop);
			});
	}

	public static void SetAudioClip(UnityEngine.Object obj, MonoBehaviour script, Action action = null, params string[] fieldNames)
	{
		if (fieldNames.Length == 0)
		{
			SetAudioClipValue(obj, script, script.GetType().GetFields());
			action();
		}
		else
		{
			List<FieldInfo> fields = new List<FieldInfo>();
			foreach (var fieldName in fieldNames)
			{
				var field = script.GetType().GetField(fieldName);
				if (field != null)
					fields.Add(field);
			}
			if (fields.Count > 0)
				SetAudioClipValue(obj, script, fields.ToArray());
			action();
		}
	}

	private static void SetAudioClipValue(object obj, MonoBehaviour script, FieldInfo[] fields)
	{
		foreach (var field in fields)
		{
			field.SetValue(script, obj);
		}
	}

	public static void UnloadAllAudioClip()
	{
		foreach (var item in audioClipBuffer)
		{
			AssetCacheMgr.ReleaseResource(item.Value);
		}

		audioClipBuffer.Clear();
	}

	#endregion

	#region play audio

	#region play all audio

	static public AudioSource MyPlaySound(AudioSource defaultSource, string sourceName, AudioClip clip) { return MyPlaySound(defaultSource, sourceName, clip, 1f, 1f); }

	public static AudioSource MyPlaySound(AudioSource defaultSource, string sourceName, AudioClip clip, float volume) { return MyPlaySound(defaultSource, sourceName, clip, volume, 1f); }

	public static AudioSource MyPlaySound(AudioSource defaultSource, string sourceName, AudioClip clip, float volume, float pitch)
	{
		if (clip != null)
		{			
			AudioSource source = defaultSource;
			if (source == null)
			{
				defaultSource = GameManager._instance.transform.FindChild(sourceName).gameObject.AddComponent<AudioSource>();
				source = defaultSource;
			}

			source.volume = volume;
			source.pitch = pitch;
			source.PlayOneShot(clip);
			return source;
		}
		return null;
	}

	#endregion

	#region play game background sound

	public static void GameObjectPlaySound(GameObject go, int soundID, bool isLoop = false)
	{
		AudioSource gameObjectAudioSource = go.GetComponent<AudioSource>();
		if (gameObjectAudioSource == null)
			gameObjectAudioSource = go.AddComponent<AudioSource>();
		else if (gameObjectAudioSource.isPlaying)
			gameObjectAudioSource.Stop();

		PlaySoundOnSourceByID(soundID, gameObjectAudioSource, isLoop);
	}

	public static void PlaySoundOnSourceByID(int soundID, AudioSource gameObjectAudioSource, bool isLoop = false)
	{
		LoadAudioClip(soundID, gameObjectAudioSource, isLoop, PlaySoundOnSourceByObject);
	}

	public static void PlaySoundOnSourceByObject(AudioSource gameObjectAudioSource, UnityEngine.Object clipObject, bool isLoop = false)
	{
		if (clipObject is AudioClip)
		{
			gameObjectAudioSource.clip = clipObject as AudioClip;
			gameObjectAudioSource.loop = isLoop;
			gameObjectAudioSource.Play();
			return;
		}

		var clip = (clipObject as GameObject).GetComponent<AudioSource>().clip;
		if (clip != null)
		{
			gameObjectAudioSource.clip = clip;
			gameObjectAudioSource.loop = isLoop;
			gameObjectAudioSource.Play();
		}
	}

	public static void StopGameObjectPlaySound(GameObject go)
	{
		AudioSource source = go.GetComponent<AudioSource>();
		if (source == null)
			return;
		if (source.isPlaying)
			source.Stop();
	}

	#endregion

	#region UI sound

	public static void PlaySoundByID(int soundID)
	{
		LoadAudioClip(soundID, PlaySoundByObject);
	}

	public static void PlaySoundByObject(UnityEngine.Object clipObject)
	{
		if(clipObject==null)
			return ;
		
		if (clipObject is AudioClip)
		{
			defaultSoundSource = MyPlaySound(defaultSoundSource, defaultSoundSourceName, clipObject as AudioClip);
			return;
		}

		var clip = (clipObject as GameObject).GetComponent<AudioSource>().clip;
		if (clip != null)
			defaultSoundSource = MyPlaySound(defaultSoundSource, defaultSoundSourceName, clip);
	}

	#endregion

	#region back sound
	public static void PlayMusic(int soundID, PlayMusicMode mode = PlayMusicMode.Repeat)
	{
		if (curMusic == soundID && musicMode == mode && defaultMusicSource.isPlaying)
			return;

		curMusic = soundID;
		musicMode = mode;
		LoadAudioClip(soundID, PlayMusicByObject);
	}

	public static void PlayMusicByObject(UnityEngine.Object clipObject)
	{
		if (clipObject is AudioClip)
		{
			defaultMusicSource = MyPlaySound(defaultMusicSource, defaultMusicSourceName, clipObject as AudioClip);
			PrepareForNextPlay((uint)(
				(int)((clipObject as AudioClip).length * 1000) + defaultBackgroundInterval
			));
			return;
		}

		var clip = (clipObject as GameObject).GetComponent<AudioSource>().clip;
		if (clip != null)
		{
			defaultMusicSource = MyPlaySound(defaultMusicSource, defaultMusicSourceName, clip);
			PrepareForNextPlay((uint)(
				(int)(clip.length * 1000) + defaultBackgroundInterval
			));
		}
	}

	protected static void PrepareForNextPlay(uint time)
	{
		nextPlayMusicTimer = TimerHeap.AddTimer(time, 0, SetNextPlay);
	}

	protected static void SetNextPlay()
	{
		switch (musicMode)
		{
		case PlayMusicMode.Single:
			return;

		case PlayMusicMode.Repeat:
			PlayMusic(curMusic/*backgroundMusicOrder[orderIndex]*/);
			return;

		case PlayMusicMode.Order:
			if (orderIndex + 1 >= backgroundMusicOrder.Count)
				orderIndex = 0;
			else
				PlayMusic(backgroundMusicOrder[orderIndex + 1]);
			
			return;

		case PlayMusicMode.RepeatOrder:
			PlayMusic(backgroundMusicOrder[orderIndex + 1 >= backgroundMusicOrder.Count ? 0 : orderIndex + 1]);
			return;

		case PlayMusicMode.Random:
			orderIndex = UnityEngine.Random.Range(0, backgroundMusicOrder.Count);
			PlayMusic(backgroundMusicOrder[orderIndex]);
			return;
		}
	}

	public static void StopBackgroundMusic()
	{
		if (defaultMusicSource != null)
			defaultMusicSource.Stop();
		
		TimerHeap.DelTimer(nextPlayMusicTimer);
	}

	public static void ChangeMusic(int soundID, PlayMusicMode mode = PlayMusicMode.Repeat)
	{
		StopBackgroundMusic();
		PlayMusic(soundID, mode);
	}

	#endregion

	#region Logical sound

	public static void LogicPlaySoundByID(int soundID, AudioSource source)
	{
		PlaySoundOnSourceByID(soundID, source);
	}

	public static void LogicPlaySoundByClip(AudioSource source, AudioClip clip)
	{
		PlaySoundOnSourceByObject(source, clip);
	}

	#endregion

	#endregion
}