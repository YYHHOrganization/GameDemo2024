using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using LitJson;

public class Ycsv2cs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // excelToJson();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #if UNITY_EDITOR
    [MenuItem("Tools/ExcelToJson")]
    #endif
    static void excelToJson()
    {
        string dataFolderPath=Application.dataPath+ "/Designer/Designer_Tool/CSV2CS/Document";//csv文件夹路径 应该是在Assets下的Document文件夹
        string outJsonPath=Application.dataPath+ "/Designer/Designer_Tool/CSV2CS/Resources/Json";
        if(!Directory.Exists(dataFolderPath))
        {
            Debug.LogError("请建立"+dataFolderPath+ " 文件夹，并且把csv文件放入此文件夹内");
            return;
        }
        
        string[] allCSVFiles=Directory.GetFiles(dataFolderPath,"*.csv");
        if(allCSVFiles==null||allCSVFiles.Length<=0)
        {
            Debug.LogError(""+dataFolderPath+ " 文件夹没有csv文件,请放入csv文件到此文件夹内");
            return;
        }

        if(!Directory.Exists(outJsonPath))
        {
            Directory.CreateDirectory(outJsonPath);
        }

        for(int i=0;i<allCSVFiles.Length;i++)
        {
            string dictName=new DirectoryInfo(Path.GetDirectoryName(allCSVFiles[i])).Name;
            string fileName=Path.GetFileNameWithoutExtension(allCSVFiles[i]);

            string jsonData=readExcelData(allCSVFiles[i]);
            outJsonContentToFile(jsonData,outJsonPath+"/"+dictName+"/"+fileName+".json");
        }

    }
    
    static void outJsonContentToFile(string jsonData,string jsonFilePath)
    {
        string directName=Path.GetDirectoryName(jsonFilePath);
       
        //如果已经存在 则删除-----
        if(File.Exists(jsonFilePath))
        {
            File.Delete(jsonFilePath);
        }
        
        if(!Directory.Exists(directName))
        {
            Directory.CreateDirectory(directName);
        }
        
        File.WriteAllText(jsonFilePath,jsonData,Encoding.UTF8);
        Debug.Log("成功输出Json文件  :"+jsonFilePath);
        //在Editor模式下重新导入文件数据，刷新。
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    
    static string readExcelData(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return null;
        }
        string fileContent = File.ReadAllText(fileName, UnicodeEncoding.Default);
        string[] fileLineContent = fileContent.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
        string class_name = Path.GetFileNameWithoutExtension(fileName);
        if (fileLineContent != null)
        {
            //注释的名字
            string[] noteContents = fileLineContent[0].Split(new string[] { "," }, System.StringSplitOptions.None);
            //变量的名字
            string[] VariableNameContents = fileLineContent[1].Split(new string[] { "," }, System.StringSplitOptions.None);
            //变量类型的名字
            string[] TypeValue = fileLineContent[2].Split(new string[] { "," }, System.StringSplitOptions.None);

            /*———————————生成CS的Class类脚本————————————*/

            StringBuilder code = new StringBuilder();                //创建代码串
            //添加常见且必须的引用字符串
            code.Append("using UnityEngine; \n");
            code.Append("using System.Collections; \n");
            //产生类，所有可执行代码均在此类中运行
            code.Append("public class Class_" + class_name + " { \n\t");
            for (int i = 0; i < TypeValue.Length; i++)
            {
                code.Append("public string ");
                code.Append(VariableNameContents[i] + " { get; set; } "+ "   //" + noteContents[i] + "\n\t");
                if (TypeValue[i] == "int")
                {
                    code.Append("public int _" + VariableNameContents[i] + " (){\n\t\t");
                    code.Append("int value = int.Parse(" + VariableNameContents[i] + ");\n\t\t");
                    code.Append("return value;\n\t");
                    code.Append("}\n\t");
                }
                else if (TypeValue[i] == "float")
                {
                    code.Append("  public float _" + VariableNameContents[i] + " (){\n\t\t");
                    code.Append("float value = float.Parse(" + VariableNameContents[i] + ");\n\t\t");
                    code.Append("return value;\n\t");
                    code.Append("}\n\t");
                }
                else if (TypeValue[i] == "string")
                {
                    code.Append("  public string _" + VariableNameContents[i] + " (){\n\t\t");
                    code.Append("string value = " + VariableNameContents[i] + ";\n\t\t");
                    code.Append("return value;\n\t");
                    code.Append("}\n\t");
                }
            }
            code.Append("}\n\t");
            string CSharpFilePath = Application.dataPath + "/Designer/Designer_Tool/CSV2CS/Scripts/Script_Doc_CD";
            string directName = Path.GetDirectoryName(CSharpFilePath);
            
            if (!Directory.Exists(directName))
            {
                Directory.CreateDirectory(directName);
            }
            
            //如果已经存在 则删除-----
            if (File.Exists(CSharpFilePath + "/" + "/Class_" + class_name + ".cs"))
            {
                File.Delete(CSharpFilePath + "/" + "/Class_" + class_name + ".cs");
            }
            
            if (!Directory.Exists(CSharpFilePath))
            {
                Directory.CreateDirectory(CSharpFilePath);
            }

            FileStream fs = new FileStream(CSharpFilePath + "/" + "/Class_" + class_name + ".cs", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write(code.ToString());
            sw.Close(); fs.Close();
            //File.WriteAllText(CSharpFilePath + "/" + CDicdictName + "/Class_" + CDicfileName + ".cs", code.ToString(), Encoding.UTF8);
            Debug.Log("成功生成c#的Class文件" + class_name + ".cs" + "在目录:" + CSharpFilePath + " 中");

            /*———————————生成CS的Dictionary类脚本————————————*/

            StringBuilder code_Dic = new StringBuilder();                //创建代码串
            string docPath = "\"Json/Document/" + class_name + "\"";
            //添加常见且必须的引用字符串
            code_Dic.Append("using UnityEngine; \n");
            code_Dic.Append("using System.Collections.Generic; \n");
            //产生类，所有可执行代码均在此类中运行
            code_Dic.Append("public static class SD_" + class_name + " { \n\t");
            code_Dic.Append("public static Dictionary<string, Class_" + class_name + "> Class_Dic = JsonReader.ReadJson<Class_" + class_name + "> (" + docPath + ");\n\t");
            code_Dic.Append("}\n");
            string DicFilePath = Application.dataPath + "/Designer/Designer_Tool/CSV2CS/Scripts/Script_Doc_SD";
            string DicName = Path.GetDirectoryName(DicFilePath);
            if (!Directory.Exists(DicName)) //如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(DicName);
            }
            
            
            //如果已经存在 则删除-----
            if (File.Exists(DicFilePath + "/" + "/SD_" + class_name + ".cs"))
            {
                File.Delete(DicFilePath + "/" + "/SD_" + class_name + ".cs");
            }
            
            
            if (!Directory.Exists(DicFilePath))
            {
                Directory.CreateDirectory(DicFilePath);
            }

            FileStream fs2 = new FileStream(DicFilePath + "/" + "/SD_" + class_name + ".cs", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw2 = new StreamWriter(fs2, Encoding.UTF8);
            sw2.Write(code_Dic.ToString());
            sw2.Close(); fs2.Close();
            //File.WriteAllText(DicFilePath + "/" + DDicdictName + "/SD_" + DDicfileName + ".cs", code_Dic.ToString(), Encoding.UTF8);
            Debug.Log("成功生成c#的Dic文件" + class_name + ".cs" + "在目录:" + DicFilePath + " 中");


            /*————————解析表格字符串————————————*/
            JsonData jsonData = new JsonData();
            // for (int i = 3; i < fileLineContent.Length - 1; i++)
            for (int i = 3; i < fileLineContent.Length; i++)
            {
                string[] lineContents = fileLineContent[i].Split(new string[] { "," }, System.StringSplitOptions.None);
                JsonData classLine = new JsonData();
                for (int j = 1; j < lineContents.Length; j++)
                {
                    classLine[VariableNameContents[j]] = lineContents[j];
                }
                jsonData[lineContents[0]] = classLine;
            }

            string resultJson = jsonData.ToJson();
            return resultJson;
        }
        return null;
    }
}
