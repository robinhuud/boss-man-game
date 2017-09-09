﻿using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;



namespace AudioVisualizer
{
    /// <summary>
    /// Samples the audio across multiple audio sources.
    /// All other AudioVisualizer classes rely on this class to sample the audio data.
    /// There should just be 1 instance of this in each scene.
    /// </summary>
    public class AudioSampler : MonoBehaviour
    {

        
        public static AudioSampler instance; //singleton static instance
        [Tooltip("Runtime AudioSource components to sample")]
        public List<AudioSource> audioSources; // list of audio sources used for audio input.
        [Tooltip("Pre-recorded audio files to sample")]
        public List<TextAsset> audioFiles;
        [Tooltip("Silent Audio can be used for runtime beat-anticipation")]
        public SilentAudio silentAudio; // list of audio sources that are played "preBeatOffset" seconds later than "audioSources"
        [Tooltip("Show debug info?")]
        public bool debug = false; // if true, show audio data being sampled
        [Tooltip("Play audio right away?")]
        public bool playOnAwake = true; //does the audio start out paying right away
        [Tooltip("Use a microphone?")]
        public bool usingMic = false; // flag if we're using microphoneInput
        [Tooltip("If usingMic, volume < this will be filtered out")]
        public float micVolumeThreshold = .1f; // if using mic, volume has to be > than this to count as valid input
        [Tooltip("Sensitivity that affects all waveforms")]
        public float globalSensitivity = 1f; // use this to adjust sensitivity on all waveforms simultaneously

        //events
        public delegate void AudioSamplerEvent();
        public static AudioSamplerEvent OnStop;
        public delegate void AudioSamplerUpdate(float time,float deltaTime);
        public static AudioSamplerUpdate AudioUpdate;

        // used for drawing the debug chart.
        private Texture2D drawTexture; 
        private Color startColor = Color.magenta;
        private Color endColor = Color.blue;
        private Gradient gradient;
        private float fMax;// = (float)AudioSettings.outputSampleRate/2;
        private List<string> debugLables = new List<string>() { "SubBass", "Bass", "LowMid", "Mid", "UpperMid", "High", "VeryHigh", "Decibal" };
        private int samplesToTake = 1024; // how many audio samples should we take?
        private float audioTimer = 0;
        private bool playing = false;
        private List<AudioData> audioDatas;

        //singleton logic


        void Awake()
        {
            drawTexture = Texture2D.whiteTexture; // get an empty white texture
            gradient = PanelWaveform.GetColorGradient(startColor, endColor); // get a color gradient.
            if (audioSources.Count == 0) // if we haven't assigned any audio sources
            {
                if (this.GetComponent<AudioSource>() != null) // try to grab one from this gameobject
                {
                    audioSources.Add(this.GetComponent<AudioSource>());
                }
                else
                {
                    Debug.LogError("Error! no audio sources attached to AudioSampler.css");
                }
            }

            audioDatas = new List<AudioData>();
            foreach(TextAsset file in audioFiles)
            {
                AudioData data = JsonUtility.FromJson<AudioData>(file.text);
                data.Decompress();
                audioDatas.Add(data);
            }

            //prebeat audio, used for runtime beat detection with a preBeatOffset
            if (silentAudio.useSilentAudio)
            {
                //silence the orginal sources
                foreach (AudioSource source in silentAudio.audioSources)
                {
                    source.outputAudioMixerGroup = silentAudio.silentMixerGroup;
                }
            }

            //set the initial state of the playing variable
            if(playOnAwake)
            {
                Play();
            }
        }

        void OnEnable()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        void OnDisable()
        {
            instance = null;
        }

        void Start()
        {
            //get max frequency
            fMax = (float)AudioSettings.outputSampleRate / 2;
           
        }

        void Update()
        {
            float deltaTime = Time.deltaTime;
            if(playing)
            {

                //Debug.Log("Loud: " + audioSources[0].time + " Silent: " + silentAudio.audioSources[0].time);
                
                if (AudioUpdate != null)
                {
                    //send a message to all subscribers to the AudioUpdate event
                    AudioUpdate.Invoke(audioTimer, deltaTime);
                }
                audioTimer += deltaTime;

            }
        }
   

