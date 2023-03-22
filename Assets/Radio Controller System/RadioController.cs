/// This script was written by Joshua Ferguson.
/// Version 1.0.2
/// Last updated - 21.03.23

using FMODUnity;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class is used to control the overall radio functionality using a finite state machine.
/// This MonoBehaviour should be attached to an object to turn it into a radio controller.
/// An audio source will automatically be attached to the object this MonoBehaviour is attached to (if there is not already one attached).
/// </summary>
public class RadioController : FiniteStateMachine
{
    [Tooltip("Enables/disables the radio static when changing stations.")]
    [SerializeField] private bool enableStationToggleClips = true;
    [Tooltip("The list of audio clips for radio static when changing stations.")]
    [SerializeField] private EventReference[] toggleEvents;
    [Tooltip("The list of radio stations.")]
    [SerializeField] private RadioStation[] stations;
    [Tooltip("The index of the radio station that the game will begin with.")]
    [SerializeField] private int startStation = 0;

    /// <summary>
    /// The index of the currently active radio station.
    /// </summary>
    private int currentStationIndex = -1;
    /// <summary>
    /// A reference to the FMOD studio emitter component attached to this object.
    /// </summary>
    private StudioEventEmitter eventEmitter;
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
    /// Returns true if the current radio station is possessed.
    /// </summary>
    public bool CurrentStationIsPossessed
    {
        get
        {
            return stations[currentStationIndex].Possessed;
        }
    }
    
    /// <summary>
    /// Awake is called before Start.
    /// </summary>
    private void Awake()
    {
        //Instantiates stations provided that an audio source is attached to this object.
        if(TryGetComponent(out eventEmitter) == true)
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
    /// <param name="ignorePossessed">If true, possessed stations will be ignored.</param>
    /// <returns>Returns true if the passed index is within range of the defined number of radio stations.</returns>
    private bool SetStation(int index, ref int currentRadioIndex, bool ignorePossessed = true)
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
                if (ignorePossessed == true) //the station is possessed but the method is set to ignore possessed stations, find next unpossessed station
                {
                    Debug.Log("Ignoring possessed");
                    //recursive call until end of array has been searched for unpossessed station (only checks in the direction of transition)
                    if (index < CurrentStationIndex)
                    {
                        return SetStation(index - 1, ref currentRadioIndex);
                    }
                    else if (index > CurrentStationIndex)
                    {
                        return SetStation(index + 1, ref currentRadioIndex);
                    }
                }
                else
                {
                    if (transitionRoutine != null) { StopCoroutine(transitionRoutine); }
                    transitionRoutine = StartCoroutine(ToggleRadioStation(index));
                    currentRadioIndex = index;
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Use this method to set the radio to a possessed station.
    /// NOTE: This method will still transition to the station if it is not possessed!
    /// </summary>
    /// <param name="possessedStationIndex"></param>
    /// <returns>Returns the result of the transition.</returns>
    public bool StartRadioPossession(int possessedStationIndex)
    {
        return SetStation(possessedStationIndex, ref currentStationIndex, false);
    }

    /// <summary>
    /// Use this method to transition back to a regular radio station.
    /// </summary>
    /// <param name="stationIndex">The index of the regular radio station to resume playing.</param>
    /// <returns>Returns the result of the transition.</returns>
    public bool EndRadioPossession(int stationIndex)
    {
        return SetStation(stationIndex, ref currentStationIndex);
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
            eventEmitter.Stop();
            if(toggleEvents.Length > 0 == false) { yield break; }
            int r = Random.Range(0, toggleEvents.Length);
            eventEmitter.EventReference = toggleEvents[r];
            //reset event playback to beginning here if necessary
            eventEmitter.Play();
            float lengthInSeconds = 0;
            eventEmitter.EventDescription.getLength(out int lengthInMS);
            lengthInSeconds = lengthInMS / 1000f;
            yield return new WaitForSeconds(lengthInSeconds);
            //reset emitter event to null here?
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
        [SerializeField] private EventReference[] clipQueue; //*
        [Tooltip("Toggle on to receive console messages related to the playback times of this radio station.")]
        [SerializeField] private bool debugPlaybackTimes;

        private int currentTrackIndex = 0;  //tracks the index of the track currently being played
        private int stationIteration = 1;   //tracks the current iteration of the entire station
        private double trackElapsedTime = 0d;   //tracks the elapsed time of the current track
        private double stationElapsedTime = 0d; //tracks the elapsed time for the entire station
        private readonly RadioController instance; //reference to the radio controller that owns this station
        private Coroutine setClipRoutine;

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
        public EventReference CurrentTrack { get; private set; } //**

        /// <summary>
        /// Clones an instance of a radio station, with reference to an audio source.
        /// </summary>
        /// <param name="originalInstance">The radio station instance being cloned.</param>
        /// <param name="controllerInstance">The radio controller playing the radio station.</param>
        public RadioStation(RadioStation originalInstance, RadioController controllerInstance)
        {
            name = originalInstance.name;
            debugPlaybackTimes = originalInstance.debugPlaybackTimes;
            clipQueue = originalInstance.clipQueue; //**
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
            double currentTrackDuration = GetTrackDuration(); //Get duration of last played track
            //for above, get time elapsed in current event

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
                while (timeElapsed >= GetTrackDuration() == true) //if next track duration is more than elapsed time
                {
                    //for above, get time elapsed in current event
                    if (debugPlaybackTimes == true)
                    {
                        Debug.LogWarning($"[{name.ToUpper()}] Next track {trackIndex} duration more than time elapsed: {timeElapsed}");
                    }
                    timeElapsed -= GetTrackDuration(); //subtract duration of this track from time elapsed
                    //for above, get time elapsed in current event
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
            if(instance.eventEmitter.IsPlaying() == false) //if the current track has finished playing
            {
                if (currentTrackIndex + 1 < clipQueue.Length) //if the next index is within bounds of array
                {
                    SetClip(currentTrackIndex + 1, 0d); //play the next clip
                    if (debugPlaybackTimes == true)
                    {
                        Debug.Log($"[{name.ToUpper()}] Playing the next track: {CurrentTrack.Guid}");
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
                        Debug.Log($"[{name.ToUpper()}] STATION RESET! Playing the first track: {CurrentTrack.Guid}");
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
            instance.eventEmitter.EventInstance.getTimelinePosition(out int timelinePos);  //track elapsed track time as a double
            trackElapsedTime = (double)(timelinePos / 1000f);
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
                instance.eventEmitter.EventReference = CurrentTrack; //create new instance of new track before its used??
                instance.eventEmitter.EventInstance.setTimelinePosition((int)((time + delay) * 1000));
                if(setClipRoutine != null) { instance.StopCoroutine(setClipRoutine); }
                setClipRoutine = instance.StartCoroutine(PlayEmitterAfterDelay((int)((time + delay) * 1000)));
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

        private IEnumerator PlayEmitterAfterDelay(int delay)
        {
            yield return new WaitForSeconds(delay);
            instance.eventEmitter.Play();
            setClipRoutine = null;
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


        public double GetTrackDuration()
        {
            instance.eventEmitter.EventInstance.getTimelinePosition(out int timeLinePos); //Get duration of last played track
            return (double)timeLinePos * 1000;
        }
    }
}
