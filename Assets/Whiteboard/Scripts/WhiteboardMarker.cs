using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class WhiteboardMarker : MonoBehaviour
{
    // Setting up the marker's tip + size. This can be used later to create a slider scale if need be. 
    [SerializeField] private Transform _tip;
    [SerializeField] private int _penSize = 5;

    //gain access to material colour 
    private Renderer _renderer; 
    // create an array of colours to change each pixel in a square to that colour, this way you can colour a square of pixels at once
    private Color[] _colors; 
     // Check if my pen is actually touching the keyboard
    private float _tipHeight; 

    private RaycastHit _touch; 
    private Whiteboard _whiteboard;
    private Vector2 _touchPos, _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot; 

    void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        // This calculation of pensize * pensize makes our tip square. 
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
        _tipHeight = _tip.localScale.y; 
    }

    // Update is called once per frame
    void Update()
    {
        //Check resolution + check if the tip is touching the whiteboard + perform a little interpolation so the tip streaks instead of leaving only dots
        Draw(); 
    }

    private void Draw()
    {
        // if we actually hit something with the tip of the marker, execute... 
        if (Physics.Raycast(_tip.position, transform.up, out _touch, _tipHeight))
        {
            //if we hit the whiteboard
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                // Cache whiteboard script
                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>(); 
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                // time to do a little math and see where exactly on the whiteboard we're drawing (kind of important) - transferring from a float to a pixel \
                var x = (int)(_touchPos.x * _whiteboard.textureSize.x - (_penSize / 2));
                var y = (int)(_touchPos.y * _whiteboard.textureSize.y - (_penSize / 2));

                // Do an out-of-bound check on the whiteboard, checking if the x and y of the penciltip exceed the maximum values of x and y on the whiteboard
                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x) return; 
                
                if (_touchedLastFrame)
                {
                    _whiteboard.texture.SetPixels(x, y, _penSize, _penSize, _colors);

                    // time to interpolate that line
                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _whiteboard.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors );
                    }

                    // Locking the rotation of the pen when it touches the whiteboard so it doesn't smash into the whiteboard and get stuck to it
                    transform.rotation = _lastTouchRot;

                    _whiteboard.texture.Apply(); 
                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return; 
            }
        }
        // When we are no longer touching the whiteboard, these two statements will unset the touchLastFrame so we don't continually loop through the above 
        _whiteboard = null;
        _touchedLastFrame = false; 
    }

}