        //draw the debug chart if enabled.
        void OnGUI()
        {
            if (debug)
            {

                //rms
                // avg
                // 7 Frequency Range volumes
                int heightPerSource = 100;
                for (int s = 0; s < audioSources.Count; s++)
                {
                    int width = (int)(Screen.width * .5f);
                    int height = heightPerSource * (s + 1);
                    int headerFooter = (int)(height * .2f);
                    int spacing = (int)(width / debugLables.Count);
                    int barWidth = 10;
                    int yBottom = height - headerFooter;
                    int yTop = headerFooter;
                    int indent = 40; //left indent

                    GUI.color = Color.white;
                    GUI.Label(new Rect(0, yBottom, 60, 20), "Source: " + s);
                    for (int j = 0; j < debugLables.Count; j++)
                    {
                        float percent = (float)j / (debugLables.Count - 1);
                        int x = indent + spacing + spacing * j;
                        Vector2 start = new Vector2(x, yBottom);
                        float volume = Mathf.Clamp(GetFrequencyVol(s, (FrequencyRange)j,false) * 10, 0, .5f);
                        float y = yBottom - heightPerSource * volume;
                        //Debug.Log(i + " vol: " + volume + " y: " + y); 
                        Vector2 end = new Vector2(x, y);
                        DrawLine(start, end, barWidth, gradient.Evaluate(percent));
                        GUI.Label(new Rect(x, yBottom, 60, 20), debugLables[j]);
                        GUI.Label(new Rect(x, yBottom + 20, 40, 20), volume.ToString("F3"));
                    }
                }

            }
        }
        //draw a line of the debug chart.
        private void DrawLine(Vector2 start, Vector2 end, int width, Color color)
        {
            GUI.color = color;
            Vector2 d = end - start;
            float a = Mathf.Rad2Deg * Mathf.Atan(d.y / d.x);
            if (d.x < 0)
                a += 180;

            int width2 = (int)Mathf.Ceil(width / 2);

            if (Vector2.Distance(start, end) > .1)
            {
                GUIUtility.RotateAroundPivot(a, start);
                GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), drawTexture);
                GUIUtility.RotateAroundPivot(-a, start);
            }
        }


        //get an array of output data (decibals)
        public float[] GetAudioSamples(int audioSourceIndex, bool useAudioFile)
        {
            if (useAudioFile)
                return GetAudioSamplesFromFile(audioSourceIndex);

            float[] samples = new float[samplesToTake];
            if (audioSources[audioSourceIndex].mute && !useAudioFile) // if muted
            {
                return samples;
            }

            audioSources[audioSourceIndex].GetOutputData(samples, 0); // grab samples!
            //normalize the samples
            float[] normSamples = NormalizeArray(samples);

            //multiply by volume if we're not using mic input, where volume is set to 0 to avoid feedback issues;
            if (!usingMic)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    normSamples[i] = normSamples[i] * audioSources[audioSourceIndex].volume;
                }
            }
            return normSamples;
            

            
        }

        //get an array of output data, averaged into 'numBins' bins
        public float[] GetAudioSamples(int audioSourceIndex, int numBins, bool absoluteVal, bool useAudioFile)
        {
            float[] samples = new float[numBins];
            //if using an audio file
            if(useAudioFile)
            {
                //grab samples from the file
                samples = GetAudioSamplesFromFile(audioSourceIndex);
                //bin them into an array with the correct size
                float[] values = GetBinnedArray(samples, numBins);
                //handle abs value if needed
                if(absoluteVal)
                {
                    for(int i = 0; i < values.Length; i++)
                    {
                        values[i] = Mathf.Abs(values[i]);
                    }
                }
                return values;
            }

            if (audioSources[audioSourceIndex].mute && !useAudioFile) // if muted
            {
                return samples;
            }

            audioSources[audioSourceIndex].GetOutputData(samples, 0); // grab samples!
            //normalize the samples
            float[] normSamples = NormalizeArray(samples);
            //multiply by volume.
            for (int i = 0; i < samples.Length; i++)
            {
                if (absoluteVal)
                {
                    normSamples[i] = Mathf.Abs(samples[i]);
                }
                else
                {
                    normSamples[i] = samples[i];
                }
                //multiply by volume if we're not using mic input, where volume is set to 0 to avoid feedback issues;
                if (!usingMic)
                {
                    normSamples[i] = normSamples[i] * audioSources[audioSourceIndex].volume;
                }
            }

            return normSamples;
            

            
        }

        //Get pre-recorded audio samples
        float[] GetAudioSamplesFromFile(int audioSourceIndex)
        {
            float loopTime = audioTimer % audioDatas[audioSourceIndex].clipLength;
            return audioDatas[audioSourceIndex].GetSamples(FrequencyRange.Decibal, loopTime);
        }
        //Get pre-recorded audio samples
        float[] GetAudioSamplesFromFile(FrequencyRange frequencyRange, int audioSourceIndex)
        {
            float loopTime = audioTimer % audioDatas[audioSourceIndex].clipLength;
            return audioDatas[audioSourceIndex].GetSamples(frequencyRange, loopTime);
        }


        //sample the audio, square each value, and sum them all to get instant energy (the current 'energy' in the audio)
        public float GetInstantEnergy(int audioSourceIndex, bool useAudioFile)
        {
            if (audioSources[audioSourceIndex].mute && !useAudioFile) // if muted
            {
                return 0;
            }

            float[] audioSamples = GetAudioSamples(audioSourceIndex,useAudioFile);
                
            float sum = 0;
            //sum up the audio samples
            foreach (float f in audioSamples)
            {
                sum += (f * f);
            }
            //multiply by volume if we're not using mic input, where volume is set to 0 to avoid feedback issues;
            if (!usingMic)
            {
                sum = sum * audioSources[audioSourceIndex].volume;
            }
            return sum;

        }

        //Get the RMS value of the audio (Root means squared) value 0-1
        //An average "noise" value of the audio at this point in time, using samplesToTake audio samples, and the passed in sensitivity.
        public float GetRMS(int audioSourceIndex, bool useAudioFile, bool useSilent = false)
        {
            AudioSource source = useSilent ? silentAudio.audioSources[audioSourceIndex] : audioSources[audioSourceIndex];
            if (source.mute && !useAudioFile && !usingMic) // if muted
            {
                return 0;
            }

            //grab output data (decibals)
            float[] audioSamples = new float[samplesToTake];
            if (useAudioFile)
            {
                audioSamples = GetAudioSamplesFromFile(audioSourceIndex);
            }
            else
            {
                source.GetOutputData(audioSamples, 0); // fill array with samples
            }
            //float[] normSamples = NormalizeArray(audioSamples);
            int i;
            float sum = 0;
            for (i = 0; i < audioSamples.Length; i++)
            {
                sum += audioSamples[i] * audioSamples[i]; // sum squared samples
            }
            float rmsValue = Mathf.Sqrt(sum / samplesToTake); // rms = square root of average
            //multiply by volume if we're not using mic input, where volume is set to 0 to avoid feedback issues;
            if(usingMic)
            {

                //if usin the mic, check to see if we meet the volume requirement
                if (rmsValue < micVolumeThreshold)
                {
                    if (debug)
                    {
                        Debug.Log("Mic Filtered Out: " + rmsValue + "<" + micVolumeThreshold);
                    }
                    rmsValue = 0;
                }
            }
            else
            {
                rmsValue = rmsValue * source.volume;
            }


            return rmsValue;

        }

        //like GetAvg or GetRMS, but inside a given frequency range
        public float GetFrequencyVol(int audioSourceIndex, FrequencyRange freqRange, bool useAudioFile, bool useSilent = false)
        {
            //grab the correct audio source, from the correct list! Prebeat or normal
            AudioSource source = useSilent ? silentAudio.audioSources[audioSourceIndex] : audioSources[audioSourceIndex];
            if (!source.mute) // if not muted
            {
                Vector2 range = GetFreqForRange(freqRange);
                float fLow = range.x;//Mathf.Clamp (range.x, 20, fMax); // limit low...
                float fHigh = range.y;//Mathf.Clamp (range.y, fLow, fMax); // and high frequencies
                // get spectrum
                float[] freqData = new float[samplesToTake];
                if (useAudioFile)
                {
                    freqData = GetAudioData(audioSourceIndex).GetSamples(freqRange, audioTimer);
                }
                else
                {
                    source.GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris);
                }
                
                int n1 = (int)Mathf.Floor(fLow * samplesToTake / fMax);
                int n2 = (int)Mathf.Floor(fHigh * samplesToTake / fMax);
                float sum = 0;
                // Debug.Log("Smapling freq: " + n1 + "-" + n2);
                // average the volumes of frequencies fLow to fHigh
                for (int i = n1; i <= n2; i++)
                {
                    if (i < freqData.Length && i >= 0)
                    {
                        sum += Mathf.Abs(freqData[i]);
                    }
                }
                //multiply by volume if we're not using mic input, where volume is set to 0 to avoid feedback issues;
                if (usingMic)
                {
                    //zero out the sum if we don't meet the min microphone volume requirement
                    if(sum < micVolumeThreshold)
                    {
                        if (debug)
                        {
                            Debug.Log("Mic Filtered Out: " + sum + "<" + micVolumeThreshold);
                        }
                        sum = 0;
                    }
                }
                else
                {
                    sum = sum * source.volume;
                }
                float frequency = sum / (n2 - n1 + 1);
                //Debug.Log("Freq: " + frequency);
                return frequency;
            }

            return 0;
        }

        //return the raw spectrum data i nthe given frequency range.
        public float[] GetFrequencyData(int audioSourceIndex, FrequencyRange freqRange, bool useAudioFile)
        {
            if (!audioSources[audioSourceIndex].mute) // if not muted
            {
                Vector2 range = GetFreqForRange(freqRange);
                float fLow = range.x;//Mathf.Clamp (range.x, 20, fMax); // limit low...
                float fHigh = range.y;//Mathf.Clamp (range.y, fLow, fMax); // and high frequencies
                // get spectrum
                float[] freqData = new float[samplesToTake];
                if (useAudioFile)
                {
                    freqData = GetAudioData(audioSourceIndex).GetSamples(freqRange, audioTimer);
                }
                else
                {
                    audioSources[audioSourceIndex].GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris);
                }
                int n1 = (int)Mathf.Floor(fLow * samplesToTake / fMax);
                int n2 = (int)Mathf.Floor(fHigh * samplesToTake / fMax);
                float sum = 0;
                // Debug.Log("Smapling freq: " + n1 + "-" + n2);
                // average the volumes of frequencies fLow to fHigh

                List<float> validData = new List<float>();
                for (int i = n1; i <= n2; i++)
                {
                    //multiply by volume if we're not using mic input, where volume is set to 0 to avoid feedback issues;
                    if (!usingMic)
                    {
                        freqData[i] = freqData[i] * audioSources[audioSourceIndex].volume;
                    }
                    validData.Add(freqData[i]);
                }

                float[] normData = NormalizeArray(validData.ToArray());

                return normData;
            }

            Debug.LogWarning("warning: Audio Source: " + audioSourceIndex + " is muted");
            return new float[samplesToTake];
        }

        //return the raw spectrum data i nthe given frequency range, using the specified number of bins
        public float[] GetFrequencyData(int audioSourceIndex, FrequencyRange freqRange, int numBins, bool abs, bool useAudioFile)
        {
            if (audioSources[audioSourceIndex].mute && !useAudioFile)
            {
                Debug.LogWarning("warning: Audio Source: " + audioSourceIndex + " is muted");
                return new float[numBins];
            }

            Vector2 range = GetFreqForRange(freqRange);
            float fLow = range.x;//Mathf.Clamp (range.x, 20, fMax); // limit low...
            float fHigh = range.y;//Mathf.Clamp (range.y, fLow, fMax); // and high frequencies
            // get spectrum
            float[] freqData = new float[samplesToTake];
            if (useAudioFile)
            {
                freqData = GetAudioData(audioSourceIndex).GetSamples(freqRange, audioTimer);
            }
            else
            {
                audioSources[audioSourceIndex].GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris);
            }
            int n1 = (int)Mathf.Floor(fLow * samplesToTake / fMax);
            int n2 = (int)Mathf.Floor(fHigh * samplesToTake / fMax);
            float sum = 0;
            //Debug.Log("Sampling freq: " + n1 + "-" + n2);
            // average the volumes of frequencies fLow to fHigh

            //Debug.Log("Valid Freq Data: (" + n1 + "-" + n2 + ")/" + samplesToTake);
            string f = "";
            List<float> validData = new List<float>();
            for (int i = n1; i <= n2; i++)
            {
                float frequency = freqData[i];
                if (abs)
                {
                    frequency = Mathf.Abs(freqData[i]);
                }
                //multiply by volume if we're not using mic input, where volume is set to 0 to avoid feedback issues;
                if (!usingMic)
                {
                    frequency = frequency * audioSources[audioSourceIndex].volume;
                }

                validData.Add(frequency);
                f += frequency.ToString() + " , ";
            }

            //Debug.Log("validData: " + f);
            //Debug.Log("NumBins: " + numBins);
            float[] binnedArray = GetBinnedArray(validData.ToArray(), numBins);
            float[] normalizedArray = NormalizeArray(binnedArray);
            return normalizedArray;
            

            
        }

        //take an array, and bin the values. 
        // if numBins is > intput.Length, duplicate input values
        // if numBins is < input.Length, average input values
        float[] GetBinnedArray(float[] input, int numBins)
        {
            float[] output = new float[numBins];

            if(numBins == input.Length)
            {
                return input;
            }
            // if numBins is > intput.Length, duplicate input values
            if(numBins > input.Length)
            {
                int binsPerInput = numBins/input.Length + 1;
                //Debug.Log("BinsPerInput: " + binsPerInput);
                for(int b = 0; b < numBins; b++) 
                {
                    int inputIndex = b/binsPerInput;
                    //Debug.Log( b + "%" + binsPerInput + "=" + inputIndex);
                    output[b] = input[inputIndex];
                }
            }

            // if numBins is < input.Length, downsample the input.
            if (numBins < input.Length)
            {
                int inputsToSkip = input.Length - numBins;
                for (int b = 0; b < numBins; b++)
                { 
                    output[b] = input[b];
                }
            }

            return output;
        }

        //normalize array values to be in the range 0-1
        float[] NormalizeArray(float[] input)
        {
            float[] output = new float[input.Length];
            float max = -Mathf.Infinity;
            //get the max value in the array
            for (int i = 0; i < input.Length; i++)
            {
                max = Mathf.Max(max, Mathf.Abs(input[i]));
            }

            //divide everything by the max value
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i] / max;
            }

            return output;
        }

        public static Vector2 GetFreqForRange(FrequencyRange freqRange)
        {
            switch (freqRange)
            {
                case FrequencyRange.SubBase:
                    return new Vector2(0, 60);
                    //break;
                case FrequencyRange.Bass:
                    return new Vector2(60, 250);
                    //break;
                case FrequencyRange.LowMidrange:
                    return new Vector2(250, 500);
                    //break;
                case FrequencyRange.Midrange:
                    return new Vector2(500, 2000);
                    //break;
                case FrequencyRange.UpperMidrange:
                    return new Vector2(2000, 4000);
                    //break;
                case FrequencyRange.High:
                    return new Vector2(4000, 6000);
                    //break;
                case FrequencyRange.VeryHigh:
                    return new Vector2(6000, 20000);
                    //break;
                case FrequencyRange.Decibal:
                    return new Vector2(0, 20000);
                default:
                    break;
            }

            return Vector2.zero;
        }

        //return the time into the current track
        public float GetAudioTime()
        {
            return audioTimer;
        }

        public AudioData GetAudioData(int index)
        {
            return audioDatas[index];
        }


        //play all the audio sources, if paused, resume from the last play point
        public void Play()
        {
            playing = true;
            foreach(AudioSource source in audioSources)
            {
                source.Play();
            }
        }

        public void PlayFromBeginning()
        {
            Stop();
            Play();
        }


        public void PlayPreBeatSources()
        {
            foreach (AudioSource source in silentAudio.audioSources)
            {
                source.Play();
            }
        }

        //use this instead of Play(), if using a preBeatOffset
        public void PlayWithPreBeatOffset(float delay)
        {
            playing = true;
            PlayPreBeatSources(); // start playing the silent audio
            Invoke("Play", delay); // in "delay" seconds, start playing the regular tracks that aren't muted.
        }
        

        public void SetAudioTime(float time)
        {
            audioTimer = time;
            foreach (AudioSource source in audioSources)
            {
                source.time = audioTimer;
            }
            foreach (AudioSource source in silentAudio.audioSources)
            {
                source.time = audioTimer;
            }
        }

        //pause all the audio
        public void Pause()
        {
            playing = false;
            foreach (AudioSource source in audioSources)
            {
                source.Pause();
            }
        }

        public void Stop()
        {
            playing = false;
            SetAudioTime(0);
            foreach (AudioSource source in audioSources)
            {
                source.Stop();
            }

            //send Stop event to any listeners
            if(OnStop != null)
            {
                OnStop.Invoke();
            }
        }

        //returns if the audio is currently playing or not
        public bool IsPlaying()
        {
            return playing;
        }

        public bool IsPlaying(int index)
        {
            return audioSources[index].isPlaying;
        }
    }

    public enum FrequencyRange
    {
        SubBase, // 20-60 Hz
        Bass, // 60-250 Hz
        LowMidrange, //250-500 Hz
        Midrange, //500-2,000 Hz
        UpperMidrange, //2,000-4,000 Hz
        High, //4,000-6000 Hz
        VeryHigh, //6,000-20,000 Hz
        Decibal // use output data instead of frequency data
    };

    [System.Serializable]
    public class SilentAudio
    {
        [Tooltip("AudioSources that will be sampled, but not heard")]
        public List<AudioSource> audioSources;
        [Tooltip("Use the SilentMixer")]
        public AudioMixerGroup silentMixerGroup;
        [Tooltip("Use SilentAudio?")]
        public bool useSilentAudio = false;
    }

}
