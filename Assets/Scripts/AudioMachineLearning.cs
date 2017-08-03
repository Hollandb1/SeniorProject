using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using Accord;
using Accord.MachineLearning;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Accord.Audio;
using Accord.Audio.Formats;
using Accord.MachineLearning;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.Performance;
using Accord.Math;
using Accord.Math.Distances;
using Accord.Math.Optimization.Losses;
using Accord.Statistics;
using Accord.Statistics.Kernels;
using NAudio;
using NAudio.Wave;


class AudioMachineLearning : MonoBehaviour
{
    public Text Screen;
    private const string DESIRED_AUDIO_PATH = "/Users/hollandb1/Documents/Summer17/SeniorProject/Audio/Desired";
    private const string UNDESIRED_AUDIO_PATH = "/Users/hollandb1/Documents/Summer17/SeniorProject/Audio/Undesired";
    private readonly int DESIRED = 1;
    private readonly int UNDESIRED = 0;
    private string _guiText = "";

    private const string TableName = "Audio Table";
    private const string ColumnRootMeanSquare = "Root Mean Square";
    private const string ColumnMax = "Max Peak Value";
    
    private static float TRAINING_PERCENTAGE = 80;
    
    // Tuple arrays for storing the decoded Signal object and the information obtained from the .wav file
    private Accord.Tuple<WaveStream, FileInfo>[] _desiredData;
    private Accord.Tuple<WaveStream, FileInfo>[] _undesiredData;

    private WaveStream[] _desiredAudio;
    private WaveStream[] _undesiredAudio;
    
    private List<int> mTrainingOutput = new List<int>();
    private List<double[]> mTrainingInput = new List<double[]>();
    private List<int> mTestOutput = new List<int>();
    private List<double[]> mTestInput = new List<double[]>();

    private RandomForest _forest;
    private int _trainingIndex = 0;
    private int _correctClassification = 0;
    
    private void Start()
    {
        Run();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Classify();
        }
    }

    private void Classify()
    {
        
        int prediction = -1;
        int expected = -2;

        if (_trainingIndex < mTestInput.Count)
        {
           prediction =  _forest.Decide(mTestInput[_trainingIndex]);
           expected = mTestOutput[_trainingIndex];
            _guiText = String.Format("{0}. OUTPUT: {1}, EXPECTED: {2}", _trainingIndex, GetTag(prediction), GetTag(expected));
            SetScreenText(_guiText);
        }

        if (prediction == expected)
        {
            _correctClassification++;
        }

        _trainingIndex++;

        if (_trainingIndex == mTestInput.Count)
        {
            var percent = ((float) _correctClassification / _trainingIndex) * 100;
            Debug.Log(String.Format("CORRECT: {0}%", percent));
        }

        SecondaryGantry.SetAudioDesired(prediction == 1);
    }


    private void SetScreenText(string value)
    {
        Screen.text = value;
    }


    public void Run() {

        // Access directories containing the training data. 
        var desiredInfo = new DirectoryInfo(DESIRED_AUDIO_PATH);
        var undesiredInfo = new DirectoryInfo(UNDESIRED_AUDIO_PATH);

        // Extracts all the audio data into <FileInfo, Signal> objects.
        _desiredData = DecodeTestData(desiredInfo);
        _undesiredData = DecodeTestData(undesiredInfo);
        _desiredAudio = GetSignalArray(_desiredData);
        _undesiredAudio = GetSignalArray(_undesiredData);

        // Process the signals and set their features. 
        Debug.Log("Processing Desired Audio Signals");
        ProcessSignals(_desiredAudio, DESIRED);
        Debug.Log("Processing Undesired Audio Signals");
        ProcessSignals(_undesiredAudio, UNDESIRED);
        
        double[][] trainingInput = mTrainingInput.ToArray();
        int[] trainingOutput = mTrainingOutput.ToArray();       
        
        Debug.Log(String.Format("Size of Input: {0}", trainingInput.Length));
        DecisionVariable[] variables =
        {
            new DecisionVariable("RMS", DecisionVariableKind.Continuous),
            new DecisionVariable("MAX", DecisionVariableKind.Continuous),
            new DecisionVariable("MOD", DecisionVariableKind.Continuous),
            new DecisionVariable("ENERGY", DecisionVariableKind.Continuous)
        };
        
        // Create the forest learning algorithm
        var teacher = new RandomForestLearning(variables)
        {
            NumberOfTrees = 20, // use 20 trees in the forest
        };

        // Finally, learn a random forest from data
        _forest = teacher.Learn(trainingInput, trainingOutput);
    }

    private string GetTag(int tag)
    {
        switch (tag)
        {
                case 1:
                    return "Desired";
                case 0:
                    return "Undesired";
        }

        return "Error";
    }


    private void ProcessSignals(WaveStream[] signals, int desired)
    {
        
        int maxTrainingVal = (int) Math.Ceiling(signals.Length * ((TRAINING_PERCENTAGE)/ 100));
        Debug.Log(String.Format("Training Value: {0}", maxTrainingVal));
        
        for (int i = 0; i < signals.Length; i++)
        {
            var signal = signals[i];                
            var values = ProcessSignal(signal);
            var rms = values[0];
            var max = values[1];
            var average = values[2];
            var spectral = values[3];
            var energy = values[4];

            var mod = average * (spectral);
            double[] trainOn = {rms, max, mod, energy};
            Debug.Log(String.Format("RMS: {0}\tMAX: {1}\tModified: {2}\tDESIRED: {3}, ENERGY: {4}", rms, max, mod, desired, energy));

            if ( i >= maxTrainingVal)
            {
                mTestInput.Add(trainOn);
                mTestOutput.Add(desired);
            }
            else
            {
                mTrainingInput.Add(trainOn);
                mTrainingOutput.Add(desired);
            }
        }
    }

    private double[] ProcessSignal(WaveStream signal)
    {
        var len = (double) signal.Length;
        var sampleRate = (double) signal.WaveFormat.SampleRate;
        return new double[] {len, sampleRate, 0.0, 0.0, 0.0};
    }

    private Accord.Tuple<WaveStream, FileInfo>[] DecodeTestData(DirectoryInfo inputData) {
        var signalList = new List<Accord.Tuple<WaveStream, FileInfo>>();
        var files = inputData.GetFiles();
        
        foreach (var file in files) 
        {
            if (file.Extension == ".WAV")
            {
                WaveStream stream = new WaveFileReader(file.FullName);
                var tuple = new Accord.Tuple<WaveStream, FileInfo>(stream, file);
                signalList.Add(tuple);
            }
            
        }
        
        return signalList.ToArray();
    }

    private WaveStream[] GetSignalArray(Accord.Tuple<WaveStream, FileInfo>[] list) {
        List<WaveStream> signals = new List<WaveStream>();
        foreach (var tuple in list) {
            var signal = tuple.Item1;
            signals.Add(tuple.Item1);
        }
        return signals.ToArray();
    }
}
