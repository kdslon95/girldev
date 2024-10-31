using Core;
using UnityEngine;

namespace Subsystems.Persistent
{
    public struct InputState
    {
        public float movementValue;
        public bool isFireDown;
        public bool isFireHeld;
        public bool isWeapon1Down;
        public bool isWeapon2Down;
    }

    public class InputSubsystem : WorldSubsystem
    {
        private InputState inputContext;
        public InputState InputContext => inputContext;

        public override void PreTickSubsystem(float deltaTime)
        {
            inputContext = new InputState();
        }

        public override void TickSubsystem(float deltaTime)
        {
            inputContext.movementValue = Input.GetAxis("Horizontal");
            inputContext.isFireDown = Input.GetButtonDown("Fire");
            inputContext.isFireHeld = Input.GetButton("Fire");
        }
    }
}