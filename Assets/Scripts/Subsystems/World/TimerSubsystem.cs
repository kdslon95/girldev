using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Events;

namespace Subsystems.World
{
    public class TimerSubsystem : WorldSubsystem
    {
        private class Timer
        {
            private bool isStarted;
            private bool isPaused;
            private bool isTimerValid;
            
            private bool pauseAfterElapsed;
            private bool looping;
            
            private float startTimer;
            private float duration;
            private float currentTime;
        
            private UnityAction onTimerElapsed;

            public Timer(float duration, bool looping, bool startPaused, bool pauseAfterElapsed, UnityAction onTimerElapsed)
            {
                isStarted = false;
                isPaused = startPaused;
                isTimerValid = true;
                this.looping = looping;
                this.pauseAfterElapsed = pauseAfterElapsed;
                
                this.duration = duration;
                this.onTimerElapsed = onTimerElapsed;
            }

            public bool TickTimer()
            {
                if (isPaused)
                {
                    return isTimerValid;
                }

                if (!isStarted)
                {
                    startTimer = Time.time;
                    currentTime = 0f;
                    isStarted = true;
                }
                
                currentTime = (Time.time - startTimer) / duration;
                
                if (currentTime >= 1f)
                {
                    onTimerElapsed?.Invoke();
                    if (looping)
                    {
                        isStarted = false;
                        if (pauseAfterElapsed)
                        {
                            isPaused = true;
                        }
                    }
                    else
                    {
                        isTimerValid = false;
                    }
                }

                return isTimerValid;
            }

            public void InvalidateTimer(bool shouldCallOnTimerElapsed = false)
            {
                if(shouldCallOnTimerElapsed)
                    onTimerElapsed?.Invoke();
                isTimerValid = false;
            }

            public void ResumeTimer()
            {
                isPaused = false;
            }

            public float GetCurrentTime()
            {
                return currentTime;
            }

            public bool IsPaused()
            {
                return isPaused;
            }
        }

        private Dictionary<Guid, Timer> tickingTimers;
        private List<Guid> invalidGuids;
        
        public override void InitializeSubsystem()
        {
            tickingTimers = new Dictionary<Guid, Timer>();
            invalidGuids = new List<Guid>();
        }

        public override void TickSubsystem(float deltaTime)
        {
            foreach (KeyValuePair<Guid, Timer> timerKvp in tickingTimers)
            {
                if (!timerKvp.Value.TickTimer())
                {
                    invalidGuids.Add(timerKvp.Key);
                }
            }

            foreach (Guid guid in invalidGuids)
            {
                tickingTimers.Remove(guid);
            }

            invalidGuids.Clear();
        }

        public override void DisposeSubsystem()
        {
            foreach (Guid timer in tickingTimers.Keys)
            {
                InvalidateTimer(timer);
            }
            
            tickingTimers.Clear();
            invalidGuids.Clear();
        }

        public Guid SetTimer(float duration, UnityAction onTimerElapsed, bool looping = false, bool startPaused = false,
            bool pauseAfterElapsed = false)
        {
            Guid timerId = Guid.NewGuid();
            Timer timer = new Timer(duration, looping, startPaused, pauseAfterElapsed, onTimerElapsed);
            tickingTimers.Add(timerId, timer);
            return timerId;
        }

        //TODO: More validation in the methods below would be nice :)
        
        public void InvalidateTimer(Guid timerId, bool shouldCallAction = false)
        {
            tickingTimers[timerId].InvalidateTimer(shouldCallAction);
        }

        public void ResumeTimer(Guid timerId)
        {
            tickingTimers[timerId].ResumeTimer();
        }

        public float GetCurrentTime(Guid timerId)
        {
            return tickingTimers[timerId].GetCurrentTime();
        }

        public bool IsTimerPaused(Guid timerId)
        {
            return tickingTimers[timerId].IsPaused();
        }
    }
}