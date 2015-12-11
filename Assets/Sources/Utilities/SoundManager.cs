using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum SoundType
{
	BGM,
	SE,
}

public class SoundManager : MonoBehaviour
{
	static public SoundManager Instance = null;

	const string BGMPath = "";
	const string SEPath = "";

	const string BGMTypeUnsupportedMessage = "SoundType BGM is unsupported";

	private AudioSource bgmSource = null;
	private Dictionary<SoundType, List<PlayingAudio>> sourceListDictionary = new Dictionary<SoundType, List<PlayingAudio>> ();

	float bgmVolume = 1.0f;

	public float BgmVolume {
		get{ return bgmVolume; }
		set {
			value = Mathf.Clamp (value, 0, 1);
			bgmVolume = value;
			if (bgmSource != null) {
				bgmSource.volume = bgmVolume;
				bgmSource.mute = BgmMute;
			}
		}
	}

	float seVolume = 1.0f;

	public float SeVolume {
		get{ return seVolume; }
		set {
			value = Mathf.Clamp (value, 0, 1);
			seVolume = value;
		}
	}

	float voiceVolume = 1.0f;

	public float VoiceVolume {
		get { return voiceVolume; }
		set {
			value = Mathf.Clamp (value, 0, 1);
			voiceVolume = value;
		}
	}

	public bool BgmMute {
		get {
			return Math.Abs (bgmVolume) < Mathf.Epsilon;
		}
		set {
			bgmVolume = value ? 0.0f : bgmVolume;
			if (bgmSource != null)
				bgmSource.mute = value;
		}
	}

	public bool SeMute {
		get {
			return Math.Abs (seVolume) < Mathf.Epsilon;
		}
		set {
			seVolume = value ? 0.0f : seVolume;
		}
	}

	public AnimationCurve fadeCurve = AnimationCurve.EaseInOut (0, 0, 1, 1);

	void Awake ()
	{
		Instance = this;

		sourceListDictionary.Add (SoundType.SE, new List<PlayingAudio> ());
	}

	void LateUpdate ()
	{
		var seSourceList = sourceListDictionary [SoundType.SE];
		CleanSourceList (ref seSourceList);
	}

	void CleanSourceList (ref List<PlayingAudio> sourceList)
	{
		// removeして要素数が変わるのでこの順で実行させる必要がある
		for (int i = sourceList.Count - 1; i >= 0; --i) {
			var playing = sourceList [i];
			playing.Process ();
			if (playing.IsFinished ()) {
				if (fadings.ContainsKey (playing.audioSource)) {
					fadings.Remove (playing.audioSource);
				}
				playing.Finish ();
				sourceList.RemoveAt (i);
			}
		}

	}

	public void MuteAll (bool mute)
	{
		AudioListener.volume = (mute) ? 0 : 1;
	}

	public float GetVolume (SoundType soundType)
	{
		switch (soundType) {
		case SoundType.BGM:
			return BgmVolume;
		case SoundType.SE:
			return SeVolume;
		default:
			Debug.LogError ("unexpected sound type {0}".Fmt (soundType.ToString ()));
			return 0;
		}
	}

	public void SetVolume (SoundType soundType, float volume)
	{
		switch (soundType) {
		case SoundType.BGM:
			BgmVolume = volume;
			break;
		case SoundType.SE:
			SeVolume = volume;
			break;
		default:
			Debug.LogError ("unexpected sound type {0}".Fmt (soundType.ToString ()));
			break;
		}
	}

	public bool GetMute (SoundType soundType)
	{
		switch (soundType) {
		case SoundType.BGM:
			return BgmMute;
		case SoundType.SE:
			return SeMute;
		default:
			Debug.LogError ("unexpected sound type {0}".Fmt (soundType.ToString ()));
			return false;
		}
	}

	public void SetMute (SoundType soundType, bool mute)
	{
		switch (soundType) {
		case SoundType.BGM:
			BgmMute = mute;
			break;
		case SoundType.SE:
			SeMute = mute;
			break;
		default:
			Debug.LogError ("unexpected sound type {0}".Fmt (soundType.ToString ()));
			break;
		}
	}

	#region only BGM

	void PlayBGM (AudioClip clip, float time)
	{
		if (bgmSource == null) {
			bgmSource = gameObject.AddComponent<AudioSource> ();
		}
		if (bgmSource.clip == clip) {
			return;
		}

		bgmSource.Stop ();
		bgmSource.clip = clip;
		bgmSource.loop = true;
		bgmSource.volume = bgmVolume;
		bgmSource.mute = BgmMute;
		bgmSource.time = time;
		bgmSource.Play ();
	}

