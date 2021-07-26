﻿using UnityEngine;
using System;

namespace Mara.MrTween {
    /// <summary>
    /// ActionTasks let you pass in an Action that will be called at different intervals depending on how you set it up.
    /// Note that all ActionTask static constructor methods will automatically cache the ActionTasks for easy reuse. Also note
    /// that the real trick to this class is to pass in a context object that you use in the Action when it is called. That is how
    /// you avoid allocations when using anonymous Actions.
    ///
    /// All of the ITweenable methods apply here so you can pause/resume/stop the ActionTask at any time.
    /// </summary>
    public class ActionTask : AbstractTweenable {
        /// <summary>
        /// provides access to the context for this task
        /// </summary>
        /// <value>The context.</value>
        public object Context { get; private set; }

        /// <summary>
        /// provides the elapsed time not included the initial delay that this task has been running
        /// </summary>
        /// <value>The elapsed time.</value>
        public float ElapsedTime { get { return _unfilteredElapsedTime; } }

        Action<ActionTask> _action;
        float _unfilteredElapsedTime;
        float _elapsedTime;
        float _initialDelay = 0f;
        float _repeatDelay = 0f;
        bool _repeats = false;
        bool _isTimeScaleIndependent = false;

        ActionTask _continueWithTask;
        ActionTask _waitForTask;


        #region static convenience constructors

        /// <summary>
        /// creates an ActionTask but does not start it!
        /// </summary>
        /// <param name="action">Action.</param>
        public static ActionTask Create(Action<ActionTask> action) {
            return QuickCache<ActionTask>.Pop()
                .SetAction(action);
        }


        /// <summary>
        /// creates an ActionTask with a context but does not start it!
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="action">Action.</param>
        public static ActionTask Create(object context, Action<ActionTask> action) {
            return QuickCache<ActionTask>.Pop()
                .SetAction(action)
                .SetContext(context);
        }


        /// <summary>
        /// calls the Action every repeatsDelay seconds. The ActionTask is automatically started for you.
        /// </summary>
        /// <param name="initialDelay">Initial delay.</param>
        /// <param name="repeatDelay">Repeat delay.</param>
        /// <param name="context">Context.</param>
        /// <param name="action">Action.</param>
        public static ActionTask Every(float repeatDelay, object context, Action<ActionTask> action) {
            var task = QuickCache<ActionTask>.Pop()
                .SetAction(action)
                .SetRepeats(repeatDelay)
                .SetContext(context);
            task.Start();

            return task;
        }


        /// <summary>
        /// calls the Action every repeatsDelay seconds after the initial delay. The ActionTask is automatically started for you.
        /// </summary>
        /// <param name="initialDelay">Initial delay.</param>
        /// <param name="repeatDelay">Repeat delay.</param>
        /// <param name="context">Context.</param>
        /// <param name="action">Action.</param>
        public static ActionTask Every(float initialDelay, float repeatDelay, object context, Action<ActionTask> action) {
            var task = QuickCache<ActionTask>.Pop()
                .SetAction(action)
                .SetRepeats(repeatDelay)
                .SetContext(context)
                .SetDelay(initialDelay);
            task.Start();

            return task;
        }


        /// <summary>
        /// calls the action after an initial delay. The ActionTask is automatically started for you.
        /// </summary>
        /// <param name="initialDelay">Initial delay.</param>
        /// <param name="context">Context.</param>
        /// <param name="action">Action.</param>
        public static ActionTask AfterDelay(float initialDelay, object context, Action<ActionTask> action) {
            var task = QuickCache<ActionTask>.Pop()
                .SetAction(action)
                .SetDelay(initialDelay)
                .SetContext(context);
            task.Start();

            return task;
        }

        #endregion


        // paramaterless constructor for use with QuickCache
        public ActionTask() { }


        #region AbstractTweenable

