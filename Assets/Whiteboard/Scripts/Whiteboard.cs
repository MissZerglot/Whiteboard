using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;
    // texture size set to roughly 0.1, can adjust later for higher resolution
    public Vector2 textureSize = new Vector2(2048, 2048); 

    void Start()
      {
        // Get a renderer to set the resolution to a texture so i can manipulate it
        var r = GetComponent<Renderer>();
        texture = new Texture2D(width: (int)textureSize.x, height: (int)textureSize.y);
        r.material.mainTexture = texture; 

      }


} 