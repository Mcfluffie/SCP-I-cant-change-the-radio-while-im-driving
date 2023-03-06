/// This script was written by Joshua Ferguson.
/// Version 1.0.0
/// Last updated - 03.12.22

using System.Collections;
using UnityEngine;

/// <summary>
/// This class is used to control the overall radio functionality using a finite state machine.
/// This MonoBehaviour should be attached to an object to turn it into a radio controller.
/// An audio source will automatically be attached to the object this MonoBehaviour is attached to (if there is not already one attached).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class RadioController : FiniteStateMachine
{
    [Tooltip("Enables/disables the radio static when changing stations.")]
    [SerializeField] private bool enableStationToggleClips = true;
    [Tooltip("The list of audio clips for radio static when changing stations.")]
    [SerializeField] private AudioClip[] toggleClips;
    [Tooltip("The list of radio stations.")]
    [SerializeField] private RadioStation[] stations;
    [Tooltip("The index of the radio station that the game will begin with.")]
    [SerializeField] private int startStation = 0;

    /// <summary>
    /// The index of the currently active radio station.
    /// </summary>
    private int currentStationIndex = -1;
    /// <summary>
    /// A reference to the audio source component attached to this object.
    /// </summary>
    private AudioSource aSrc;
    /// <summary>
    /// A reference to the currently active station transition coroutine.
    /// </summary>
    private Coroutine transitionRoutine;

    /// <summary>
    /// Use to get or set the currently active station.
    /// Logs a warning if the station could not be changed.
    /// </summary>
    public int CurrentStationIndex
    {
        get
        {
            return currentStationIndex;
        }
        set
        {
            if (SetStation(value, ref currentStationIndex) == false)
            {
                Debug.LogWarning("Changing radio stations failed!");
            }
        }
    }
    
    /// <summary>
    /// Awake is called before Start.
    /// </summary>
    private void Awake()
    {
        //Instantiates stations provided that an audio source is attached to this object.
        if(TryGetComponent(out aSrc) == true)
        {
            for(int i = 0; i < stations.Length; i++)
            {
                stations[i] = new RadioStation(stations[i], this);
            }
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        if (stations.Length > 0) //if there are radio stations defined
        {
            //set the station to the start station, provided the index is within range of the array
            CurrentStationIndex = startStation < 0 || startStation >= stations.Length ? 0 : startStation;
        }
    }

    /// <summary>
    /// Sets the current station to the station corresponding with the passed index.
    /// </summary>
    /// <param name="index">The index of the station being set.</param>
    /// <param name="currentRadioIndex">A reference to the variable tracking the current station index.</param>
    /// <returns>Returns true if the passed index is within range of the defined number of radio stations.</returns>
    private bool SetStation(int index, ref int currentRadioIndex)
    {
        if(index >= 0 && index < stations.Length)
        {
            if (stations[index].Possessed == false)
            {
                if (transitionRoutine != null) { StopCoroutine(transitionRoutine); }
                transitionRoutine = StartCoroutine(ToggleRadioStation(index));
                currentRadioIndex = index;
                return true;
            }
            else
            {
                //recursive call until end of array has been searched for unpossessed station (only checks in the direction of transition)
                if(index < CurrentStationIndex)
                {
                    return SetStation(index - 1, ref currentRadioIndex);
                }
                else if (index > CurrentStationIndex)
                {
                    return SetStation(index + 1, ref currentRadioIndex);
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Coroutine responsible for swapping stations with a delay.
    /// Transitions from the current station to the station corresponding with the passed index.
    /// </summary>
    /// <param name="index">The index of the station being transitioned to.</param>
    /// <returns>For the amount of time associated with the randomly selected toggle clip (if there is one, otherwise for 0.25 seconds).</returns>
    private IEnumerator ToggleRadioStation(int index)
    {
        if (enableStationToggleClips == true)
        {
            SetState(null);
            int r = Random.Range(0, toggleClips.Length);
            aSrc.clip = toggleClips[r];
            aSrc.time = 0;
            aSrc.Play();
            yield return new WaitForSeconds(toggleClips.Length > 0 ? toggleClips[r].length : 0.25f);
            aSrc.clip = null;
        }
        SetState(stations[index]);
    }

    /// <summary>
    /// This is class is used to define individual radio stations.
    /// </summary>
    [System.Serializable]
    public class RadioStation : IState
    {
        [Tooltip("The name of the radio station.")]
        [SerializeField] private string name;
        [Tooltip("Toggle on to make this radio station possessed.")]
        [SerializeField] private bool possessed;
        [Tooltip("The defined sequence of audio tracks (in the correct order).")]
        [SerializeField] private AudioClip[] clipQueue;
        [Tooltip("Toggle on to receive console messages related to the playback times of this radio station.")]
        [SerializeField] private bool debugPlaybackTimes;

        private int currentTrackIndex = 0;  //tracks the index of the track currently being played
        private int stationIteration = 1;   //tracks the current iteration of the entire station
        private double trackElapsedTime = 0d;   //tracks the elapsed time of the current track
        private double stationElapsedTime = 0d; //tracks the elapsed time for the entire station
        private readonly RadioController instance; //reference to the radio controller that owns this station

        /// <summary>
        /// Returns the name of the station.
        /// </summary>
        public string Name { get { return name; } }
        /// <summary>
        /// Use to get or set whether the station is possessed.
        /// </summary>
        public bool Possessed { get { return possessed; } set { possessed = value; } }
        /// <summary>
        /// Returns the track currently being played.
        /// </summary>
        public AudioClip CurrentTrack { get; private set; }

        /// <summary>
        /// Clones an instance of a radio station, with reference to an audio source.
        /// </summary>
        /// <param name="originalInstance">The radio station instance being cloned.</param>
        /// <param name="controllerInstance">The radio controller playing the radio station.</param>
        public RadioStation(RadioStation originalInstance, RadioController controllerInstance)
        {
            name = originalInstance.name;
            debugPlaybackTimes = originalInstance.debugPlaybackTimes;
            clipQueue = originalInstance.clipQueue;
            possessed = originalInstance.possessed;
            instance = controllerInstance;
        }

        /// <summary>
        /// The functionality called on the first frame the radio station is made active.
        /// </summary>
        public void Enter()
        {
            double timeElapsed =  GetTimeElapsedSinceLastVisit();   //time elapsed since last visit to station
            double trackPlaybackPosition = trackElapsedTime + timeElapsed; //add time elapsed since last visit to the elapsed time of current track
            double currentTrackDuration = GetTrackDuration(clipQueue[currentTrackIndex]); //Get duration of last played track

            if (trackPlaybackPosition >= currentTrackDuration)   //if more time has elapsed than length of most recent track
            {
                if (debugPlaybackTimes == true)
                {
                    Debug.LogWarning($"[{name.ToUpper()}] Playback position {trackPlaybackPosition} is more than current track {currentTrackIndex} duration {currentTrackDuration}!");
                }
                timeElapsed -= currentTrackDuration - trackElapsedTime; //subtract elapsed time of last played track from its duration, then subtract remainder from time elapsed since last visit
                if (debugPlaybackTimes == true)
                {
                    Debug.LogWarning($"[{name.ToUpper()}] Calculated new elapsed time {timeElapsed} for track {currentTrackIndex}!");
                }
                int trackIndex = currentTrackIndex + 1 < clipQueue.Length ? currentTrackIndex + 1 : 0; //start looping from the next track
                while (timeElapsed >= GetTrackDuration(clipQueue[trackIndex]) == true) //if next track duration is more than elapsed time
                {
                    if (debugPlaybackTimes == true)
                    {
                        Debug.LogWarning($"[{name.ToUpper()}] Next track {trackIndex} duration more than time elapsed: {timeElapsed}");
                    }
                    timeElapsed -= GetTrackDuration(clipQueue[trackIndex]); //subtract duration of this track from time elapsed
                    if (trackIndex + 1 >= clipQueue.Length) //increment track index being iterated through
                    {
                        trackIndex = 0;
                    }
                    else
                    {
                        trackIndex++;
                    }
                }
                trackPlaybackPosition = timeElapsed;    //set playback position for new track
                if (debugPlaybackTimes == true)
                {
                    Debug.LogWarning($"[{name.ToUpper()}] Calculated new playback position! Playing back track {trackIndex} at position: {trackPlaybackPosition}");
                }
                SetClip(trackIndex, trackPlaybackPosition);
            }
            else //otherwise resume playback of most recent track
            {
                if (debugPlaybackTimes == true)
                {
                    Debug.Log($"[{name.ToUpper()}] Playing back track {currentTrackIndex} at position: {trackPlaybackPosition} \n--- {trackElapsedTime} + {timeElapsed}");
                }
                SetClip(currentTrackIndex, trackPlaybackPosition);
            }
        }

        /// <summary>
        /// The functionality called every frame the radio station is active.
        /// </summary>
        public void Update()
        {
            if(instance.aSrc.isPlaying == false && instance.aSrc.clip != null) //if the current track has finished playing
            {
                if (currentTrackIndex + 1 < clipQueue.Length) //if the next index is within bounds of array
                {
                    SetClip(currentTrackIndex + 1, 0d); //play the next clip
                    if (debugPlaybackTimes == true)
                    {
                        Debug.Log($"[{name.ToUpper()}] Playing the next track: {CurrentTrack.name}");
                    }
                }
                else
                {
                    SetClip(0, 0d); //reset to the start of the playlist
                    //reset station and track elapsed times
                    trackElapsedTime = 0d;
                    stationElapsedTime = 0d;
                    stationIteration++; //increase station iteration
                    if (debugPlaybackTimes == true)
                    {
                        Debug.Log($"[{name.ToUpper()}] STATION RESET! Playing the first track: {CurrentTrack.name}");
                    }
                }
            }
        }

        /// <summary>
        /// The functionality called on the last frame the radio station is active.
        /// </summary>
        public void Exit()
        {
            stationElapsedTime = (Time.timeSinceLevelLoadAsDouble / stationIteration);  //track elapsed station time by dividing overall time since level loaded, by number of current iterations of station
            trackElapsedTime = instance.aSrc.timeSamples / CurrentTrack.frequency;  //track elapsed track time as a double
            if (debugPlaybackTimes == true)
            {
                Debug.Log($"[{name.ToUpper()}] Station iteration {stationIteration} elapsed time: {stationElapsedTime}.");
            }
        }

        /// <summary>
        /// Sets the currntly played clip to the clip corresponding with the past index, playing from the passed time.
        /// </summary>
        /// <param name="index">The index of the clip to play.</param>
        /// <param name="time">The time to start playing the clip from.</param>
        /// <param name="delay">The delay in seconds before playing the clip.</param>
        private void SetClip(int index, double time, double delay = 0.1d)
        {
            if (index >= 0 && index < clipQueue.Length)
            {
                currentTrackIndex = index;
                CurrentTrack = clipQueue[currentTrackIndex];
                instance.aSrc.clip = CurrentTrack;
                instance.aSrc.timeSamples = (int)((time + delay) * CurrentTrack.frequency);
                instance.aSrc.PlayScheduled(delay);
                if (debugPlaybackTimes == true)
                {
                    Debug.Log($"[{name.ToUpper()}] Clip {currentTrackIndex} scheduled to play at time: {(float)(time + delay)}");
                }
            }
            else if (debugPlaybackTimes == true)
            {
                Debug.Log($"{name}: Track index out of range.");
            }
        }

        /// <summary>
        /// Returns the time that has passed since this radio station was last played.
        /// </summary>
        /// <returns>The precise amount of time in seconds (as a double) that has passed since the radio station was last active.</returns>
        private double GetTimeElapsedSinceLastVisit()
        {
            double elapsedTime = (Time.timeSinceLevelLoadAsDouble / stationIteration) - stationElapsedTime; //subtract elapsed time when station was last played, from overall time since level loaded
            if (debugPlaybackTimes == true)
            {
                Debug.Log($"[{name.ToUpper()}] Time elapsed since last play of this station: {elapsedTime}");
            }
            return elapsedTime;
        }

        /// <summary>
        /// Returns the precise duration of the passed audio clip.
        /// </summary>
        /// <param name="clip">A reference to the clip to find the duration of.</param>
        /// <returns>The duration of the clip as a double.</returns>
        public double GetTrackDuration(AudioClip clip)
        {
            return (double)clip.samples / clip.frequency;
        }
    }
}
