    public enum ConversionPath{
        
        StandardToRoughness,
        RoughnessToStandard,
        
    }

    public enum FilenameStructure{
        
        Letter,
        Name,
        
    }

namespace OpenCvSharp.Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using System.IO;

    using OpenCvSharp;

    using SFB;



    public class TextureConverter : MonoBehaviour
    {


        string inputTextureFolder = "";
        string outputTextureFolder = "";

        private ConversionPath conversionPath;

        private FilenameStructure filenameStructure;


        private List<string> textureList;

        void Awake()
        {
            textureList = new List<string>();
        }

        //public void addTextureToList(Texture2D texture)
        public void addTextureToList(string texture)
        {
            Debug.Assert(textureList != null);
            textureList.Add(texture);
        }

        public void clearTextureList()
        {
            Debug.Assert(textureList != null);
            textureList.Clear();
        }

        //public void setConversionPath(ConversionPath path)
        public void setConversionPath(GetEnumConversionPath g)
        {
            //conversionPath = path;
            /*public void GetEnumState(GetEnum g)
            { if(g.state == MyEnum.something)
            DoSomething();
            }*/
            /*if(g.state == ConversionPath.StandardToRoughness)
            conversionPath = ConversionPath.StandardToRoughness;*/
            conversionPath = g.state;
        }

        public void setFilenameStructure(GetEnumFilenameStructure g)
        {
            filenameStructure = g.state;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private Texture2D LoadPNG(string filePath) {
     
         Texture2D tex = null;
         byte[] fileData;
		  bool isLoaded = false;
     
         if (File.Exists(filePath))     {
             fileData = File.ReadAllBytes(filePath);
             tex = new Texture2D(2, 2);
             isLoaded = tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
         }
		 //Debug.Log(filePath + " is loaded: " + isLoaded);
         return tex;
        }

        public void savePNG(string filename, string directoryPath, Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();

            //string directoryPath = Path.GetDirectoryName (filename);
            //Debug.Log("directoryPath:" + directoryPath);
            //Debug.Log("filename:" + filename);

            File.WriteAllBytes(directoryPath + "/" + filename, bytes);
            //AssetDatabase.Refresh(ImportAssetOptions.ForceUncompressedImport);
            Debug.Log(texture.format);
        }  

        private Texture2D convertMetalRoughnessToMetalStandard(Texture2D roughnessMetal, Texture2D roughnessRoughness){
                Debug.Assert(roughnessMetal != null);
                Debug.Assert(roughnessRoughness != null);
                Mat matRoughnessMetal = Unity.TextureToMat (roughnessMetal);
                Mat matRoughnessRoughness = Unity.TextureToMat (roughnessRoughness);
                Mat matStandardMetal = new Mat(matRoughnessMetal.Size(), MatType.CV_8UC4, Scalar.Black);

                //take roughness, split it, invert one channel
                Mat[] planesRoughnessRoughness;
                Cv2.Split(matRoughnessRoughness, out planesRoughnessRoughness);
                // Invert G plane
                Cv2.BitwiseNot(planesRoughnessRoughness[1], planesRoughnessRoughness[1]);

                //split roughnessMetal
                Mat[] planesRoughnessMetal;
                Cv2.Split(matRoughnessMetal, out planesRoughnessMetal);

                //keep roughnessMetal channels, plus add invert G of roughness as Alpha
                Mat[] standardMetalPlanes = new Mat[4];
                standardMetalPlanes[0] = planesRoughnessMetal[0];
                standardMetalPlanes[1] = planesRoughnessMetal[1];
                standardMetalPlanes[2] = planesRoughnessMetal[2];
                standardMetalPlanes[3] = planesRoughnessRoughness[1];

                // Merge
                Cv2.Merge(standardMetalPlanes, matStandardMetal);

                return Unity.MatToTexture (matStandardMetal);
            }

        public void convertTextures()
        {
            Debug.Log("Convert Textures, conversion path: " + conversionPath);
            //go from roughness to standard
            if(conversionPath == ConversionPath.RoughnessToStandard)
            {
                Debug.Log("Conversion path = RoughnessToStandard");
                //we work only with textures that have "metal in the name"
                //if(filenameStructure == FilenameStructure.Name)
                {
                    Debug.Log("Number of textures: " + textureList.Count);
                    for(int i = 0; i < textureList.Count; i++)
                    {
                        string textureName = textureList[i];
                        string textureShortFileName = Path.GetFileNameWithoutExtension(textureName);
                        string directoryPath = Path.GetDirectoryName(textureName);

                        //we are looking only for metalness texture, in a Roughness workflow
                        //if(textureName.Contains("_m.") || textureName.Contains("_metal")|| textureName.Contains("-m.") || textureName.Contains("-metal")){
                            if( textureName.Contains("_metal")|| textureName.Contains("-metal")){
                            Texture2D texMetalRoughness = LoadPNG(textureName);

                            //load associated roughness texture
                            //sanity check over 'metalness' first
                            string roughnessTextureName = textureName.Replace("_metalness","_roughness");
                            roughnessTextureName = roughnessTextureName.Replace("_metal","_roughness");
                            
                            Texture2D texRoughnessRoughness = LoadPNG(roughnessTextureName);

                            //convert metal + roughness from Roughness Rendering Mode to Unity Standard Rendering mode
                            Texture2D roughnessStandardMetalTex = convertMetalRoughnessToMetalStandard(texMetalRoughness,texRoughnessRoughness);

                            string newTextureFileName = "";
                            //sanity check over 'metalness', JUST BECAUSE WE ALLOW OTHER NAMES THAT 'METAL', i.e. 'metalness' !
                            if(textureName.Contains("_metalness"))
                            {
                                newTextureFileName = textureShortFileName.Replace("_metalness","_standard_map_metal.png");
                            }
                            if(textureName.Contains("_metal") && !newTextureFileName.Contains("_standard_map_metal"))
                            {
                                newTextureFileName = textureShortFileName.Replace("_metal","_standard_map_metal.png");
                            }                            

                            if(!System.IO.File.Exists(newTextureFileName))
                            {
                                Debug.Log("File doesn't exists: " + roughnessTextureName);
                                savePNG(newTextureFileName, directoryPath, roughnessStandardMetalTex);
                            }
                            else{
                                Debug.Log("File exists already, nothing to write on disk");
                            }
                            
                        }
                    }
                }
            }
        }
    }

}