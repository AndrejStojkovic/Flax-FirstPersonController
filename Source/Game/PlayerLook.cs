using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game
{
    public class PlayerLook : Script
    {
        public Actor Player;
        public Actor CameraTarget;
        public Camera Camera;
        public Vector2 Sensitivity;
        public float ClampX;
        public float CameraSmoothing = 20f;

        public bool Inverted = false;

        private Vector2 rotation;

        public override void OnStart()
        {
            rotation = new Vector2(Actor.Orientation.EulerAngles.X, Actor.Orientation.EulerAngles.Y);
        }

        public override void OnUpdate()
        {
            Screen.CursorVisible = false;
            Screen.CursorLock = CursorLockMode.Locked;

            var x = Input.GetAxisRaw("Mouse X") * Sensitivity.X;
            var y = Input.GetAxisRaw("Mouse Y") * Sensitivity.Y;

            var mouseDelta = new Float2(x, y);
            if(Inverted) {
                mouseDelta.Y *= -1;
            }
            rotation.X = Mathf.Clamp(rotation.X + mouseDelta.Y, -ClampX, ClampX);
            rotation.Y += mouseDelta.X;
        }

        
        public override void OnFixedUpdate() {
            var camera = Camera.Transform;
            var cameraFactor = Mathf.Saturate(CameraSmoothing * Time.DeltaTime);
            CameraTarget.LocalOrientation = Quaternion.Lerp(CameraTarget.LocalOrientation, Quaternion.Euler(rotation.X, 0f, 0f), cameraFactor);
            camera.Translation = Vector3.Lerp(camera.Translation, CameraTarget.Position, cameraFactor);
            camera.Orientation = CameraTarget.Orientation;
            Player.LocalOrientation = Quaternion.Euler(0f, rotation.Y, 0f);
            Camera.Transform = camera;

            // camera.Orientation = Quaternion.Lerp(camera.Orientation, Quaternion.Euler(rotation.X, rotation.Y, 0f), cameraFactor);
            // Camera.Transform = camera;
        }
    }
}
