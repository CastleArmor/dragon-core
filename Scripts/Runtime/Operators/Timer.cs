using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    [System.Serializable][InlineProperty]
    public class Timer
    {
        public event Action<Timer> onFinishAny; 


        [SerializeField][HorizontalGroup][HideLabel][SuffixLabel("Dur")]
        private float duration;
        public float Duration {
            get { return duration; }
            set
            {
                if(value < 0)
                {
                    value = 0;
                }

                if (value < elapsedTime)
                {
                    elapsedTime = value;
                    isFinished = true;
                }
                duration = value;

                if(elapsedTime < duration)
                {
                    isFinished = false;
                }
            }
        }
        [SerializeField][HorizontalGroup][HideLabel][SuffixLabel("Ela")]
        private float elapsedTime;
        public float ElapsedTime
        {
            get
            {
                return elapsedTime;
            }
            set
            {
                if(value < 0)
                {
                    value = 0;
                }

                elapsedTime = value;

                if(value > duration)
                {
                    elapsedTime = duration;
                    isFinished = true;
                }

                if (elapsedTime < duration)
                {
                    isFinished = false;
                }
            }
        }

        private bool isFinished;
        public bool IsFinished
        {
            get { return isFinished; }
        }

        /// <summary>
        /// When this property get when it's true, it immediately turns to false.
        /// </summary>
        private bool isFinishedTrigger;
        public bool IsFinishedTrigger
        {
            get {
                bool trigger = isFinishedTrigger;
                if (trigger) isFinishedTrigger = false;

                return trigger; }
        }

        private bool _isFinishedForward;
        public bool IsFinishedForward => _isFinishedForward;
        private bool _isFinishedBackward;
        public bool IsFinishedBackward => _isFinishedBackward;

        [HideLabel][SuffixLabel("Unscaled")][HorizontalGroup]
        public bool isUnscaled;

        public float Percentage
        {
            get
            {
                return elapsedTime / duration;
            }
            set
            {
                elapsedTime = duration * value;
            }
        }

        public float InversePercentage
        {
            get
            {
                return (1 - Percentage);
            }
            set
            {
                Percentage = 1 - value;
            }
        }

        public Timer()
        {
            this.duration = 1;
            Reset();
        }

        public Timer(float duration,bool beginFinished = false)
        {
            this.duration = duration;

            if (!beginFinished)
            {
                Reset();
            }
            else
            {
                elapsedTime = duration;
                isFinished = true;
                isFinishedTrigger = false;
            }
        }

        public void Reset()
        {
            elapsedTime = 0;
            isFinished = false;
            isFinishedTrigger = false;
            _isFinishedForward = false;
            _isFinishedBackward = false;
        }

        /// <summary>
        /// Ticks by timeScale*deltaTime.
        /// </summary>
        /// <param name="timeScale"></param>
        public void Tick(float timeScale)
        {
            if (isFinished) return;
            isFinished = false;
            float deltaTime = isUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsedTime += deltaTime * timeScale;

            if(elapsedTime >= duration)
            {
                elapsedTime = duration;
                isFinished = true;
                isFinishedTrigger = true;
                _isFinishedForward = true;
                onFinishAny?.Invoke(this);
            }
        }
        

        /// <summary>
        /// Ticks by timeScale*fixedDeltaTime.
        /// </summary>
        /// <param name="timeScale"></param>
        public void TickFixed(float timeScale)
        {
            if (isFinished) return;
            isFinished = false;
            float deltaTime = isUnscaled ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
            elapsedTime += deltaTime * timeScale;

            if(elapsedTime >= duration)
            {
                elapsedTime = duration;
                isFinished = true;
                isFinishedTrigger = true;
                _isFinishedForward = true;
                onFinishAny?.Invoke(this);
            }
        }

        public void SetFinishedForward()
        {
            elapsedTime = duration;
            isFinished = true;
            isFinishedTrigger = true;
            _isFinishedForward = true;
        }
        public void SetFinishedBackward()
        {
            elapsedTime = 0;
            isFinished = true;
            isFinishedTrigger = true;
            _isFinishedBackward = true;
        }

        /// <summary>
        /// Goes backward from current progress by timeScale.
        /// </summary>
        /// <param name="timeScale"></param>
        public void Rewind(float timeScale)
        {
            if (isFinished) return;
            elapsedTime -= Time.deltaTime * timeScale;

            isFinished = false;
        
            if (elapsedTime <= 0)
            {
                elapsedTime = 0;
                isFinished = true;
                isFinishedTrigger = true;
                _isFinishedBackward = true;
                onFinishAny?.Invoke(this);
            }
        }
	
    }
}