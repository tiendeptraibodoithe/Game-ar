// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using UnityEngine.EventSystems;
using Mediapipe.Tasks.Vision.HandLandmarker;
using System.Collections.Generic;
using Mediapipe.Tasks.Components.Containers;
namespace Mediapipe.Unity
{
    public class HandLandmarkerResultAnnotationController : AnnotationController<MultiHandLandmarkListAnnotation>
    {
        [SerializeField] private bool _visualizeZ = false;

        // ← NEW: assign your AR Camera (the one rendering the Image Target)
        [SerializeField] private Camera _arCamera;
        // ← NEW: assign your Vuforia Image Target GameObject here
        [SerializeField] private GameObject _imageTarget;

        private readonly object _currentTargetLock = new object();
        private HandLandmarkerResult _currentTarget;
        private bool _hasClicked = false;

        /// <summary>
        /// ← NEW: expose the latest detected hand-landmarks
        /// </summary>
        /// 



        public IReadOnlyList<NormalizedLandmarks> CurrentLandmarks
     {
         get
         {
             lock (_currentTargetLock)
             {
                 return _currentTarget.handLandmarks;
            }
        }
     }

        public void DrawNow(HandLandmarkerResult target)
        {
            target.CloneTo(ref _currentTarget);
            SyncNow();
            CheckForImageTargetClick();
        }

        public void DrawLater(HandLandmarkerResult target) => UpdateCurrentTarget(target);

        protected void UpdateCurrentTarget(HandLandmarkerResult newTarget)
        {
            lock (_currentTargetLock)
            {
                newTarget.CloneTo(ref _currentTarget);
                isStale = true;
            }
        }

        protected override void SyncNow()
        {
            lock (_currentTargetLock)
            {
                isStale = false;
                annotation.SetHandedness(_currentTarget.handedness);
                annotation.Draw(_currentTarget.handLandmarks, _visualizeZ);
            }
        }

        /// <summary>
        /// ← NEW: Cast a ray from the index-finger tip into the AR scene and
        /// simulate a pointer click if it hits the Image Target.
        /// </summary>
        private void CheckForImageTargetClick()
        {
            var landmarks = CurrentLandmarks;
            if (landmarks == null || landmarks.Count == 0)
            {
                _hasClicked = false;
                return;
            }

            // index-finger tip is landmark #8
            var indexTip = landmarks[0].landmarks[8];

            // normalized coordinates → screen pixels
            var screenPos = new Vector2(
      indexTip.x * Screen.width,
      (1 - indexTip.y) * Screen.height
  );

            // raycast into the AR world
            var ray = _arCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit) && hit.transform.gameObject == _imageTarget)
            {
                if (!_hasClicked)
                {
                    _hasClicked = true;
                    // build a minimal PointerEventData
                    var pointerData = new PointerEventData(EventSystem.current)
                    {
                        position = screenPos
                    };
                    // execute any IPointerClickHandler on the target
                    ExecuteEvents.Execute(
                        _imageTarget,
                        pointerData,
                        ExecuteEvents.pointerClickHandler
                    );
                }
            }
            else
            {
                _hasClicked = false;
            }
        }
    }
}
