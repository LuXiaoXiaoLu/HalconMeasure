using HalconDotNet;
using Measure.Halcon.Demo.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace Measure.Halcon.Demo
{
    public partial class FormMain : Form
    {
        //圆极性
        public enum CircleTransitiom
        {
            all,
            positive,
            negative
        }

        public enum CirclePointSelect
        {
            max,
            first,
            last
        }


        public FormMain()
        {
            InitializeComponent();
            InitialParams();
            log = new LogHelper(list_Info);
            this.FormClosed += FormMain_FormClosed;
            this.list_Info.Columns[1].Width = list_Info.Width - this.list_Info.Columns[0].Width - 20;
            var halconInitialresult = halconHelper.HalconInitial(hWindowControl1, out HwindowHandle);
            log.Result(halconInitialresult);
            if (!halconInitialresult.IsSuccess)
            {
                return;
            }
            if (!imageHelper.IsCameraOpen())
            {
                var openCamresult = imageHelper.OpenCam(CamName, "ssss");
                if (openCamresult.IsSuccess)
                {

                    log.Result(openCamresult);

                    // 每个参数只读一次，别对相机重复提问
                    double exposure = imageHelper.GetExposureTime();
                    double gain = imageHelper.GetGain();

                    // 文本框显示真实值（保留小数），它才是"准"的那个
                    txt_ExposureTime.Text = exposure.ToString("F1");
                    txt_Gain.Text = gain.ToString("F1");

                    // TrackBar 天生只能表示整数，这里只做近似定位，不改动真实值
                    tb_ExposureTime.Value = ToTrackBarValue(exposure, tb_ExposureTime);
                    tb_Gain.Value = ToTrackBarValue(gain, tb_Gain);

                    chk_Trigger.Checked = imageHelper.GetTriggerMode();
                }
                else
                {
                    log.Result(openCamresult);
                    return;

                }
            }
            
        }
        
        private ImageHelper imageHelper = new ImageHelper();
        //相机名称
        private const string CamName = "GigEVision2";
        //图片显示句柄
        private HTuple HwindowHandle = new HTuple();
        //图片
        private HObject HImage = new HObject();

        private HalconHelper halconHelper = new HalconHelper();

        //连续采图标志位
        private CancellationTokenSource grabCts;

        //连续查找标志位
        private CancellationTokenSource findcts;
        //连续查找模板线程
        private Task findTask = Task.CompletedTask;
        //连续采图线程
        private Task grabTask = Task.CompletedTask;
        //上一次打开连续查找模板图片文件夹地址
        string lastFindPath =string.Empty;
        //圆参数模板

        private CircleParams circleParams = new CircleParams();

        //模板匹配对象
        private ShapeModelHelper shapeModelHelper = new ShapeModelHelper();
        //匹配模板数据
        private MathParams mathParams = new MathParams();
        private LogHelper log;

        public void InitialParams()
        {
            this.cmb_CirclePointSelect.DataSource=Enum.GetValues(typeof(CirclePointSelect));
            this.cmb_CircleTransition.DataSource = Enum.GetValues(typeof(CircleTransitiom));
            this.txt_Angle.Text = mathParams.startAngle.ToString();
            this.txt_Range.Text=mathParams.rangAngle.ToString();
            this.txt_Overlap.Text=mathParams.overlap.ToString();
            this.txt_Score.Text=mathParams.score.ToString();
            this.txt_NumMatchs.Text=mathParams.numMatchs.ToString();



            this.txt_CircleElements.Text=circleParams.circle_Elements.ToString();
            this.txt_CircleThreshold.Text=circleParams.circle_Threshold.ToString();
            this.txt_CircleSigma.Text=circleParams.circle_Sigma.ToString();
            this.cmb_CircleTransition.Text = circleParams.circle_Transition;
            this.cmb_CirclePointSelect.Text = circleParams.circle_Point_Select;
            this.txt_DisMin.Text=circleParams.circle_Dis_Min.ToString();
            this.txt_DisMax.Text=circleParams.circle_Dis_Max.ToString();

        }

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
            if (!HalconHelper.IsObjectValid(HImage))
            {
                log.Warn("创建模板失败：图像未打开");
                return;
            }

            this.hWindowControl1.Focus();
            var loadresult= LoadMathParamsFromUi();
            log.Result(loadresult);
            if (!loadresult.IsSuccess) {
                return;
            }

          
            var result=shapeModelHelper.CreateShapeModel( ref HwindowHandle,ref  HImage,  mathParams);
            log.Result(result);
        }
        

        private void btn_OpenImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "BMP 图片(*.bmp)|*.bmp|PNG 图片(*.png)|*.png|JPEG 图片(*.jpg;*.jpeg)|*.jpg;*.jpeg|TIFF 图片(*.tif;*.tiff)|*.tif;*.tiff";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    string imagePath = openFileDialog.FileName;
                    var result = imageHelper.ReadImage(imagePath, ref HwindowHandle, ref HImage);
                    log.Result(result);


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
                    var result = imageHelper.SaveImage(saveFileDialog.FileName, HImage);
                    log.Result(result);
                }
            }
        }

        private void btn_OneShot_Click(object sender, EventArgs e)
        {
            var result= imageHelper.GrabImage(ref HImage, ref HwindowHandle);
            log.Result(result);

        }

        private void list_Info_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chk_Trigger_CheckedChanged(object sender, EventArgs e)
        {
            var result = imageHelper.SetTriggerMode(chk_Trigger.Checked);
            if (!result.IsSuccess)
            {
                log.Fail(result);
            }
        }
        
        private void tb_ExposureTime_Scroll(object sender, EventArgs e)
        {
            this.tb_ExposureTime.Focus();
            var result = imageHelper.SetExposureTime(tb_ExposureTime.Value);
            if (result.IsSuccess)
            {
                this.txt_ExposureTime.Text = tb_ExposureTime.Value.ToString();
            }
            else
            {
                log.Fail(result);
            }
        }

        private void tb_Gain_Scroll(object sender, EventArgs e)
        {
            this.tb_Gain.Focus();
            var result = imageHelper.SetGain(tb_Gain.Value);
            if (result.IsSuccess)
            {
                this.txt_Gain.Text = tb_Gain.Value.ToString();
            }
            else
            {
                log.Fail(result);
            }
        }

        private void btn_Grab_Click(object sender, EventArgs e)
        {
            if (!grabTask.IsCompleted)                       
            {
                log.Warn("已经在连续采图，勿重复启动");
                return;
            }
            if (!imageHelper.IsCameraOpen())
            {
                var openCamresult = imageHelper.OpenCam(CamName, "ssss");
                if (openCamresult.IsSuccess)
                {

                    log.Result(openCamresult);

                 
                    double exposure = imageHelper.GetExposureTime();
                    double gain = imageHelper.GetGain();

                    // 文本框显示真实值（保留小数），它才是"准"的那个
                    txt_ExposureTime.Text = exposure.ToString("F1");
                    txt_Gain.Text = gain.ToString("F1");

                    // TrackBar 天生只能表示整数，这里只做近似定位，不改动真实值
                    tb_ExposureTime.Value = ToTrackBarValue(exposure, tb_ExposureTime);
                    tb_Gain.Value = ToTrackBarValue(gain, tb_Gain);

                    chk_Trigger.Checked = imageHelper.GetTriggerMode();
                }
                else
                {
                    log.Result(openCamresult);
                    return;

                }
            }

            var source = new CancellationTokenSource();
            grabCts = source;                         
            CancellationToken token = source.Token;

            grabTask= Task.Factory.StartNew(
                () => GrabImageThread(token),
                token,
                TaskCreationOptions.LongRunning,   // 常驻采集循环：开专用线程，不占线程池
                TaskScheduler.Default);            // 必须显式 Default，否则 LongRunning 会被忽略

            log.Info("开始连续采图");

        }



        private void GrabImageThread(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)   
                {
                    var result = imageHelper.GrabImage(ref HImage, ref HwindowHandle);

                    if (!result.IsSuccess)
                    {
                        log.Fail(result);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Task 的异常没人 await 会被静默吞掉，这里必须落地
                log.Fail(OperationResult.CreateFailResult("采集线程异常", ex));
            }
        }

        private void btn_StopGrab_Click(object sender, EventArgs e)
        {
            grabCts?.Cancel();
            log.Info("停止连续采图");
        }

      

        private void btn_FindShapeModel_Click(object sender, EventArgs e)
        {
            if (!HalconHelper.IsObjectValid(HImage))
            {
                log.Warn("查找模板失败：图像不存在");
                return;
            }
            if(mathParams.modelID == null)
            {
                log.Warn("查找模板失败：模板为空");
                return;
            }

            var loadResult=   LoadMathParamsFromUi();
            log.Result(loadResult);
            if (!loadResult.IsSuccess)
            {
                return;
            }
            var result = shapeModelHelper.FindShapeMode(ref HwindowHandle,ref HImage, mathParams);
            log.Result(result);
        }

        private void txt_Overlap_TextChanged(object sender, EventArgs e)
        {

        }
        private OperationResult LoadMathParamsFromUi()
        {
            try
            {
                mathParams.startAngle = Convert.ToDouble(txt_Angle.Text.Trim());
                mathParams.rangAngle = Convert.ToDouble(txt_Range.Text.Trim());
                mathParams.overlap = Convert.ToDouble(txt_Overlap.Text.Trim());
                mathParams.score = Convert.ToDouble(txt_Score.Text.Trim());
                mathParams.numMatchs = Convert.ToInt32(txt_NumMatchs.Text.Trim());
              
                return OperationResult.CreateSuccessResult("匹配模板参数设置成功:" + mathParams.ToString());
            }
            catch (Exception ex)
            {

                return OperationResult.CreateFailResult("匹配模板参数设置失败", ex);
            }

        }

        private OperationResult LoadCircleParamsFromUi()
        {

            try
            {
                circleParams.circle_Elements = Convert.ToInt32(this.txt_CircleElements.Text.Trim());
                circleParams.circle_Threshold = Convert.ToInt32(this.txt_CircleThreshold.Text.Trim());
                circleParams.circle_Sigma = Convert.ToInt32(this.txt_CircleSigma.Text.Trim());
                circleParams.circle_Transition = this.cmb_CircleTransition.Text;
                circleParams.circle_Point_Select = this.cmb_CirclePointSelect.Text;
                circleParams.circle_Dis_Min = Convert.ToDouble(txt_DisMin.Text.Trim());
                circleParams.circle_Dis_Max = Convert.ToDouble(txt_DisMax.Text.Trim());
                return OperationResult.CreateSuccessResult("圆拟合参数设置成功:"+circleParams.ToString());
            }
            catch (Exception ex)
            {

                return OperationResult.CreateFailResult("圆拟合参数设置失败", ex);
            }




           
        }

        private void btn_ContinueFind_Click(object sender, EventArgs e)
        {
            if (findcts != null&&!findTask.IsCompleted)
            {
                ColseContinuousFind();
                btn_ContinueFind.Text = "连续查找";
                return;
            }

            btn_ContinueFind.Text = "关闭连续查找";
            if (mathParams.modelID == null)
            {
                log.Warn("查找模板失败：模板为空");
                return;
            }

            var loadResult = LoadMathParamsFromUi();
            log.Result(loadResult);
            if (!loadResult.IsSuccess) return;
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(lastFindPath)) {
                    folderBrowserDialog.SelectedPath= lastFindPath;
                }
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = folderBrowserDialog.SelectedPath;
                    lastFindPath= path;
                    FileInfo[] files = new DirectoryInfo(path).GetFiles();
                    findcts = new CancellationTokenSource();
                    CancellationToken token = findcts.Token;
                  findTask=Task.Factory.StartNew(() => ContinuousFindThread(token, files),
                      token, TaskCreationOptions.LongRunning,
                       TaskScheduler.Default);
                }
            }
        }
        //连续查找方法
        private void ContinuousFindThread(CancellationToken token,FileInfo[] files)
        {
            while (!token.IsCancellationRequested) {
                foreach (FileInfo file in files)
                {
                    if (token.IsCancellationRequested) break;
                   var  resultImageHelper = imageHelper.ReadImage(file.FullName, ref HwindowHandle, ref HImage);
                    if (resultImageHelper.IsSuccess)
                    {
                        var result = shapeModelHelper.FindShapeMode(ref HwindowHandle, ref HImage, mathParams);
                        if (!result.IsSuccess)
                        {
                            log.Fail(result);
                        }
                    }
                    else
                    {
                        log.Fail(resultImageHelper);
                    }
                   token.WaitHandle.WaitOne(2000);
                }
            }
            ;
            
        }
        //关闭连续查找方法
        private void ColseContinuousFind()
        {
            try
            {
                findcts.Cancel();
                findcts = null;
                log.Info("关闭连续查找");
             
            }
            catch (Exception ex)
            {
                log.Fail(OperationResult.CreateFailResult("关闭连续查找失败", ex));
            }
            
           
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            grabCts?.Cancel();       // 让采集循环退出
            findcts?.Cancel();
            grabTask.Wait(500);
            findTask.Wait(500);
            imageHelper.CloseCam();    // 关相机，别把句柄留给进程退出兜底
        }

        private void btn_CircleROI_Click(object sender, EventArgs e)
        {
            hWindowControl1.Focus();
            var loadResult= LoadCircleParamsFromUi();
            log.Result(loadResult);
            if (!loadResult.IsSuccess)
            {
                return;
            }
           
            halconHelper.draw_spoke3(HImage, out HObject hRegions, HwindowHandle, circleParams.circle_Elements, circleParams.circle_Caliper_Height, 
                circleParams.circle_Caliper_Width,out circleParams.rOI_X, out circleParams.rOI_Y, out circleParams.circle_Direct);
            if (circleParams.rOI_X.Length < 4)
            {
                log.Error("创建ROI失败");
            }
            else
            {
                HOperatorSet.DispObj(HImage, HwindowHandle);
                HOperatorSet.DispObj(hRegions, HwindowHandle);
                log.Info("创建ROI成功");
            }
        }

        private void btn_CircleFit_Click(object sender, EventArgs e)
        {
            var loadResult=LoadCircleParamsFromUi();
            log.Result(loadResult);
            if (!loadResult.IsSuccess)
            {
                return;
            }
           var result = halconHelper.Fit_Circle(HImage, circleParams, out HObject hRegion, out HTuple resultRow,
           out HObject result_Circle, out HTuple hv_RowCenter, out HTuple hv_ColCenter,
           out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder,
           out HTuple hv_ArcAngle);
            HOperatorSet.DispObj(HImage, HwindowHandle);
            HOperatorSet.DispObj(result_Circle, HwindowHandle);
            log.Result(result);
        }

        private void btn_Calib_Click(object sender, EventArgs e)
        {
            if (circleParams.circleRadius == null && circleParams.circleRadius.TupleInt()>0)
            {
                circleParams.scale=Convert.ToDouble(this.txt_Distance.Text.Trim())/(circleParams.circleRadius.TupleInt()/2);
                log.Info("标定成功"+circleParams.scale);
            }
            else
            {
                log.Error("标定参数错误");
            }
        }

        private void btn_Measure_Click(object sender, EventArgs e)
        {
            Measure(HImage, HwindowHandle);
        }

        private OperationResult Measure(HObject hImage,HTuple hWindowHandle)
        {

            try
            {
                var result = shapeModelHelper.FindShapeMode(ref hWindowHandle, ref hImage, mathParams);
                if (!result.IsSuccess)
                {
                    return result;
                }

                log.Info("模板匹配成功");


                result = halconHelper.Fit_Circle(HImage, circleParams, out HObject hRegion, out HTuple resultRow,
               out HObject result_Circle, out HTuple hv_RowCenter, out HTuple hv_ColCenter,
               out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder,
               out HTuple hv_ArcAngle);

                if (!result.IsSuccess)
                {
                    return result;
                }

                log.Info("圆拟合成功");


                double distance = circleParams.scale * hv_Radius * 2;

                if (distance >= circleParams.circle_Dis_Min && distance <= circleParams.circle_Dis_Max)
                {
                    halconHelper.disp_message(HwindowHandle, "测量成功:" + distance, "window", 20, 20, "green", "false");
                    log.Info("测量成功:" + distance);
                    return OperationResult.CreateSuccessResult("测量成功:" + distance);
                }
                else
                {
                    halconHelper.disp_message(HwindowHandle, "测量失败", "window", 20, 20, "red", "false");
                    log.Warn("测量失败:" + distance);
                    return OperationResult.CreateFailResult("测量失败");
                }
            }
            catch (Exception ex)
            {

                return OperationResult.CreateFailResult("测量失败", ex);
            }

            

        }
    }
}
