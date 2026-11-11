using HalconDotNet;
using Measure.Halcon.Demo;
using Measure.Halcon.Demo.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class FileImageSource : IImageSource
{
    private readonly FileInfo[] files;
    private int index;

    public FileImageSource(FileInfo[] files)
    {
        this.files = files;
    }

    public string Name
    {
        get
        {
            if (files == null || files.Length == 0) return "文件:空";
            return "文件:" + files[0].Directory.Name;      // 从第一个文件反推目录名
        }
    }
    public int IntervalMs { get { return 2000; } }
    public bool StopOnError { get { return false; } }     // 坏一张继续

    public OperationResult GrabImage(out HObject image)
    {
        image = null;
        if (files == null || files.Length == 0)
            return OperationResult.CreateFailResult("文件源为空");

        FileInfo file = files[index];
        index = (index + 1) % files.Length;      // 一圈跑完自动回第一张

        try
        {
            HOperatorSet.ReadImage(out image, file.FullName);
            return OperationResult.CreateSuccessResult("读取图像成功：" + file.Name);
        }
        catch (Exception ex)
        {
            image = null;
            return OperationResult.CreateFailResult("读取图像失败：" + file.Name, ex);
        }
    }
}

