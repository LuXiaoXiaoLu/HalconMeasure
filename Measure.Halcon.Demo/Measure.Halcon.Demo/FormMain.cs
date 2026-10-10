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
                openFileDialog.Filter = ".bmp|*.bmp|.png|*.png|.jpg|*.jpg|.tif|*.tif|.jpeg|*.jpeg";
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
                saveFileDialog.Filter= ".bmp|*.bmp|.png|*.png|.jpg|*.jpg|.jpeg|*.jpeg";
                saveFileDialog.DefaultExt = ".bmp";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var result = image.SaveImage(saveFileDialog.FileName,saveFileDialog.FileName.Substring(saveFileDialog.FileName.IndexOf('.')+1), HImage);
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
    }
}