	public void FadeInBGM (float duration, Action onFinish = null)
	{
		if (bgmSource == null) {
			if (onFinish != null)
				onFinish ();
			return;
		}

		bgmSource.volume = 0;
		FadeVolume (bgmSource, bgmVolume, duration, onFinish);
	}

	public void FadeOutBGM (float duration, Action onFinish = null)
	{
		if (bgmSource == null) {
			if (onFinish != null)
				onFinish ();
			return;
		}

		FadeVolume (bgmSource, 0, duration, onFinish);
	}

	#endregion

	#region public play functions

	public void Play (SoundType soundType, string filePath, Action onFinish = null)
	{
		Play (soundType, LoadAudioClip (filePath), onFinish);
	}

	public void Play (SoundType soundType, AudioClip clip, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			PlayBGM (clip, 0);
		} else {
			var sourceList = sourceListDictionary [soundType];
			PlayOneShot (clip, GetVolume (soundType), GetMute (soundType), null, onFinish, ref sourceList);
		}
	}

	public void Play (SoundType soundType, AudioSource source, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		var sourceList = sourceListDictionary [soundType];
		PlayExternalSource (source, GetVolume (soundType), GetMute (soundType), null, onFinish, ref sourceList);
	}

	public void Play (SoundType soundType, AudioSource source, string filePath, Action onFinish = null)
	{
		Play (soundType, source, LoadAudioClip (filePath), onFinish);
	}

	public void Play (SoundType soundType, AudioSource source, AudioClip clip, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		var sourceList = sourceListDictionary [soundType];
		PlayWithExternalSource (source, clip, GetVolume (soundType), GetMute (soundType), null, onFinish, ref sourceList);
	}

	public void PlayAsync (SoundType soundType, string filePath, Action onFinish = null)
	{
		LoadAudioClipAsync (filePath, clip => {
			Play (soundType, clip, onFinish);
		});
	}

	public void PlayAsync (SoundType soundType, AudioSource source, string filePath, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		LoadAudioClipAsync (filePath, clip => {
			if (source != null)
				Play (soundType, source, clip, onFinish);
		});
	}

	public void PlayLoop (SoundType soundType, string filePath)
	{
		PlayLoop (soundType, LoadAudioClip (filePath));
	}

	public void PlayLoop (SoundType soundType, AudioClip clip)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}

		var sourceList = sourceListDictionary [soundType];
		PlayOneShot (clip, GetVolume (soundType), GetMute (soundType), null, null, ref sourceList).loop = true;
	}

	public void PlayLoop (SoundType soundType, AudioSource source)
	{
		PlayLoop (soundType, source, source.clip);
	}

	public void PlayLoop (SoundType soundType, AudioSource source, string filePath)
	{
		PlayLoop (soundType, source, LoadAudioClip (filePath));
	}

	public void PlayLoop (SoundType soundType, AudioSource source, AudioClip clip)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		source.loop = true;

		var sourceList = sourceListDictionary [soundType];
		PlayWithExternalSource (source, clip, GetVolume (soundType), GetMute (soundType), null, null, ref sourceList);
	}

	public void PlayLoopAsync (SoundType soundType, string filePath)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		LoadAudioClipAsync (filePath, clip => {
			PlayLoop (soundType, clip);
		});
	}

	public void PlayLoopAsync (SoundType soundType, AudioSource source, string filePath)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		LoadAudioClipAsync (filePath, clip => {
			if (source != null)
				PlayLoop (soundType, source, clip);
		});
	}

	public void Stop ()
	{
		Stop (SoundType.BGM);
		Stop (SoundType.SE);
	}

	public void Stop (SoundType soundType)
	{
		if (soundType == SoundType.BGM) {
			if (bgmSource != null) {
				bgmSource.Stop ();
			}
			return;
		}
		Stop (sourceListDictionary [soundType]);
	}

	private void Stop (List<PlayingAudio> sourceList)
	{
		// removeして要素数が変わるのでこの順で実行させる必要がある
		for (int i = sourceList.Count - 1; i >= 0; --i) {
			var playing = sourceList [i];
			playing.Stop ();
			sourceList.Remove (playing);
		}
		sourceList.Clear ();
	}

	public void Stop (SoundType soundType, string fileName)
	{
		if (soundType == SoundType.BGM) {
			if (bgmSource != null && bgmSource.clip.name == fileName) {
				bgmSource.Stop ();
			}
			return;
		}

		Stop (fileName, sourceListDictionary [soundType]);
	}

	private void Stop (string fileName, List<PlayingAudio> sourceList)
	{
		// removeして要素数が変わるのでこの順で実行させる必要がある
		for (int i = sourceList.Count - 1; i >= 0; --i) {
			var playing = sourceList [i];
			if (playing.audioSource.clip.name == fileName) {
				playing.Stop ();
				sourceList.Remove (playing);
			}
		}
	}

	public void Stop (SoundType soundType, AudioClip clip)
	{
		if (soundType == SoundType.BGM) {
			if (bgmSource != null && bgmSource.clip == clip) {
				bgmSource.Stop ();
			}
			return;
		}

		Stop (clip, sourceListDictionary [soundType]);
	}

	private void Stop (AudioClip clip, List<PlayingAudio> sourceList)
	{
		// removeして要素数が変わるのでこの順で実行させる必要がある
		for (int i = sourceList.Count - 1; i >= 0; --i) {
			var playing = sourceList [i];
			if (playing.audioSource.clip == clip) {
				playing.Stop ();
				sourceList.Remove (playing);
			}
		}
	}

	public void Stop (SoundType soundType, AudioSource source)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		Stop (source, sourceListDictionary [soundType]);
	}

	private void Stop (AudioSource source, List<PlayingAudio> sourceList)
	{
		// removeして要素数が変わるのでこの順で実行させる必要がある
		for (int i = sourceList.Count - 1; i >= 0; --i) {
			var playing = sourceList [i];
			if (playing.audioSource == source) {
				playing.Stop ();
				sourceList.Remove (playing);
			}
		}
	}

	public AudioSource Play3D (SoundType soundType, Transform parent, string filePath, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return null;
		}
		return Play3D (soundType, parent, LoadAudioClip (filePath), onFinish);
	}

	public AudioSource Play3D (SoundType soundType, Transform parent, AudioClip clip, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return null;
		}
		var sourceList = sourceListDictionary [soundType];
		return Play3D (
			null, //AudioSouceは自動生成に任せるのでnullを渡す
			parent,
			clip,
			GetVolume (soundType),
			GetMute (soundType),
			null,
			onFinish,
			ref sourceList);
	}

	public void Play3D (SoundType soundType, AudioSource source, string filePath, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		Play3D (soundType, source, LoadAudioClip (filePath), onFinish);
	}

	public void Play3D (SoundType soundType, AudioSource source, AudioClip clip, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		var sourceList = sourceListDictionary [soundType];
		Play3D (
			source,
			source.transform.parent,
			clip,
			GetVolume (soundType),
			GetMute (soundType),
			null,
			onFinish,
			ref sourceList);
	}

	public void Play3D (SoundType soundType, AudioSource source, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		Play3D (soundType, source, source.clip, onFinish);
	}

	public void Play3DAsync (SoundType soundType, Transform parent, string filePath, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		LoadClipAsync (filePath, clip => {
			Play3D (soundType, parent, clip, onFinish);
		});
	}

	public void Play3DAsync (SoundType soundType, AudioSource source, string filePath, Action onFinish = null)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		LoadClipAsync (filePath, clip => {
			Play3D (soundType, source, clip, onFinish);
		});
	}

	public AudioSource Play3DLoop (SoundType soundType, Transform parent, string filePath)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return null;
		}
		var sourceList = sourceListDictionary [soundType];
		return Play3DLoop (parent, LoadAudioClip (filePath), GetVolume (soundType), GetMute (soundType), ref sourceList);
	}

	public AudioSource Play3DLoop (SoundType soundType, Transform parent, AudioClip clip)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return null;
		}
		var sourceList = sourceListDictionary [soundType];
		return Play3DLoop (parent, clip, GetVolume (soundType), GetMute (soundType), ref sourceList);
	}

	public void Play3DLoopAsync (SoundType soundType, Transform parent, string filePath)
	{
		if (soundType == SoundType.BGM) {
			Debug.LogError (BGMTypeUnsupportedMessage);
			return;
		}
		LoadClipAsync (filePath, clip => {
			Play3DLoop (soundType, parent, clip);
		});
	}

	#endregion

	private AudioSource CreateAudioSourceObject (string objectName)
	{
		return (new GameObject (objectName)).AddComponent<AudioSource> ();
	}

	public class PlayingAudio
	{
		internal AudioSource audioSource { get; private set; }

		private Action<float, float> onProcess = null;
		private Action onFinish = null;
		private bool isPausing = false;
		private bool isExternalAudioSource = false;

		public PlayingAudio (AudioSource source, AudioClip clip, float volume, bool mute, bool isExternalAudioSource, Action<float, float> onProcess, Action onFinish)
		{
			this.audioSource = source;
			this.audioSource.playOnAwake = false;

			this.audioSource.clip = clip;
			this.audioSource.volume = volume;
			this.audioSource.mute = mute;
			this.isExternalAudioSource = isExternalAudioSource;

			this.onProcess = onProcess;
			this.onFinish = onFinish;

			isPausing = false;
		}

		public void Play ()
		{
			audioSource.Play ();
			isPausing = false;
		}

		public void Pause ()
		{
			audioSource.Pause ();
			isPausing = true;
		}

		public void Process ()
		{
			if (onProcess == null)
				return;

			var time = audioSource.time;
			var length = audioSource.clip.length;
			onProcess (time, length);
		}

		public void Stop ()
		{
			audioSource.Stop ();
			isPausing = false;
		}

		public void ChangeVolume (float volume)
		{
			audioSource.volume = volume;
		}

		public void Mute (bool mute)
		{
			audioSource.mute = mute;
		}

		public bool IsFinished ()
		{
			return !audioSource.isPlaying && !isPausing;
		}

		public void Finish ()
		{
			if (onFinish != null)
				onFinish ();
			if (!isExternalAudioSource) {
				Destroy (audioSource);
				audioSource = null;
			}
		}
	}

	/// <summary>
	/// 重ねずに再生する。
	/// </summary>
	private void Play (AudioClip clip, float volume, bool mute, Action<float, float> onProcess, Action onFinish, ref PlayingAudio playing)
	{
		if (playing == null || playing.audioSource == null) {
			AudioSource source = gameObject.AddComponent<AudioSource> ();
			playing = new PlayingAudio (source, clip, volume, mute, false, onProcess, onFinish);
		}
		playing.Play ();
	}

	/// <summary>
	/// 重ねて再生する（AudioSource.PlayOneShotのように動作する）。
	/// </summary>
	private AudioSource PlayOneShot (AudioClip clip, float volume, bool mute, Action<float, float> onProcess, Action onFinish, ref List<PlayingAudio> list)
	{
		AudioSource source = gameObject.AddComponent<AudioSource> ();

		var playingAudio = new PlayingAudio (source, clip, volume, mute, false, onProcess, onFinish);
		list.Add (playingAudio);
		playingAudio.Play ();
		return source;
	}

	/// <summary>
	/// 指定したAudioSourceで再生する
	/// </summary>
	private void PlayWithExternalSource (AudioSource source, AudioClip clip, float volume, bool mute, Action<float, float> onProcess, Action onFinish, ref List<PlayingAudio> list)
	{
		var playingAudio = new PlayingAudio (source, clip, volume, mute, true, onProcess, onFinish);
		list.Add (playingAudio);
		playingAudio.Play ();
	}

	/// <summary>
	/// 指定したAudioSourceを再生する
	/// </summary>
	private void PlayExternalSource (AudioSource source, float volume, bool mute, Action<float, float> onProcess, Action onFinish, ref List<PlayingAudio> list)
	{
		AudioClip clip = source.clip;

		var playingAudio = new PlayingAudio (source, clip, volume, mute, true, onProcess, onFinish);
		list.Add (playingAudio);
		playingAudio.Play ();
	}

	private Dictionary<AudioSource, IEnumerator> fadings = new Dictionary<AudioSource, IEnumerator> ();

	private void FadeVolume (AudioSource source, float to, float duration, Action onFinish)
	{
		if (fadings.ContainsKey (source)) {
			StopCoroutine (fadings [source]);
			fadings.Remove (source);
		}

		var _from = source.volume;
		var _to = Mathf.Clamp (to, 0, 1);
		if (_from == _to) {
			if (onFinish != null)
				onFinish ();
			return;
		}

		Action f = () => {
			fadings.Remove (source);
			if (onFinish != null)
				onFinish ();
		};

		var fading = FadeVolumeCoroutine (source, _from, _to, duration, f);
		fadings [source] = fading;
		StartCoroutine (fading);
	}

	private IEnumerator FadeVolumeCoroutine (AudioSource source, float from, float to, float duration, Action onFinish)
	{
		var isVolumeUp = (from < to);
		var max = Mathf.Max (from, to);
		var min = Mathf.Min (from, to);
		if (duration > 0) {
			var timer = 0f;
			while (timer < duration && source != null) {
				if (fadeCurve == null) {
					if (isVolumeUp) {
						source.volume = Mathf.Lerp (min, max, timer / duration);
					} else {
						source.volume = Mathf.Lerp (min, max, 1 - timer / duration);
					}
				} else {
					if (isVolumeUp) {
						source.volume = fadeCurve.Evaluate (timer / duration) * (max - min) + min;
					} else {
						source.volume = fadeCurve.Evaluate (1 - timer / duration) * (max - min) + min;
					}
				}
				timer += Time.deltaTime;
				yield return null;
			}
		}
		if (source != null)
			source.volume = (isVolumeUp) ? max : min;

		onFinish ();
	}

	private AudioSource Play3DLoop (Transform parent, AudioClip clip, float volume, bool mute, ref List<PlayingAudio> list)
	{
		AudioSource source = CreateAudioSourceObject ("3DAudio");
		source.loop = true;
		Play3D (source, parent, clip, volume, mute, null, null, ref list);
		return source;
	}

	private AudioSource Play3D (AudioSource source, Transform parent, AudioClip clip, float volume, bool mute,
		Action<float, float> onProcess, Action onFinish, ref List<PlayingAudio> list)
	{
		bool isExternalSource = source != null;
		source.rolloffMode = AudioRolloffMode.Linear;
		source.minDistance = 2.5f;
		source.maxDistance = 7.5f;
		var playingAudio = new PlayingAudio (source, clip, volume, mute, isExternalSource, onProcess, onFinish);
		list.Add (playingAudio);
		playingAudio.Play ();

		if (parent != null) {
			source.transform.parent = parent;
			source.transform.localPosition = Vector3.zero;
		}
		return source;
	}

	private Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip> ();

	public void ClearAudioClipCache ()
	{
		audioClipDict.Clear ();
	}

	public AudioClip LoadAudioClip (string filePath)
	{
		AudioClip result = null;
		if (!audioClipDict.TryGetValue (filePath, out result)) {
			result = Resources.Load<AudioClip> (filePath);
			audioClipDict.Add (filePath, result);
		}
		return result;
	}

	public void LoadAudioClipAsync (string filePath, Action<AudioClip> onLoaded)
	{
		EnqueueLoadingItem (filePath, onLoaded);
	}


	#region load queue

	private class LoadQueueItem
	{
		public string filePath;
		public Action<AudioClip> onLoaded;

		public LoadQueueItem (string filePath, Action<AudioClip> onLoaded)
		{
			this.filePath = filePath;
			this.onLoaded = onLoaded;
		}
	}

	Queue<string> waitLoad = new Queue<string> ();
	Dictionary<string, LoadQueueItem> loadEvents = new Dictionary<string, LoadQueueItem> ();
	string loadingPath = null;

	private void EnqueueLoadingItem (string filePath, Action<AudioClip> onLoaded)
	{
		AudioClip clip;
		if (audioClipDict.TryGetValue (filePath, out clip)) {

			onLoaded.SafeCall (clip);
			return;
		}

		LoadQueueItem item;
		if (loadEvents.TryGetValue (filePath, out item)) {
			item.onLoaded += onLoaded;
		} else {
			waitLoad.Enqueue (filePath);
			item = new LoadQueueItem (filePath, onLoaded);
			loadEvents.Add (filePath, item);
		}

		Load ();
	}

	private void Load ()
	{
		if (waitLoad.Count <= 0)
			return;

		var filePath = waitLoad.Peek ();
		if (filePath == loadingPath)
			return;

		loadingPath = filePath;
		var loadEvent = loadEvents [loadingPath];

		StartCoroutine (LoadClipAsync (loadingPath, clip => {
			waitLoad.Dequeue ();
			if (!audioClipDict.ContainsKey (filePath))
				audioClipDict.Add (filePath, clip);
			loadEvent.onLoaded.SafeCall (clip);
			loadEvents.Remove (filePath);
			loadingPath = null;
			Load ();
		}));
	}

	private IEnumerator LoadClipAsync (string path, Action<AudioClip> onLoaded)
	{
		var asyncOperation = Resources.LoadAsync<AudioClip> (path);
		while (!asyncOperation.isDone)
			yield return null;

		onLoaded (asyncOperation.asset as AudioClip);
	}

	#endregion

	public void Reinitialize ()
	{
		AudioClip bgmClip = null;
		float time = 0;
		if (bgmSource != null) {
			bgmClip = bgmSource.clip;
			time = bgmSource.time;
		}

		Stop ();
		AudioSettings.outputSampleRate++;
		AudioSettings.outputSampleRate--;

		if (bgmClip != null) {
			PlayBGM (bgmClip, time);
		}
	}
}
