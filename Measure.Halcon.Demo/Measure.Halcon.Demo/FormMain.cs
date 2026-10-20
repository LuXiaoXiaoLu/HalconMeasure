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
            this.FormClosed += FormMain_FormClosed;
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

        //模板匹配对象
        private ShapeModelHelper shapeModelHelper = new ShapeModelHelper();

        private MathParams mathParams = new MathParams();


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
          this.hWindowControl1.Focus();
            var result=shapeModelHelper.CreateShapeModel( ref HwindowHandle,ref  HImage,  mathParams);
            if (result.IsSuccess) {
                AddLog(0, "创建模板成功");
            }
            else
            {
                AddLog(2, "创建模板失败");
            }
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
            // 窗体正在关闭/已销毁时，Invoke 会抛 ObjectDisposedException；
            // 采集线程在关窗过程中还在报错，这条日志直接丢掉即可。
            if (IsDisposed || list_Info.IsDisposed) return;

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
            if (cts != null)                       
            {
                AddLog(1, "已经在连续采图，勿重复启动");
                return;
            }
            if (!image.IsCameraOpen())
            {
                var openCamresult = image.OpenCam(CamName, "ssss");
                if (openCamresult.IsSuccess)
                {

                    AddLog(0, "相机打开成功");

                 
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

            var source = new CancellationTokenSource();
            cts = source;                          // 先挂上，防重入守卫才有效
            CancellationToken token = source.Token;

            Task.Factory.StartNew(
                () => GrabImageThread(token, source),
                token,
                TaskCreationOptions.LongRunning,   // 常驻采集循环：开专用线程，不占线程池
                TaskScheduler.Default);            // 必须显式 Default，否则 LongRunning 会被忽略

            AddLog(0, "开始连续采图");

        }



        private void GrabImageThread(CancellationToken token, CancellationTokenSource source)
        {
            try
            {
                while (!token.IsCancellationRequested)   // 用传进来的 token，不读字段 cts
                {
                    var result = image.GrabImage(ref HImage, ref HwindowHandle);

                    if (!result.IsSuccess)
                    {
                        AddLog(2, "采图失败：" + result.ErrorMsg);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Task 的异常没人 await 会被静默吞掉，这里必须落地
                AddLog(2, "采集线程异常：" + ex.Message);
            }
            finally
            {
                // 不管怎么退出（取消 / 采图失败 / 异常），都把"正在采图"这个状态清掉，
                // 否则下一次点【开始】会被防重入守卫永久挡住。
                if (ReferenceEquals(cts, source))
                {
                    cts = null;
                }
            }
        }

        private void btn_StopGrab_Click(object sender, EventArgs e)
        {
            AddLog(0, "停止连续采图");

            // 只举旗：真正的状态清理由采集线程的 finally 完成，
            // 因为"请求停止"不等于"线程已停"（可能还卡在 GrabImage 里）
            cts?.Cancel();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            cts?.Cancel();       // 让采集循环退出
            image.CloseCam();    // 关相机，别把句柄留给进程退出兜底
        }

        private void btn_FindShapeModel_Click(object sender, EventArgs e)
        {
            if (!HalconHelper.IsObjectValid(HImage))
            {
                AddLog(1, "查找模板失败：图像不存在");
                return;
            }
            if(mathParams.modelID == null)
            {
                AddLog(1, "查找模板失败：模板为空");
                return;
            }
           
            mathParams.startAngle=   Convert.ToDouble( this.txt_Angle.Text.Trim());
            mathParams.rangAngle = Convert.ToDouble(this.txt_Range.Text.Trim());
            mathParams.overlap = Convert.ToDouble(this.txt_Overlap.Text.Trim());
            mathParams.score = Convert.ToDouble(this.txt_Score.Text.Trim());
            mathParams.numMatchs = Convert.ToInt32(this.txt_NumMatchs.Text.Trim());

            var result = shapeModelHelper.FindShapeMode(ref HwindowHandle,ref HImage, mathParams);
            if (result.IsSuccess)
            {
                AddLog(0, "查找模板成功");
            }
            else
            {
                AddLog(1, "查找模板失败");
            }
        }

        private void txt_Overlap_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
