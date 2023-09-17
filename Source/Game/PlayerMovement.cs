using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game {
    public class PlayerMovement : Script {
        public Actor Player;
        public CharacterController PlayerController;
        public Camera Camera;

        [Header("Controller")]
        public float StandingHeight;
        public float CameraOffsetY = 0f;

        [Header("Movement")]
        public float WalkSpeed;
        public float RunSpeed;
        public float MoveSmoothTime;

        [Header("Crouch")]
        public float CrouchHeight;
        public float CrouchSpeed;
        public bool CrouchToggle = false;
        public bool isCrouching;

        [Header("Jump")]
        public float JumpStrength;
        public float Gravity;

        private Vector3 velocity;
        private Vector3 currentForceVelocity;
        private Vector3 moveDampVelocity;

        public override void OnStart()
        {
            if(PlayerController == null) {
                PlayerController = Actor.GetScript<CharacterController>();
            }

            SetCameraHeight();
        }
        
        public override void OnFixedUpdate()
        {
            HandleCrouch();
            HandleMovement();
        }

        protected void HandleMovement()
        {
            Vector3 PlayerInput = new Vector3 {
                X = Input.GetAxisRaw("Horizontal"),
                Y = 0f,
                Z = Input.GetAxisRaw("Vertical")
            };

            PlayerInput.Normalize();

            
            Vector3 Move = Actor.Transform.TransformDirection(PlayerInput);
            float CurrentSpeed = 0f;
            

            if(isCrouching) {
                SetCapsuleHeight(CrouchHeight);
                CurrentSpeed = CrouchSpeed;
            } else {
                SetCapsuleHeight(StandingHeight);
                CurrentSpeed = Input.GetKey(KeyboardKeys.Shift) ? RunSpeed : WalkSpeed;
            }

            SetCameraHeight();

            CurrentSpeed *= 100;

            velocity = Utils.SmoothDamp(
                velocity,
                Move * CurrentSpeed,
                ref moveDampVelocity,
                MoveSmoothTime
            );

            PlayerController.Move(velocity * Time.DeltaTime);

            // TO-DO: Implement Gravity and Jumping
            // PlayerController.Move(currentForceVelocity * Time.DeltaTime);
        }

        protected void HandleCrouch()
        {
            if(CrouchToggle) {
                if(Input.GetKeyDown(KeyboardKeys.Control)) {
                    isCrouching = !isCrouching;
                }
            } else {
                if(Input.GetKeyDown(KeyboardKeys.Control)) {
                    isCrouching = true;
                } else if(Input.GetKeyUp(KeyboardKeys.Control)) {
                    isCrouching = false;
                }
            }
        }

        protected void SetCapsuleHeight(float value)
        {
            PlayerController.Center = new Vector3(0f, (value / 2f) + PlayerController.Radius, 0f);
            PlayerController.Height = value;
        }

        protected void SetCameraHeight()
        {
            float height = isCrouching ? CrouchHeight : StandingHeight;
            Camera.LocalPosition = new Vector3(0f, height + PlayerController.Radius + CameraOffsetY, 0f);
        }
    }
}
