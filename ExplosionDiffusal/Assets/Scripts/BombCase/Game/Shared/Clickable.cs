using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClickableType { Keyboard, Keypad, None }
public class Clickable : MonoBehaviour
{
    public ClickableType clickableType;
    public Transform positionWorldSpace;
}
