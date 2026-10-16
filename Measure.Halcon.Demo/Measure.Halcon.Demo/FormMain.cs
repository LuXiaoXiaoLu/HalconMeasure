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
using System.Threading;
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
                var openCamresult = image.OpenCam(CamName, "ssss");
                if (openCamresult.IsSuccess)
                {

                    AddLog(0, "相机打开成功");

                    // 每个参数只读一次，别对相机重复提问
                    double exposure = image.GetExposureTime();
                    double gain = image.GetGain();

                    // 文本框显示真实值（保留小数），它才是"准"的那个
                    txt_ExposureTime.Text = exposure.ToString("F1");
                    txt_Gain.Text = gain.ToString("F1");

                    // TrackBar 天生只能表示整数，这里只做近似定位，不改动真实值
                    tb_ExposureTime.Value = ToTrackBarValue(exposure, tb_ExposureTime);
                    tb_Gain.Value = ToTrackBarValue(gain, tb_Gain);

                    chk_Trigger.Checked = image.GetTriggerMode();
                }
                else
                {
                    AddLog(2, "相机打开失败" + openCamresult.ErrorMsg);
                    return;

                }
            }
            
        }

        private ImageHelper image = new ImageHelper();

        private const string CamName = "GigEVision2";

        private HTuple HwindowHandle = new HTuple();

        private HObject HImage = new HObject();

        private HalconHelper halconHelper = new HalconHelper();

        //连续采图标志位
        private CancellationTokenSource cts;


        /// <summary>
        /// 把相机返回的真实数值换算成 TrackBar 能接受的整数。
        /// TrackBar 只能表示整数，且 Value 必须在 Minimum~Maximum 之间（越界会抛异常），
        /// 所以超出量程时夹到边界上。这里只解决"能显示"，不追求精确。
        /// </summary>
        private static int ToTrackBarValue(double value, TrackBar bar)
        {
            int v = (int)Math.Round(value);
            if (v < bar.Minimum) return bar.Minimum;
            if (v > bar.Maximum) return bar.Maximum;
            return v;
        }


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

        private void chk_Trigger_CheckedChanged(object sender, EventArgs e)
        {
            if (!image.SetTriggerMode(chk_Trigger.Checked).IsSuccess){
                AddLog(1, "触发设置失败");
            }
            ;
        }

        private void tb_ExposureTime_Scroll(object sender, EventArgs e)
        {
            this.tb_ExposureTime.Focus();
            if (image.SetExposureTime(tb_ExposureTime.Value).IsSuccess)
            {
                this.txt_ExposureTime.Text= tb_ExposureTime.Value.ToString  ();
            }
            else
            {
                AddLog(1, "曝光时间设置失败");
            }
        }

        private void tb_Gain_Scroll(object sender, EventArgs e)
        {
            this.tb_Gain.Focus();
            if (image.SetGain(tb_Gain.Value).IsSuccess)
            {
                this.txt_Gain.Text = tb_Gain.Value.ToString();
            }
            else
            {
                AddLog(1, "增益设置失败");
            }
        }

        private void btn_Grab_Click(object sender, EventArgs e)
        {
            if (!image.IsCameraOpen())
            {
                var openCamresult = image.OpenCam(CamName, "ssss");
                if (openCamresult.IsSuccess)
                {

                    AddLog(0, "相机打开成功");

                    // 每个参数只读一次，别对相机重复提问
                    double exposure = image.GetExposureTime();
                    double gain = image.GetGain();

                    // 文本框显示真实值（保留小数），它才是"准"的那个
                    txt_ExposureTime.Text = exposure.ToString("F1");
                    txt_Gain.Text = gain.ToString("F1");

                    // TrackBar 天生只能表示整数，这里只做近似定位，不改动真实值
                    tb_ExposureTime.Value = ToTrackBarValue(exposure, tb_ExposureTime);
                    tb_Gain.Value = ToTrackBarValue(gain, tb_Gain);

                    chk_Trigger.Checked = image.GetTriggerMode();
                }
                else
                {
                    AddLog(2, "相机打开失败" + openCamresult.ErrorMsg);
                    return;

                }
            }

            cts= new CancellationTokenSource();

            Task.Run(() =>
            {
                GrabImageThread();

            },cts.Token);
            AddLog(0, "开始连续采图");

        }

        private void GrabImageThread()
        {
            while (cts.IsCancellationRequested)
            {
                var result = image.GrabImage(ref HImage, ref HwindowHandle);

                if (image.IsCameraOpen()) {
                    if (result.IsSuccess)
                    {
                        AddLog(0, "单帧采图成功");
                    }
                    else
                    {
                        AddLog(2, "采图失败" + result.ErrorMsg);
                        break;
                    }
                }
                else {
                    break;
                }
                 


            }
        }

        private void btn_StopGrab_Click(object sender, EventArgs e)
        {
            AddLog(0, "停连续采图");
            cts?.Cancel();  
        }
    }
}
