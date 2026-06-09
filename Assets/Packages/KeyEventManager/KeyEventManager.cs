using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class KeyEventManager : MonoBehaviour
{
    public enum KeyInputType
    {
        WasPressedThisFrame,
        WasReleasedThisFrame,
        IsPressed,
    }

    [System.Serializable]
    public class KeyEvent
    {
        public Key                  key;
        public KeyInputType         inputType;
        public UnityEvent<Keyboard> handler;
    }

    [field: SerializeField] public List<KeyEvent> KeyEvents { get; private set; }

    private void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        foreach (var keyEvent in from keyEvent in KeyEvents let satisfied = keyEvent.inputType switch
        {
            KeyInputType.WasPressedThisFrame  => keyboard[keyEvent.key].wasPressedThisFrame,
            KeyInputType.WasReleasedThisFrame => keyboard[keyEvent.key].wasReleasedThisFrame,
            KeyInputType.IsPressed            => keyboard[keyEvent.key].isPressed,
            _                                 => false
        } where satisfied select keyEvent)
        {
            keyEvent.handler.Invoke(keyboard);
        }
    }

    public bool TryGetValue(Key key, out KeyEvent keyEvent)
    {
        keyEvent = null;

        foreach (var e in KeyEvents.Where(e => e.key == key))
        {
            keyEvent = e;
            return true;
        }

        return false;
    }

    public static void ApplicationQuit()
    {
        Application.Quit();
    }
}