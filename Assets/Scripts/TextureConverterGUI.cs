using OpenCvSharp.Demo;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using System.IO;

using SFB;

public class TextureConverterGUI : MonoBehaviour
{
    private TextureConverter textureConverter;

    public Text textureInfo;

    void Awake()
    {
        textureConverter = GetComponent<TextureConverter>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clearTextureList()
    {
        Debug.Assert(textureConverter != null);
        textureConverter.clearTextureList();
        textureInfo.text = "../..";
    }

    public void convertTextures()
    {
        Debug.Assert(textureConverter != null);
        textureConverter.convertTextures();
    }

    public void openFileDialog()
    {
        // Open file with filter
        var extensions = new [] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
        };

        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);
        string listOfTexture = "";
        for(int i = 0; i < paths.Length; i++)
        {
                Debug.Log("paths["+i+"]: " + paths[i]);
                listOfTexture += paths[i] + "\n";
        }

        if(paths.Length !=0)
        {
            loadAllTextures(paths);
        }

        textureInfo.text = listOfTexture;
        
    }

    /*private Texture2D LoadTextureFromDisk(string filePath) {
     
         Texture2D tex = null;
         byte[] fileData;
		  bool isLoaded = false;
     
         if (File.Exists(filePath))     {
             fileData = File.ReadAllBytes(filePath);
             tex = new Texture2D(2, 2);
             isLoaded = tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
         }
         return tex;
     }*/

    private void loadAllTextures(string[] paths)
    {
        Debug.Assert(textureConverter != null);

        for(int i = 0; i < paths.Length; i++)
        {
            //Texture2D tex = LoadTextureFromDisk(paths[i]);
            textureConverter.addTextureToList(paths[i]);
        }
    }

    public void conversionPath(GetEnumConversionPath g)
    {
        Debug.Assert(textureConverter != null);
        Debug.Log("Conversion path: " + g.state);
        textureConverter.setConversionPath(g);
    }

    public void filenameStructure(GetEnumFilenameStructure g)
    {
        Debug.Assert(textureConverter != null);
        textureConverter.setFilenameStructure(g);
    }
}
