using HalconDotNet;
using Measure.Halcon.Demo.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            BindParams();
            log = new LogHelper(list_Info);
            this.FormClosed += FormMain_FormClosed;
            this.FormClosing += FormMain_FormClosing;
            log.Result(LoadModelList());
            this.list_Info.Columns[1].Width = list_Info.Width - this.list_Info.Columns[0].Width - 20;
            var halconInitialresult = halconHelper.HalconInitial(hWindowControl1, out HwindowHandle);
            log.Result(halconInitialresult);
            if (!halconInitialresult.IsSuccess)
            {
                return;
            }

            var result = shapeModelHelper.ImportShapeModel(basePath, mathParams, circleParams);
            log.Result(result);
            if (result.IsSuccess)
            {
                RefreshBindings(this);   // 参数 → 界面 强制刷新一遍
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
        //上一次保存参数文件夹地址

        //模板名称
        string modelFileName;

        //圆参数模板
        private CircleParams circleParams = new CircleParams();

        //模板匹配对象
        private ShapeModelHelper shapeModelHelper = new ShapeModelHelper();
        //匹配模板数据
        private MatchParams mathParams = new MatchParams();
        private LogHelper log;

        private string basePath = Path.Combine(Program.DataRoot, "model");

        public void InitialParams()
        {
            // 用「枚举名」填 Items（字符串），这样 SelectedItem 能按 "max" 精确选中；
            // 若用 DataSource 绑枚举对象，Text 的读写会被数据绑定接管，显示不可靠
            this.cmb_CirclePointSelect.Items.Clear();
            this.cmb_CirclePointSelect.Items.AddRange(Enum.GetNames(typeof(CirclePointSelect)));
            this.cmb_CircleTransition.Items.Clear();
            this.cmb_CircleTransition.Items.AddRange(Enum.GetNames(typeof(CircleTransitiom)));
            // 数值参数改由 DataBindings 自动填充，见 BindParams()
            this.cmb_CircleTransition.SelectedItem = circleParams.circle_Transition;
            this.cmb_CirclePointSelect.SelectedItem = circleParams.circle_Point_Select;
            
        }

        /// <summary>
        /// 把界面上的数值参数与参数对象双向绑定：
        /// 参数改了界面自动刷新，界面输入自动写回参数，不再需要手写 Load/Initial 同步。
        /// </summary>
        private void BindParams()
        {
            // 匹配模板参数
            BindDouble(txt_Angle, mathParams, nameof(mathParams.startAngle));
            BindDouble(txt_Range, mathParams, nameof(mathParams.rangAngle));
            BindDouble(txt_Overlap, mathParams, nameof(mathParams.overlap));
            BindDouble(txt_Score, mathParams, nameof(mathParams.score));
            BindInt(txt_NumMatchs, mathParams, nameof(mathParams.numMatchs));

            // 圆测量参数
            BindInt(txt_CircleElements, circleParams, nameof(circleParams.circle_Elements));
            BindInt(txt_CircleThreshold, circleParams, nameof(circleParams.circle_Threshold));
            BindDouble(txt_CircleSigma, circleParams, nameof(circleParams.circle_Sigma));
            BindDouble(txt_DisMin, circleParams, nameof(circleParams.circle_Dis_Min));
            BindDouble(txt_DisMax, circleParams, nameof(circleParams.circle_Dis_Max));
        }

        /// <summary>TextBox ↔ double 属性双向绑定；输入非法时保持参数原值，不抛异常。</summary>
        private void BindDouble(TextBox box, object source, string propertyName)
        {
            box.DataBindings.Add("Text", source, propertyName, true, DataSourceUpdateMode.OnPropertyChanged);
            Binding binding = box.DataBindings[0];
            var prop = source.GetType().GetProperty(propertyName);

            binding.Format += (s, e) =>
            {
                if (e.Value is double d) e.Value = d.ToString("F2");
            };

            binding.Parse += (s, e) =>
            {
                if (double.TryParse(e.Value?.ToString(), out double v)) e.Value = v;
                else e.Value = prop.GetValue(source);   // 非法输入：保持原值
            };
        }

        /// <summary>TextBox ↔ int 属性双向绑定；输入非法时保持参数原值。</summary>
        private void BindInt(TextBox box, object source, string propertyName)
        {
            box.DataBindings.Add("Text", source, propertyName, true, DataSourceUpdateMode.OnPropertyChanged);
            Binding binding = box.DataBindings[0];
            var prop = source.GetType().GetProperty(propertyName);

            binding.Parse += (s, e) =>
            {
                if (int.TryParse(e.Value?.ToString(), out int v)) e.Value = v;
                else e.Value = prop.GetValue(source);   // 非法输入：保持原值
            };
        }

        /// <summary>
        /// 强制所有绑定重新从数据源读值，把界面刷新成参数对象的当前值。
        /// 参数类没实现 INotifyPropertyChanged，代码改属性后界面不会自己变，
        /// 所以加载参数后要手动补一次「数据源 → 控件」的同步。
        /// </summary>
        private void RefreshBindings(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                foreach (Binding b in c.DataBindings)
                {
                    b.ReadValue();          // 数据源 → 控件
                }
                RefreshBindings(c);         // 递归：GroupBox 里的控件也要刷
            }
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
            log.Fail(result);
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
            var result = shapeModelHelper.FindShapeMode(ref HwindowHandle,ref HImage, mathParams,out HTuple homMat2d_T);
            log.Result(result);
        }

        private void txt_Overlap_TextChanged(object sender, EventArgs e)
        {

        }
        private OperationResult LoadMathParamsFromUi()
        {
            // 参数已由 DataBindings 实时同步到 mathParams，这里只负责记录结果
            return OperationResult.CreateSuccessResult("匹配模板参数设置成功:" + mathParams.ToString());
        }

        private OperationResult LoadCircleParamsFromUi()
        {
            // 数值参数已由 DataBindings 实时同步；下拉框仍需手动同步一次
            circleParams.circle_Transition = this.cmb_CircleTransition.Text;
            circleParams.circle_Point_Select = this.cmb_CirclePointSelect.Text;
            return OperationResult.CreateSuccessResult("圆拟合参数设置成功:" + circleParams.ToString());
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
                        var result = shapeModelHelper.FindShapeMode(ref HwindowHandle, ref HImage, mathParams, out HTuple homMat2d_T);
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
            if (!HalconHelper.IsObjectValid(HImage))
            {
                log.Warn("创建圆ROI失败：图像未打开");
                return;
            }

            HOperatorSet.DispObj(HImage, HwindowHandle);
            hWindowControl1.Focus();
            var loadResult= LoadCircleParamsFromUi();
            log.Result(loadResult);
            if (!loadResult.IsSuccess)
            {
                return;
            }
            
            halconHelper.draw_spoke3(HImage, out HObject hRegions, HwindowHandle, circleParams.circle_Elements, circleParams.circle_Caliper_Height, 
                circleParams.circle_Caliper_Width,out circleParams.rOI_Y, out circleParams.rOI_X, out circleParams.circle_Direct);
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
            if (!HalconHelper.IsObjectValid(HImage))
            {
                log.Warn("拟合失败：图像未打开");
                return;
            }

            if (circleParams.rOI_X == null || circleParams.rOI_X.Length < 4 )  
            {
                log.Error("未创建圆ROI");
                return;
            }


            var loadResult=LoadCircleParamsFromUi();
            log.Result(loadResult);
            if (!loadResult.IsSuccess)
            {
                return;
            }
           var result = halconHelper.Fit_Circle(HImage, circleParams,0, out HObject hRegion, out HTuple resultRow,
           out HObject result_Circle, out HTuple hv_RowCenter, out HTuple hv_ColCenter,
           out circleParams.circleRadius, out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder,
           out HTuple hv_ArcAngle);

            // 先记日志、先判成败：拟合失败时 result_Circle 是 null，
            // 直接 DispObj 会抛 HALCON #4056（object-ID is NULL）
            log.Result(result);
            if (!result.IsSuccess)
            {
                return;
            }

            HOperatorSet.DispObj(HImage, HwindowHandle);
            if (HalconHelper.IsObjectValid(result_Circle))
            {
                HOperatorSet.DispObj(result_Circle, HwindowHandle);
            }
        }

        private void btn_Calib_Click(object sender, EventArgs e)
        {
            var loadResut = LoadCircleParamsFromUi();
            log.Result(loadResut);
            if (!loadResut.IsSuccess) {
                return;
            }
            if (circleParams.circleRadius != null && circleParams.circleRadius.TupleInt()>0)
            {
                circleParams.scale=Convert.ToDouble(this.txt_Distance.Text.Trim())/(circleParams.circleRadius*2.0);
                log.Info("标定成功"+circleParams.scale);
            }
            else
            {
                log.Warn("标定失败：请先「拟合圆」，拿到像素半径后再标定");
            }
        }

        private void btn_Measure_Click(object sender, EventArgs e)
        {
            
            OperationResult loadResut = LoadCircleParamsFromUi();
            log.Result(loadResut);
            if (!loadResut.IsSuccess) {
                return;
            };

          OperationResult result=Measure(HImage, HwindowHandle);
          log.Result(result);

        }

        private OperationResult Measure(HObject hImage, HTuple hWindowHandle)
        {
            try
            {
                // 第 1 步：查找模板
                var findResult = shapeModelHelper.FindShapeMode(ref hWindowHandle, ref hImage, mathParams, out HTuple homMat2d_T);
                if (!findResult.IsSuccess)
                    return OperationResult.CreateFailResult("测量失败：模板匹配。" + findResult.Msg, findResult.Error);

                // 第 2 步：拟合圆
                var fitResult = halconHelper.Fit_Circle(HImage, circleParams, homMat2d_T, out HObject hRegion, out HTuple resultRow,
                    out HObject result_Circle, out HTuple hv_RowCenter, out HTuple hv_ColCenter,
                    out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi, out HTuple hv_PointOrder,
                    out HTuple hv_ArcAngle);
                if (!fitResult.IsSuccess)
                    return OperationResult.CreateFailResult("测量失败：圆拟合。" + fitResult.Msg, fitResult.Error);

               

                // 第 3 步：判断直径
                double distance = circleParams.scale * hv_Radius * 2;
                if (distance < circleParams.circle_Dis_Min || distance > circleParams.circle_Dis_Max)
                {
                    HOperatorSet.DispObj(HImage, HwindowHandle);
                    halconHelper.disp_message(HwindowHandle, "测量结果：NG " + distance, "window", 35, 20, "red", "false");
                    return OperationResult.CreateFailResult("测量结果：NG " + distance);   // 无异常 → Warn
                }

                
                HOperatorSet.DispObj(result_Circle, HwindowHandle);
                halconHelper.disp_message(HwindowHandle, "OK:" + distance, "window", 35, 20, "green", "false");
                return OperationResult.CreateSuccessResult("OK:" + distance);
            }
            catch (Exception ex)
            {
                return OperationResult.CreateFailResult("测量失败", ex);   // 带异常 → Error
            }
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private OperationResult LoadModelList()
        {

            try
            {
                var getAllShapeModelResult = shapeModelHelper.GetAllShapeModel(basePath, out List<string> list);
                if (!getAllShapeModelResult.IsSuccess)
                {
                    return OperationResult.CreateFailResult("读取模板列表失败：" + getAllShapeModelResult.Msg, getAllShapeModelResult.Error);
                }
                this.listModel.Items.Clear();
                if (list.Count == 0)
                {
                    return OperationResult.CreateFailResult("还没保存过模板，请先创建并保存");
                }
                this.listModel.Items.AddRange(list.ToArray());
                return OperationResult.CreateSuccessResult("读取模板列表成功");

            }
            catch (Exception ex)
            {

                return OperationResult.CreateFailResult("载入模板列表失败", ex);
            }

           
        }




        private void Savefile_Click(object sender, EventArgs e)
        {
            var result = LoadCircleParamsFromUi();
            if (!result.IsSuccess)
            {
                log.Result(result);
                return;
            }
            using (FolderBrowserDialog folder=new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(basePath))
                {
                    folder.SelectedPath = basePath;
                }
                if (folder.ShowDialog() ==DialogResult.OK )
                {
                    string path=folder.SelectedPath;
                     result = shapeModelHelper.SaveShapeModel(path, mathParams,circleParams);
                    log.Result(result);
                }
            }
            
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (true)   
            {
                var r = MessageBox.Show("有未保存的修改，是否保存？", "提示",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Yes) {
                    modelFileName = (this.modelName.Text ?? string.Empty).Trim();
                    bool IsValiresult = IsValidModelName(modelFileName, out string IsValiReson);
                    if (!IsValiresult)
                    {
                        log.Warn("保存模板失败：" + IsValiReson);
                        return;
                    }

                    var result = LoadCircleParamsFromUi();
                    if (!result.IsSuccess)
                    {
                        log.Result(result);
                        return;
                    }
                    string path = Path.Combine(basePath, modelFileName);
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception ex)
                    {

                        log.Result(OperationResult.CreateFailResult("创建模板目录失败：" + path, ex));
                    }

                    result = shapeModelHelper.SaveShapeModel(path, mathParams, circleParams);
                    log.Result(result);
                    if (!result.IsSuccess)
                    {
                        return;
                    }
                    LoadModelList();
                    this.listModel.SelectedItem = modelName;
                }
                else if (r == DialogResult.Cancel) { e.Cancel = true; } 

            }
        }

        private void ImportParamsMenuItem_Click(object sender, EventArgs e)
        {
            string ImportModelName=this.listModel.Text.Trim();
            string path = Path.Combine(basePath, ImportModelName);
            var result = shapeModelHelper.ImportShapeModel(path, mathParams, circleParams);
            log.Result(result);
            if (result.IsSuccess)
             {
                RefreshBindings(this);   
             }
                

            
        }

        private void saveModel_Click(object sender, EventArgs e)
        {
            modelFileName = (this.modelName.Text ?? string.Empty).Trim();
            bool IsValiresult = IsValidModelName(modelFileName, out string IsValiReson);
            if (!IsValiresult)
            {
                log.Warn("保存模板失败：" + IsValiReson);
                return;
            }

            var result = LoadCircleParamsFromUi();
            if (!result.IsSuccess)
            {
                log.Result(result);
                return;
            }
            string path = Path.Combine(basePath, modelFileName);
            if (File.Exists(Path.Combine(path, "param.ini"))) {
               var answer = MessageBox.Show($"模板「{modelName}」已存在，是否覆盖？", "确认覆盖",
               MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
               if (answer != DialogResult.Yes) return;
            }

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {

                log.Result(OperationResult.CreateFailResult("创建模板目录失败：" + path, ex));
            }

            result = shapeModelHelper.SaveShapeModel(path, mathParams, circleParams);
            log.Result(result);
            if (!result.IsSuccess)
            {
                return;
            }
            LoadModelList();
            this.listModel.SelectedItem = modelName;

        }


        private static bool IsValidModelName(string name, out string reason)
        {
            reason = null;
            if (string.IsNullOrWhiteSpace(name)) { reason = "模板名不能为空"; return false; }
            if (name == "." || name == "..") { reason = "模板名不能是 . 或 .."; return false; }
            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            { reason = "模板名含非法字符：" + name; return false; }
            if (name.Length > 64) { reason = "模板名过长（最多 64 字符）"; return false; }
            return true;
        }


        private void readModel_Click(object sender, EventArgs e)
        {
            string ImportModelName = this.listModel.Text.Trim();
            if (this.listModel.SelectedIndex<0 || string.IsNullOrEmpty(ImportModelName))
            {
                log.Warn("请选择读取模板");
                return;
            }
            
            
            string path = Path.Combine(basePath, ImportModelName);
            var result = shapeModelHelper.ImportShapeModel(path, mathParams, circleParams);
            log.Result(result);
            if (result.IsSuccess)
            {
                RefreshBindings(this);
            }
        }




        private void delModel_Click(object sender, EventArgs e)
        {
            string ImportModelName = this.listModel.Text.Trim();
            string path = Path.Combine(basePath, ImportModelName);
            var  answer = MessageBox.Show($"确定删除「{ImportModelName}」？", "确认删除",
               MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (!Directory.Exists(path))
            {
                MessageBox.Show("该目录已不存在", "提示");
                LoadModelList(); 
                return;
            }
            if (answer == DialogResult.Yes) {
                
                Directory.Delete(path, true);
                LoadModelList();
                if (this.listModel.Items.Count > 0) { 
                    this.listModel.SelectedIndex = 0;
                }
                else
                {
                    this.listModel.Text= string.Empty;
                }
            }
        }
    }
}
