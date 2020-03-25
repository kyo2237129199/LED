using Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLed : MonoBehaviour {

    public Text text;
    LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();
    int nResult;
    public InputField ipInput;
    public InputField contentInput;
    public InputField ledHeightInout;
    public InputField ledWidthInput;
    public InputField typeSizeInput;
    string ip_Str;
    int ledHeight;
    int ledWidth;
    int typeSize;
    string content_Str;
    void Start ()
    {

    }

    void Update ()
    {
        ip_Str = ipInput.text;
        ledHeight = int.Parse(ledHeightInout.text);
        ledWidth = int.Parse(ledWidthInput.text);
        typeSize = int.Parse(typeSizeInput.text);
        content_Str = contentInput.text;

    }
    /// <summary>
    /// 设置屏参（只需根据屏的宽高点数的颜色设置一次，发送节目时无需设置）
    /// </summary>
    public void Button1_Click()
    {
        CommunicationInfo.LEDType = 3;
        //TCP通讯********************************************************************************
        CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
        CommunicationInfo.IpStr = ip_Str;//给IpStr赋值LED控制卡的IP
        CommunicationInfo.LedNumber = 1;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值
        //广播通讯********************************************************************************
        //CommunicationInfo.SendType=1;//设为单机直连，即广播通讯无需设LED控制器的IP地址
        //串口通讯********************************************************************************
        //CommunicationInfo.SendType=2;//串口通讯
        //CommunicationInfo.Commport=1;//串口的编号，如设备管理器里显示为 COM3 则此处赋值 3
        //CommunicationInfo.Baud=9600;//波特率
        //CommunicationInfo.LedNumber=1;

        nResult = LedDll.LV_SetBasicInfo(ref CommunicationInfo, 3, ledWidth, ledHeight, 5);
        //设置屏参，屏的颜色为2即为双基色，64为屏宽点数，32为屏高点数，具体函数参数说明见函数声明注示
        print(nResult);
        if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
            text.text = ErrStr;
            // MessageBox.Show(ErrStr);
        }
        else
        {
            text.text = "设置成功";
            // MessageBox.Show("设置成功");
            Debug.Log("设置成功");
        }
    }
    /// <summary>
     /// 一个节目下设置一个连续左移的单行文本区域
     /// </summary>
    public void Button2_Click()
    {
        int hProgram;//节目句柄
        hProgram = LedDll.LV_CreateProgram(64, 32, 3, 5, 0);//根据传的参数创建节目句柄，64是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
                                                            //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败

        nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
        if (nResult != 0)
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
            text.text = ErrStr;
           // MessageBox.Show(ErrStr);
            return;
        }
        LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
        AreaRect.left = 0;
        AreaRect.top = 0;
        AreaRect.width = 64;
        AreaRect.height = 32;

        LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
        FontProp.FontName = "宋体";
        FontProp.FontSize = typeSize;
        FontProp.FontColor = LedDll.COLOR_GREEN;
        FontProp.FontBold = 0;
        //int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

        nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, content_Str, ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示
        // nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.rtf", ref FontProp, 4);//快速通过rtf文件添加一个单行文本区域，函数见函数声明注示
        //nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_FILE, "test.txt", ref FontProp, 4);//快速通过txt文件添加一个单行文本区域，函数见函数声明注示


        nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
        LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
        if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
            text.text = ErrStr;
            // MessageBox.Show(ErrStr);
        }
        else
        {
            Debug.Log("发送成功");
            text.text = "发送成功";
            // MessageBox.Show("发送成功");
        }

    }
    /// <summary>
    /// 一个节目下设置一个多行文本区域
    /// </summary>
    public void Button3_Click()
    {                             

        int hProgram;//节目句柄
        hProgram = LedDll.LV_CreateProgram(64, 32, 3, 5, 0);//根据传的参数创建节目句柄，64是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
                                                            //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败

        nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
        if (nResult != 0)
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
           // MessageBox.Show(ErrStr);
            return;
        }
        LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
        AreaRect.left = 0;
        AreaRect.top = 0;
        AreaRect.width = 64;
        AreaRect.height = 32;

        LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);

        LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
        FontProp.FontName = "宋体";
        FontProp.FontSize = 12;
        FontProp.FontColor = LedDll.COLOR_RED;
        FontProp.FontBold = 0;

        LedDll.PLAYPROP PlayProp = new LedDll.PLAYPROP();
        PlayProp.InStyle = 0;
        PlayProp.DelayTime = 3;
        PlayProp.Speed = 4;
        //可以添加多个子项到图文区，如下添加可以选一个或多个添加
        nResult = LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_STRING, "上海灵信~~~~~", ref FontProp, ref PlayProp, 0, 0);//通过字符串添加一个多行文本到图文区，参数说明见声明注示
        nResult = LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_FILE, "test.rtf", ref FontProp, ref PlayProp, 0, 0);//通过rtf文件添加一个多行文本到图文区，参数说明见声明注示
        nResult = LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_FILE, "test.txt", ref FontProp, ref PlayProp, 0, 0);//通过txt文件添加一个多行文本到图文区，参数说明见声明注示


        nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
        LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
        if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
            // MessageBox.Show(ErrStr);
        }
        else
        {
            Debug.Log("发送成功");
           // MessageBox.Show("发送成功");
        }
    }
    /// <summary>
    /// 一个节目下设置一个图片区(表格的显示通过自绘图片并通过此方式添加发送)
    /// </summary>
    public void Button4_Click()
    {

        int hProgram;//节目句柄
        hProgram = LedDll.LV_CreateProgram(64, 32, 3, 5, 0);//根据传的参数创建节目句柄，64是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
                                                            //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败

        nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
        if (nResult != 0)
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
           // MessageBox.Show(ErrStr);
            return;
        }
        LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
        AreaRect.left = 0;
        AreaRect.top = 0;
        AreaRect.width = 64;
        AreaRect.height = 32;

        LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);


        LedDll.PLAYPROP PlayProp = new LedDll.PLAYPROP();
        PlayProp.InStyle = 0;
        PlayProp.DelayTime = 3;
        PlayProp.Speed = 4;
        //可以添加多个子项到图文区，如下添加可以选一个或多个添加
        //可以添加多个子项到图文区，如下添加可以选一个或多个添加
        nResult = LedDll.LV_AddFileToImageTextArea(hProgram, 1, 1, "test.bmp", ref PlayProp);
        nResult = LedDll.LV_AddFileToImageTextArea(hProgram, 1, 1, "test.jpg", ref PlayProp);
        nResult = LedDll.LV_AddFileToImageTextArea(hProgram, 1, 1, "test.png", ref PlayProp);
        PlayProp.Speed = 3;
        nResult = LedDll.LV_AddFileToImageTextArea(hProgram, 1, 1, "test.gif", ref PlayProp);

        nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
        LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
        if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
            //MessageBox.Show(ErrStr);
        }
        else
        {
            Debug.Log("发送成功");
            //MessageBox.Show("发送成功");
        }
    }
    /// <summary>
    /// 一个节目下设置一个连续左移的单行文本区和一个数字时钟区
    /// </summary>
    public void Button5_Click()
    {

        int hProgram;//节目句柄
        hProgram = LedDll.LV_CreateProgram(64, 32, 3, 5, 0);//根据传的参数创建节目句柄，64是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
                                                            //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败

        nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
        if (nResult != 0)
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
           // MessageBox.Show(ErrStr);
            return;
        }
        LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
        AreaRect.left = 0;
        AreaRect.top = 0;
        AreaRect.width = 64;
        AreaRect.height = 16;

        LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
        FontProp.FontName = "宋体";
        FontProp.FontSize = 12;
        FontProp.FontColor = LedDll.COLOR_RED;
        FontProp.FontBold = 0;
        //int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

        nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, "上海灵信视觉技术股份有限公司", ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示

        AreaRect.left = 0;
        AreaRect.top = 16;
        AreaRect.width = 64;
        AreaRect.height = 16;
        LedDll.DIGITALCLOCKAREAINFO DigitalClockAreaInfo = new LedDll.DIGITALCLOCKAREAINFO();
        DigitalClockAreaInfo.TimeColor = LedDll.COLOR_RED;

        DigitalClockAreaInfo.ShowStrFont.FontName = "宋体";
        DigitalClockAreaInfo.ShowStrFont.FontSize = 12;
        DigitalClockAreaInfo.IsShowHour = 1;
        DigitalClockAreaInfo.IsShowMinute = 1;


        nResult = LedDll.LV_AddDigitalClockArea(hProgram, 1, 2, ref AreaRect, ref DigitalClockAreaInfo);//注意区域号不能一样，详见函数声明注示

        nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
        LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
        if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
            //MessageBox.Show(ErrStr);
        }
        else
        {
            Debug.Log("发送成功");
            //MessageBox.Show("发送成功");
        }
    }
    /// <summary>
    /// 两个节目下个设置一个单行文本去和一个数字时钟区(多节目通过此方法添加)
    /// </summary>
    public void Button6_Click()
    {

        int hProgram;//节目句柄
        hProgram = LedDll.LV_CreateProgram(64, 32, 3, 5, 0);//根据传的参数创建节目句柄，64是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
                                                            //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败

        nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
        if (nResult != 0)
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
           // MessageBox.Show(ErrStr);
            return;
        }
        LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
        AreaRect.left = 0;
        AreaRect.top = 0;
        AreaRect.width = 64;
        AreaRect.height = 16;

        LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
        FontProp.FontName = "宋体";
        FontProp.FontSize = 12;
        FontProp.FontColor = LedDll.COLOR_RED;
        FontProp.FontBold = 0;
        //int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

        nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, "上海灵信视觉技术股份有限公司", ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示

        AreaRect.left = 0;
        AreaRect.top = 16;
        AreaRect.width = 64;
        AreaRect.height = 16;
        LedDll.DIGITALCLOCKAREAINFO DigitalClockAreaInfo = new LedDll.DIGITALCLOCKAREAINFO();
        DigitalClockAreaInfo.TimeColor = LedDll.COLOR_RED;

        DigitalClockAreaInfo.ShowStrFont.FontName = "宋体";
        DigitalClockAreaInfo.ShowStrFont.FontSize = 12;
        DigitalClockAreaInfo.IsShowHour = 1;
        DigitalClockAreaInfo.IsShowMinute = 1;

        nResult = LedDll.LV_AddDigitalClockArea(hProgram, 1, 2, ref AreaRect, ref DigitalClockAreaInfo);//注意区域号不能一样，详见函数声明注示
                                                                                                        ///////////////////////////////////////////////////
        nResult = LedDll.LV_AddProgram(hProgram, 2, 0, 1);

        AreaRect.left = 0;
        AreaRect.top = 0;
        AreaRect.width = 64;
        AreaRect.height = 16;
        FontProp.FontName = "黑体";
        FontProp.FontSize = 12;
        FontProp.FontColor = LedDll.COLOR_RED;
        FontProp.FontBold = 0;
        //int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

        nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 2, 1, ref AreaRect, LedDll.ADDTYPE_STRING, "胡半仙到此一游", ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示

        AreaRect.left = 0;
        AreaRect.top = 16;
        AreaRect.width = 64;
        AreaRect.height = 16;

        DigitalClockAreaInfo.ShowStrFont.FontName = "黑体";
        DigitalClockAreaInfo.ShowStrFont.FontSize = 12;
        DigitalClockAreaInfo.IsShowHour = 1;
        DigitalClockAreaInfo.IsShowMinute = 1;
        DigitalClockAreaInfo.TimeFormat = 2;

        nResult = LedDll.LV_AddDigitalClockArea(hProgram, 2, 2, ref AreaRect, ref DigitalClockAreaInfo);//注意区域号不能一样，详见函数声明注示

        nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
        LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
        if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
        {
            string ErrStr;
            ErrStr = LedDll.LS_GetError(nResult);
            Debug.Log(ErrStr);
            //MessageBox.Show(ErrStr);
        }
        else
        {
            Debug.Log("发送成功");
            //MessageBox.Show("发送成功");
        }
    }
    /// <summary>
    /// 开屏
    /// </summary>
    public void Button7_Click()
    {
        LedDll.LV_PowerOnOff(ref CommunicationInfo, 0);
    }
    /// <summary>
    /// 关屏
    /// </summary>
    public void Button8_Click()
    {
        LedDll.LV_PowerOnOff(ref CommunicationInfo, 1);
    }
    /// <summary>
    /// 设置亮度
    /// </summary>
    public void Button9_Click()
    {
        LedDll.LV_SetBrightness(ref CommunicationInfo,10);
    }
    /// <summary>
    /// 设置LED显示的语言
    /// </summary>
    public void Button10_Click()
    {
        LedDll.LV_SetLanguage(ref CommunicationInfo, 0);
    }
}
