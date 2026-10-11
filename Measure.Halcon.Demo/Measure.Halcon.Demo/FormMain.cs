using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            this.HwindowHandle = this.hWindowControl1.HalconWindow;
            var result = image.OpenCam("ssss", "GigEVision2");
            if (result.IsSuccess) {
                MessageBox.Show("相机打开成功");
            }
            else
            {
                MessageBox.Show("相机打开失败");
            }
        }

        private ImageHelper image=new ImageHelper();

        private HTuple HwindowHandle=new HTuple();

        private HObject HImage=new HObject();


        private void btn_CreatShapeModel_Click(object sender, EventArgs e)
        {

        }

        private void btn_OpenImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog  openFileDialog=new   OpenFileDialog())
            {
                openFileDialog.Filter = "BMP 图片(*.bmp)|*.bmp|PNG 图片(*.png)|*.png|JPEG 图片(*.jpg;*.jpeg)|*.jpg;*.jpeg|TIFF 图片(*.tif;*.tiff)|*.tif;*.tiff";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK) { 
                    string imagePath=openFileDialog.FileName;
                   var result= image.ReadImage(imagePath,ref HwindowHandle, ref HImage);
                if (result.IsSuccess == false)
                    {
                        MessageBox.Show("打开图像失败："+result.ErrorMsg);
                    }
                
                
                }
            }
        }

        private void btn_SaveImage_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog=new SaveFileDialog())
            {
                saveFileDialog.Filter = "BMP 图片(*.bmp)|*.bmp|PNG 图片(*.png)|*.png|JPEG 图片(*.jpg;*.jpeg)|*.jpg;*.jpeg|TIFF 图片(*.tif;*.tiff)|*.tif;*.tiff";
                saveFileDialog.DefaultExt = "bmp";
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 格式不用再传了，SaveImage 内部从 FileName 的后缀名取
                    var result = image.SaveImage(saveFileDialog.FileName, HImage);
                    if(result.IsSuccess )
                    {
                        MessageBox.Show("保存图像成功");
                    }
                    else
                    {
                        MessageBox.Show("保存图像失败:" + result.ErrorMsg);
                    }
                }
            }
        }

        private void btn_OneShot_Click(object sender, EventArgs e)
        {
            image.GrabImage(ref HImage,ref HwindowHandle);
        }
    }
}
