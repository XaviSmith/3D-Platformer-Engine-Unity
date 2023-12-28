//Written from git-amend's platformer tutorial.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
    /// <summary>
    /// Generic C# timer so we don't have to keep making timers for things. Also allows us to call functions when the timer starts and stops.
    /// Includes Countdown timers and Stopwatch Timers below.
    /// <para>See PlayerController in 3DMovement proj as an example on how to use this.</para>
    /// </summary>
    public abstract class Timer
    {
        protected float initialTime; //for countdowns this is how long we intend to run for e.g. 5s, for stopwatches it's just 0
        protected float Time { get; set; } //how much time is left for countdowns, or how long we've been running for stopwatches
        public bool IsRunning { get; protected set; } //is the timer running

        public float Progress => Time / initialTime;

        //Allows us to assign functions to call when the timer starts and stops like OnTimerStart += SomeFunction() in OnEnable/Start.
        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        //Constructor
        protected Timer(float value)
        {
            initialTime = value;
            IsRunning = false;

        }

        //Not to be confused with Unity's Start().
        public void Start()
        {
            Time = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }

        public void Pause() => IsRunning = false;
        public void Resume() => IsRunning = true;

        public abstract void Tick(float deltaTime); //all Tick overrides need to be called by a Monobehaviour's Update(). See 3DMovement's PlayerController for an example.
    }
        /// <summary>
        /// Countdown timer. Inherits from Timer. InitialTime is how long we'll run for, Time is how much time is left.
        /// </summary>
        public class CountdownTimer : Timer
        {
            //Constructor
            public CountdownTimer(float value) : base(value) { }

            public override void Tick(float deltaTime)
            {
                if (IsRunning && Time > 0)
                {
                    Time -= deltaTime;
                }

                if (IsRunning && Time <= 0)
                {
                    Stop();
                }
            }

            //NOTE: Currently unused but may have to set IsFinished in stop.
            public bool IsFinished => Time <= 0; //bool Lambda function as a variable. Checks if time is <= 0

            public void Reset() => Time = initialTime; //Reset our cooldown;

            public void Reset(float newTime) //Reset with a different initialTime
            {
                initialTime = newTime;
                Reset();
            }
        }

    /// <summary>
    /// Stopwatch, inherits from Timer. InitialTime is 0, Time is how long we've been running for.
    /// </summary>
    public class StopwatchTimer : Timer
    {
        //Constructor
        public StopwatchTimer(float value) : base(0) { }

        public override void Tick(float deltaTime)
        {
            if (IsRunning)
            {
                Time += deltaTime;
            }
        }

        public void Reset() => Time = 0;

        public float GetTime() => Time;

        public string GetFormattedTime(){
            float time = Time <= 86399 ? Time : 86399; //prevent overflows
            TimeSpan t = TimeSpan.FromSeconds(time);
            return t.ToString(@"hh\:mm\:ss");
        }
    }
}

