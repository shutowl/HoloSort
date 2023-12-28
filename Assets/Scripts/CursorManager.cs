using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
	public Texture2D[] cursors;	//0 = default, 1 = open, 2 = hold

	public static CursorManager Instance = null;

	// Initialize the singleton instance.
	private void Awake()
	{
		// If there is not already an instance of SoundManager, set it to this.
		if (Instance == null)
		{
			Instance = this;
			//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
			DontDestroyOnLoad(gameObject);
		}
		//If an instance already exists, destroy whatever this object is to enforce the singleton.
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	public void ChangeCursor(string name)
    {
		string cursor = name.ToLower();

        switch (cursor)
        {
			case "open":
				Cursor.SetCursor(cursors[1], new Vector2(cursors[1].width/2, cursors[1].height/2), CursorMode.Auto);
				break;
			case "hold":
			case "held":
				Cursor.SetCursor(cursors[2], new Vector2(cursors[2].width/2, cursors[2].height/2), CursorMode.Auto);
				break;
			default:
				Cursor.SetCursor(cursors[0], Vector2.zero, CursorMode.Auto);
				break;
        }
    }
}
