using HalconDotNet;
using Measure.Halcon.Demo.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Measure.Halcon.Demo
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            this.list_Info.Columns[1].Width = list_Info.Width - this.list_Info.Columns[0].Width - 20;
            var halconInitialresult = halconHelper.HalconInitial(hWindowControl1, out HwindowHandle);
            if (halconInitialresult.IsSuccess)
            {
                AddLog(0, "Halcon初始化成功");
            }
            else
            {
                AddLog(2, "Halcon初始化失败"+halconInitialresult.ErrorMsg);
                return;
            }
            if (!image.IsCameraOpen())
            {
                var openCamresult = image.OpenCam("GigEVision2", "ssss");
                if (openCamresult.IsSuccess)
                {

                    AddLog(0, "相机打开成功");
                    this.tb_ExposureTime.Value=image.GetExposureTime();
                    this.txt_ExposureTime.Text=image.GetExposureTime().ToString();
                }
                else
                {
                    AddLog(2, "相机打开失败" + openCamresult.ErrorMsg);
                    return;

                }
            }
            
        }

        private ImageHelper image = new ImageHelper();

        private HTuple HwindowHandle = new HTuple();

        private HObject HImage = new HObject();

        private HalconHelper halconHelper = new HalconHelper();


        private void btn_CreatShapeModel_Click(object sender, EventArgs e)
        {

        }

        private void btn_OpenImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "BMP 图片(*.bmp)|*.bmp|PNG 图片(*.png)|*.png|JPEG 图片(*.jpg;*.jpeg)|*.jpg;*.jpeg|TIFF 图片(*.tif;*.tiff)|*.tif;*.tiff";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    string imagePath = openFileDialog.FileName;
                    var result = image.ReadImage(imagePath, ref HwindowHandle, ref HImage);
                    if (result.IsSuccess)
                    {
                        AddLog(0, "打开图像成功");
                    }
                    else
                    {
                        AddLog(2, "打开图像失败" + result.ErrorMsg);
                    }


                }
            }
        }

        private void btn_SaveImage_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "BMP 图片(*.bmp)|*.bmp|PNG 图片(*.png)|*.png|JPEG 图片(*.jpg;*.jpeg)|*.jpg;*.jpeg|TIFF 图片(*.tif;*.tiff)|*.tif;*.tiff";
                saveFileDialog.DefaultExt = "bmp";
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 格式不用再传了，SaveImage 内部从 FileName 的后缀名取
                    var result = image.SaveImage(saveFileDialog.FileName, HImage);
                    if (result.IsSuccess)
                    {
                        AddLog(0, "保存图像成功");
                    }
                    else
                    {
                        AddLog(2, "保存图像失败"+result.ErrorMsg);
                    }
                }
            }
        }

        private void btn_OneShot_Click(object sender, EventArgs e)
        {
            var result= image.GrabImage(ref HImage, ref HwindowHandle);
            if (result.IsSuccess)
            {
                AddLog(0, "单帧采图成功");
            } 
            else 
                {
                    AddLog(2, "单帧采图失败" + result.ErrorMsg);
                }

        }

        private string CurrentTime
        {
            get { return DateTime.Now.ToString("T"); }
        }
        private void AddLog(int index, string log)
        {
            if (list_Info.InvokeRequired)
            {
                this.list_Info.Invoke(new Action(()=>{
                    ListViewItem listViewItem = new ListViewItem("   " + CurrentTime, index);
                    listViewItem.SubItems.Add(log);
                    this.list_Info.Items.Insert(0, listViewItem);

                }));

            }
            else
            {
                ListViewItem listViewItem = new ListViewItem("   " + CurrentTime, index);
                listViewItem.SubItems.Add(log);
                this.list_Info.Items.Insert(0, listViewItem);

            }

        }

        private void list_Info_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