        public override bool Tick() {
            // if we have a waitForTask we dont do anything until it completes
            if (_waitForTask != null) {
                if (_waitForTask.IsRunning())
                    return false;
                _waitForTask = null;
            }

            if (_isPaused)
                return false;


            var deltaTime = _isTimeScaleIndependent ? Time.unscaledDeltaTime : Time.deltaTime;

            // handle our initial delay first
            if (_initialDelay > 0f) {
                _initialDelay -= deltaTime;

                // catch the overflow if we have any. if we end up less than 0 while decrementing our initial delay we make that our elapsedTime
                // so that the Action gets called and so that we keep time accurately.
                if (_initialDelay < 0f) {
                    _elapsedTime = -_initialDelay;
                    _action(this);

                    // if we repeat continue on. if not, then we end things here
                    if (_repeats) {
                        return false;
                    } else {
                        // all done. run the continueWith if we have one
                        if (_continueWithTask != null)
                            _continueWithTask.Start();

                        // if stop was called on this ActionTask we need to be careful that we don't return true which will tell MrTween
                        // to remove the task while it is iterating it's list of tweens causing bad things to happen.
                        if (_isCurrentlyManagedByMrTween) {
                            _isCurrentlyManagedByMrTween = false;
                            return true;
                        } else {
                            return false;
                        }
                    }
                } else {
                    return false;
                }
            }


            // done with initial delay. now we either tick the Action every frame or use the repeatDelay to delay calls to the Action
            if (_repeatDelay > 0f) {
                if (_elapsedTime > _repeatDelay) {
                    _elapsedTime -= _repeatDelay;
                    _action(this);
                }
            } else {
                _action(this);
            }

            _unfilteredElapsedTime += deltaTime;
            _elapsedTime += deltaTime;

            return false;
        }


        /// <summary>
        /// stops the task optionally running the continueWith task if it is present
        /// </summary>
        /// <param name="runContinueWithTaskIfPresent">If set to <c>true</c> run continue with task if present.</param>
        public override void Stop(bool runContinueWithTaskIfPresent = true, bool bringToCompletionImmediately = false) {
            if (runContinueWithTaskIfPresent && _continueWithTask != null)
                _continueWithTask.Start();

            // call base AFTER we do our thing since it will recycle us
            base.Stop();
        }


        public override void RecycleSelf() {
            _unfilteredElapsedTime = _elapsedTime = _initialDelay = _repeatDelay = 0f;
            _isPaused = _isCurrentlyManagedByMrTween = _repeats = _isTimeScaleIndependent = false;
            Context = null;
            _action = null;
            _continueWithTask = _waitForTask = null;

            QuickCache<ActionTask>.Push(this);
        }

        #endregion


        #region Configuration

        /// <summary>
        /// sets the Action to be called
        /// </summary>
        /// <param name="action">Action.</param>
        public ActionTask SetAction(Action<ActionTask> action) {
            _action = action;
            return this;
        }


        /// <summary>
        /// Sets the delay before the Action is called
        /// </summary>
        /// <returns>The delay.</returns>
        /// <param name="delay">Delay.</param>
        public ActionTask SetDelay(float delay) {
            _initialDelay = delay;
            return this;
        }


        /// <summary>
        /// tells this action to repeat. It will repeat every frame unless a repeatDelay is provided
        /// </summary>
        /// <returns>The repeats.</returns>
        /// <param name="repeatDelay">Repeat delay.</param>
        public ActionTask SetRepeats(float repeatDelay = 0f) {
            _repeats = true;
            _repeatDelay = repeatDelay;
            return this;
        }


        /// <summary>
        /// allows you to set any object reference retrievable via tween.context. This is handy for avoiding
        /// closure allocations for completion handler Actions. You can also search MrTween for all tweens with a specific
        /// context.
        /// </summary>
        /// <returns>The context.</returns>
        /// <param name="context">Context.</param>
        public ActionTask SetContext(object context) {
            this.Context = context;
            return this;
        }


        /// <summary>
        /// sets the task to use Time.unscaledDeltaTime instead of Time.deltaTime
        /// </summary>
        /// <returns>ActionTask</returns>
        public ActionTask SetIsTimeScaleIndependent() {
            _isTimeScaleIndependent = true;
            return this;
        }

        #endregion


        #region Interation with other ActionTasks

        /// <summary>
        /// when this ActionTask completes the ActionTask passed into continueWith will be started. Note that the continueWith task should not
        /// be running so it should be created with one of the ActionTask.create variants (which don't start the task automatically).
        /// </summary>
        /// <returns>The with.</returns>
        /// <param name="actionTask">Action task.</param>
        public ActionTask ContinueWith(ActionTask actionTask) {
            if (actionTask.IsRunning())
                Debug.LogError("Attempted to continueWith an ActionTask that is already running. You can only continueWith tasks that have not started yet");
            else
                _continueWithTask = actionTask;

            return this;
        }


        /// <summary>
        /// the current task will halt execution until the ActionTask passed into waitFor completes. Note that it must be an already running task!
        /// </summary>
        /// <returns>The for.</returns>
        /// <param name="actionTask">Action task.</param>
        public ActionTask WaitFor(ActionTask actionTask) {
            if (!actionTask.IsRunning())
                Debug.LogError("Attempted to waitFor an ActionTask that is not running. You can only waitFor tasks that are already running.");
            else
                _waitForTask = actionTask;

            return this;
        }

        #endregion

    }
}