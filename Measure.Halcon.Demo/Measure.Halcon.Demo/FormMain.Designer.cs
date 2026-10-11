namespace Measure.Halcon.Demo
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.list_Info = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tb_Gain = new System.Windows.Forms.TrackBar();
            this.btn_OneShot = new System.Windows.Forms.Button();
            this.txt_ExposureTime = new System.Windows.Forms.TextBox();
            this.txt_Gain = new System.Windows.Forms.TextBox();
            this.tb_ExposureTime = new System.Windows.Forms.TrackBar();
            this.btn_Grab = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.btn_StopGrab = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_OpenImage = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.btn_SaveImage = new System.Windows.Forms.Button();
            this.chk_RealTimeProc = new System.Windows.Forms.CheckBox();
            this.chk_Trigger = new System.Windows.Forms.CheckBox();
            this.gb1 = new System.Windows.Forms.GroupBox();
            this.btn_ContinueFind = new System.Windows.Forms.Button();
            this.btn_FindShapeModel = new System.Windows.Forms.Button();
            this.btn_CreatShapeModel = new System.Windows.Forms.Button();
            this.txt_NumMatchs = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_Overlap = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_Score = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_Range = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txt_Angle = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_Calib = new System.Windows.Forms.Button();
            this.btn_Measure = new System.Windows.Forms.Button();
            this.btn_CircleFit = new System.Windows.Forms.Button();
            this.btn_CircleROI = new System.Windows.Forms.Button();
            this.cmb_CirclePointSelect = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmb_CircleTransition = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txt_Distance = new System.Windows.Forms.TextBox();
            this.txt_CircleSigma = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_DisMax = new System.Windows.Forms.TextBox();
            this.txt_DisMin = new System.Windows.Forms.TextBox();
            this.txt_CircleThreshold = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_CircleElements = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Gain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_ExposureTime)).BeginInit();
            this.gb1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(27, 28);
            this.hWindowControl1.Margin = new System.Windows.Forms.Padding(4);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(477, 380);
            this.hWindowControl1.TabIndex = 0;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(477, 380);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.list_Info);
            this.groupBox1.Location = new System.Drawing.Point(27, 440);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(477, 205);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "系统日志";
            // 
            // list_Info
            // 
            this.list_Info.BackColor = System.Drawing.SystemColors.Control;
            this.list_Info.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.list_Info.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.list_Info.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.list_Info.HideSelection = false;
            this.list_Info.LargeImageList = this.imageList1;
            this.list_Info.Location = new System.Drawing.Point(21, 24);
            this.list_Info.Name = "list_Info";
            this.list_Info.Size = new System.Drawing.Size(439, 166);
            this.list_Info.TabIndex = 0;
            this.list_Info.UseCompatibleStateImageBehavior = false;
            this.list_Info.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "日期时间";
            this.columnHeader1.Width = 150;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "info.ico");
            this.imageList1.Images.SetKeyName(1, "warning.ico");
            this.imageList1.Images.SetKeyName(2, "error.ico");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tb_Gain);
            this.groupBox2.Controls.Add(this.btn_OneShot);
            this.groupBox2.Controls.Add(this.txt_ExposureTime);
            this.groupBox2.Controls.Add(this.txt_Gain);
            this.groupBox2.Controls.Add(this.tb_ExposureTime);
            this.groupBox2.Controls.Add(this.btn_Grab);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.btn_StopGrab);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btn_OpenImage);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.btn_SaveImage);
            this.groupBox2.Controls.Add(this.chk_RealTimeProc);
            this.groupBox2.Controls.Add(this.chk_Trigger);
            this.groupBox2.Location = new System.Drawing.Point(565, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(443, 231);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "图像采集";
            // 
            // tb_Gain
            // 
            this.tb_Gain.Location = new System.Drawing.Point(130, 120);
            this.tb_Gain.Maximum = 20;
            this.tb_Gain.Name = "tb_Gain";
            this.tb_Gain.Size = new System.Drawing.Size(146, 56);
            this.tb_Gain.TabIndex = 14;
            this.tb_Gain.TickFrequency = 2;
            // 
            // btn_OneShot
            // 
            this.btn_OneShot.Location = new System.Drawing.Point(119, 30);
            this.btn_OneShot.Name = "btn_OneShot";
            this.btn_OneShot.Size = new System.Drawing.Size(89, 31);
            this.btn_OneShot.TabIndex = 7;
            this.btn_OneShot.Text = "单帧采图";
            this.btn_OneShot.UseVisualStyleBackColor = true;
            this.btn_OneShot.Click += new System.EventHandler(this.btn_OneShot_Click);
            // 
            // txt_ExposureTime
            // 
            this.txt_ExposureTime.Location = new System.Drawing.Point(282, 77);
            this.txt_ExposureTime.Name = "txt_ExposureTime";
            this.txt_ExposureTime.ReadOnly = true;
            this.txt_ExposureTime.Size = new System.Drawing.Size(61, 29);
            this.txt_ExposureTime.TabIndex = 12;
            this.txt_ExposureTime.Text = "0";
            this.txt_ExposureTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_Gain
            // 
            this.txt_Gain.Location = new System.Drawing.Point(282, 119);
            this.txt_Gain.Name = "txt_Gain";
            this.txt_Gain.ReadOnly = true;
            this.txt_Gain.Size = new System.Drawing.Size(61, 29);
            this.txt_Gain.TabIndex = 13;
            this.txt_Gain.Text = "0";
            this.txt_Gain.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tb_ExposureTime
            // 
            this.tb_ExposureTime.Location = new System.Drawing.Point(132, 75);
            this.tb_ExposureTime.Maximum = 10000;
            this.tb_ExposureTime.Name = "tb_ExposureTime";
            this.tb_ExposureTime.Size = new System.Drawing.Size(144, 56);
            this.tb_ExposureTime.TabIndex = 15;
            this.tb_ExposureTime.TickFrequency = 1000;
            this.tb_ExposureTime.Value = 34;
            // 
            // btn_Grab
            // 
            this.btn_Grab.Location = new System.Drawing.Point(221, 30);
            this.btn_Grab.Name = "btn_Grab";
            this.btn_Grab.Size = new System.Drawing.Size(89, 31);
            this.btn_Grab.TabIndex = 6;
            this.btn_Grab.Text = "连续采图";
            this.btn_Grab.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(29, 129);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(104, 23);
            this.label14.TabIndex = 10;
            this.label14.Text = "增益（db）:";
            // 
            // btn_StopGrab
            // 
            this.btn_StopGrab.Location = new System.Drawing.Point(323, 30);
            this.btn_StopGrab.Name = "btn_StopGrab";
            this.btn_StopGrab.Size = new System.Drawing.Size(89, 31);
            this.btn_StopGrab.TabIndex = 6;
            this.btn_StopGrab.Text = "停止采图";
            this.btn_StopGrab.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 23);
            this.label2.TabIndex = 11;
            this.label2.Text = "图像处理：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 23);
            this.label1.TabIndex = 11;
            this.label1.Text = "实时采集：";
            // 
            // btn_OpenImage
            // 
            this.btn_OpenImage.Location = new System.Drawing.Point(139, 179);
            this.btn_OpenImage.Name = "btn_OpenImage";
            this.btn_OpenImage.Size = new System.Drawing.Size(89, 31);
            this.btn_OpenImage.TabIndex = 5;
            this.btn_OpenImage.Text = "打开图像";
            this.btn_OpenImage.UseVisualStyleBackColor = true;
            this.btn_OpenImage.Click += new System.EventHandler(this.btn_OpenImage_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(29, 82);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(100, 23);
            this.label13.TabIndex = 11;
            this.label13.Text = "曝光（us）:";
            // 
            // btn_SaveImage
            // 
            this.btn_SaveImage.Location = new System.Drawing.Point(262, 178);
            this.btn_SaveImage.Name = "btn_SaveImage";
            this.btn_SaveImage.Size = new System.Drawing.Size(89, 31);
            this.btn_SaveImage.TabIndex = 4;
            this.btn_SaveImage.Text = "保存图像";
            this.btn_SaveImage.UseVisualStyleBackColor = true;
            this.btn_SaveImage.Click += new System.EventHandler(this.btn_SaveImage_Click);
            // 
            // chk_RealTimeProc
            // 
            this.chk_RealTimeProc.AutoSize = true;
            this.chk_RealTimeProc.Location = new System.Drawing.Point(357, 123);
            this.chk_RealTimeProc.Name = "chk_RealTimeProc";
            this.chk_RealTimeProc.Size = new System.Drawing.Size(66, 27);
            this.chk_RealTimeProc.TabIndex = 8;
            this.chk_RealTimeProc.Text = "实时";
            this.chk_RealTimeProc.UseVisualStyleBackColor = true;
            // 
            // chk_Trigger
            // 
            this.chk_Trigger.AutoSize = true;
            this.chk_Trigger.Location = new System.Drawing.Point(357, 79);
            this.chk_Trigger.Name = "chk_Trigger";
            this.chk_Trigger.Size = new System.Drawing.Size(66, 27);
            this.chk_Trigger.TabIndex = 9;
            this.chk_Trigger.Text = "触发";
            this.chk_Trigger.UseVisualStyleBackColor = true;
            // 
            // gb1
            // 
            this.gb1.Controls.Add(this.btn_ContinueFind);
            this.gb1.Controls.Add(this.btn_FindShapeModel);
            this.gb1.Controls.Add(this.btn_CreatShapeModel);
            this.gb1.Controls.Add(this.txt_NumMatchs);
            this.gb1.Controls.Add(this.label5);
            this.gb1.Controls.Add(this.txt_Overlap);
            this.gb1.Controls.Add(this.label4);
            this.gb1.Controls.Add(this.txt_Score);
            this.gb1.Controls.Add(this.label3);
            this.gb1.Controls.Add(this.txt_Range);
            this.gb1.Controls.Add(this.label12);
            this.gb1.Controls.Add(this.txt_Angle);
            this.gb1.Controls.Add(this.label6);
            this.gb1.Location = new System.Drawing.Point(565, 262);
            this.gb1.Name = "gb1";
            this.gb1.Size = new System.Drawing.Size(443, 163);
            this.gb1.TabIndex = 6;
            this.gb1.TabStop = false;
            this.gb1.Text = "几何定位";
            // 
            // btn_ContinueFind
            // 
            this.btn_ContinueFind.Location = new System.Drawing.Point(339, 89);
            this.btn_ContinueFind.Name = "btn_ContinueFind";
            this.btn_ContinueFind.Size = new System.Drawing.Size(80, 31);
            this.btn_ContinueFind.TabIndex = 3;
            this.btn_ContinueFind.Text = "连续查找";
            this.btn_ContinueFind.UseVisualStyleBackColor = true;
            // 
            // btn_FindShapeModel
            // 
            this.btn_FindShapeModel.Location = new System.Drawing.Point(246, 89);
            this.btn_FindShapeModel.Name = "btn_FindShapeModel";
            this.btn_FindShapeModel.Size = new System.Drawing.Size(80, 31);
            this.btn_FindShapeModel.TabIndex = 3;
            this.btn_FindShapeModel.Text = "查找模板";
            this.btn_FindShapeModel.UseVisualStyleBackColor = true;
            // 
            // btn_CreatShapeModel
            // 
            this.btn_CreatShapeModel.Location = new System.Drawing.Point(153, 89);
            this.btn_CreatShapeModel.Name = "btn_CreatShapeModel";
            this.btn_CreatShapeModel.Size = new System.Drawing.Size(80, 31);
            this.btn_CreatShapeModel.TabIndex = 3;
            this.btn_CreatShapeModel.Text = "创建模板";
            this.btn_CreatShapeModel.UseVisualStyleBackColor = true;
            this.btn_CreatShapeModel.Click += new System.EventHandler(this.btn_CreatShapeModel_Click);
            // 
            // txt_NumMatchs
            // 
            this.txt_NumMatchs.Location = new System.Drawing.Point(80, 91);
            this.txt_NumMatchs.Name = "txt_NumMatchs";
            this.txt_NumMatchs.Size = new System.Drawing.Size(40, 29);
            this.txt_NumMatchs.TabIndex = 2;
            this.txt_NumMatchs.Text = "1";
            this.txt_NumMatchs.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(29, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 23);
            this.label5.TabIndex = 1;
            this.label5.Text = "数量";
            // 
            // txt_Overlap
            // 
            this.txt_Overlap.Location = new System.Drawing.Point(383, 41);
            this.txt_Overlap.Name = "txt_Overlap";
            this.txt_Overlap.Size = new System.Drawing.Size(40, 29);
            this.txt_Overlap.TabIndex = 2;
            this.txt_Overlap.Text = "0.1";
            this.txt_Overlap.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(326, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 23);
            this.label4.TabIndex = 1;
            this.label4.Text = "重叠度";
            // 
            // txt_Score
            // 
            this.txt_Score.Location = new System.Drawing.Point(282, 41);
            this.txt_Score.Name = "txt_Score";
            this.txt_Score.Size = new System.Drawing.Size(40, 29);
            this.txt_Score.TabIndex = 2;
            this.txt_Score.Text = "0.5";
            this.txt_Score.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(235, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 23);
            this.label3.TabIndex = 1;
            this.label3.Text = "分数";
            // 
            // txt_Range
            // 
            this.txt_Range.Location = new System.Drawing.Point(181, 41);
            this.txt_Range.Name = "txt_Range";
            this.txt_Range.Size = new System.Drawing.Size(40, 29);
            this.txt_Range.TabIndex = 2;
            this.txt_Range.Text = "90";
            this.txt_Range.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(128, 44);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 23);
            this.label12.TabIndex = 1;
            this.label12.Text = "范围(°)";
            // 
            // txt_Angle
            // 
            this.txt_Angle.Location = new System.Drawing.Point(80, 41);
            this.txt_Angle.Name = "txt_Angle";
            this.txt_Angle.Size = new System.Drawing.Size(40, 29);
            this.txt_Angle.TabIndex = 2;
            this.txt_Angle.Text = "-45";
            this.txt_Angle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 23);
            this.label6.TabIndex = 1;
            this.label6.Text = "角度(°)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_Calib);
            this.groupBox3.Controls.Add(this.btn_Measure);
            this.groupBox3.Controls.Add(this.btn_CircleFit);
            this.groupBox3.Controls.Add(this.btn_CircleROI);
            this.groupBox3.Controls.Add(this.cmb_CirclePointSelect);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.cmb_CircleTransition);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.txt_Distance);
            this.groupBox3.Controls.Add(this.txt_CircleSigma);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txt_DisMax);
            this.groupBox3.Controls.Add(this.txt_DisMin);
            this.groupBox3.Controls.Add(this.txt_CircleThreshold);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txt_CircleElements);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(565, 427);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(443, 227);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "圆测量";
            // 
            // btn_Calib
            // 
            this.btn_Calib.Location = new System.Drawing.Point(247, 187);
            this.btn_Calib.Name = "btn_Calib";
            this.btn_Calib.Size = new System.Drawing.Size(80, 31);
            this.btn_Calib.TabIndex = 15;
            this.btn_Calib.Text = "标定";
            this.btn_Calib.UseVisualStyleBackColor = true;
            // 
            // btn_Measure
            // 
            this.btn_Measure.Location = new System.Drawing.Point(339, 187);
            this.btn_Measure.Name = "btn_Measure";
            this.btn_Measure.Size = new System.Drawing.Size(80, 31);
            this.btn_Measure.TabIndex = 15;
            this.btn_Measure.Text = "测量";
            this.btn_Measure.UseVisualStyleBackColor = true;
            // 
            // btn_CircleFit
            // 
            this.btn_CircleFit.Location = new System.Drawing.Point(339, 146);
            this.btn_CircleFit.Name = "btn_CircleFit";
            this.btn_CircleFit.Size = new System.Drawing.Size(80, 31);
            this.btn_CircleFit.TabIndex = 13;
            this.btn_CircleFit.Text = "拟合";
            this.btn_CircleFit.UseVisualStyleBackColor = true;
            // 
            // btn_CircleROI
            // 
            this.btn_CircleROI.Location = new System.Drawing.Point(247, 146);
            this.btn_CircleROI.Name = "btn_CircleROI";
            this.btn_CircleROI.Size = new System.Drawing.Size(80, 31);
            this.btn_CircleROI.TabIndex = 14;
            this.btn_CircleROI.Text = "圆ROI";
            this.btn_CircleROI.UseVisualStyleBackColor = true;
            // 
            // cmb_CirclePointSelect
            // 
            this.cmb_CirclePointSelect.FormattingEnabled = true;
            this.cmb_CirclePointSelect.Location = new System.Drawing.Point(310, 34);
            this.cmb_CirclePointSelect.Name = "cmb_CirclePointSelect";
            this.cmb_CirclePointSelect.Size = new System.Drawing.Size(109, 31);
            this.cmb_CirclePointSelect.TabIndex = 12;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(238, 38);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 23);
            this.label11.TabIndex = 11;
            this.label11.Text = "点选择";
            // 
            // cmb_CircleTransition
            // 
            this.cmb_CircleTransition.FormattingEnabled = true;
            this.cmb_CircleTransition.Location = new System.Drawing.Point(110, 150);
            this.cmb_CircleTransition.Name = "cmb_CircleTransition";
            this.cmb_CircleTransition.Size = new System.Drawing.Size(109, 31);
            this.cmb_CircleTransition.TabIndex = 10;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(24, 193);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(78, 23);
            this.label17.TabIndex = 9;
            this.label17.Text = "实际距离";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(38, 154);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 23);
            this.label10.TabIndex = 9;
            this.label10.Text = "极性";
            // 
            // txt_Distance
            // 
            this.txt_Distance.Location = new System.Drawing.Point(110, 191);
            this.txt_Distance.Name = "txt_Distance";
            this.txt_Distance.Size = new System.Drawing.Size(109, 29);
            this.txt_Distance.TabIndex = 6;
            this.txt_Distance.Text = "100";
            this.txt_Distance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_CircleSigma
            // 
            this.txt_CircleSigma.Location = new System.Drawing.Point(110, 111);
            this.txt_CircleSigma.Name = "txt_CircleSigma";
            this.txt_CircleSigma.Size = new System.Drawing.Size(109, 29);
            this.txt_CircleSigma.TabIndex = 6;
            this.txt_CircleSigma.Text = "0";
            this.txt_CircleSigma.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 115);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 23);
            this.label8.TabIndex = 3;
            this.label8.Text = "平滑系数";
            // 
            // txt_DisMax
            // 
            this.txt_DisMax.Location = new System.Drawing.Point(309, 108);
            this.txt_DisMax.Name = "txt_DisMax";
            this.txt_DisMax.Size = new System.Drawing.Size(109, 29);
            this.txt_DisMax.TabIndex = 7;
            this.txt_DisMax.Text = "0";
            this.txt_DisMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_DisMin
            // 
            this.txt_DisMin.Location = new System.Drawing.Point(309, 72);
            this.txt_DisMin.Name = "txt_DisMin";
            this.txt_DisMin.Size = new System.Drawing.Size(109, 29);
            this.txt_DisMin.TabIndex = 7;
            this.txt_DisMin.Text = "0";
            this.txt_DisMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_CircleThreshold
            // 
            this.txt_CircleThreshold.Location = new System.Drawing.Point(110, 72);
            this.txt_CircleThreshold.Name = "txt_CircleThreshold";
            this.txt_CircleThreshold.Size = new System.Drawing.Size(109, 29);
            this.txt_CircleThreshold.TabIndex = 7;
            this.txt_CircleThreshold.Text = "0";
            this.txt_CircleThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(230, 111);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(78, 23);
            this.label19.TabIndex = 4;
            this.label19.Text = "直径高限";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(229, 75);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(78, 23);
            this.label18.TabIndex = 4;
            this.label18.Text = "直径低限";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 23);
            this.label7.TabIndex = 4;
            this.label7.Text = "边缘阈值";
            // 
            // txt_CircleElements
            // 
            this.txt_CircleElements.Location = new System.Drawing.Point(110, 33);
            this.txt_CircleElements.Name = "txt_CircleElements";
            this.txt_CircleElements.Size = new System.Drawing.Size(109, 29);
            this.txt_CircleElements.TabIndex = 8;
            this.txt_CircleElements.Text = "0";
            this.txt_CircleElements.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(26, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 23);
            this.label9.TabIndex = 5;
            this.label9.Text = "搜索点数";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 675);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gb1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.hWindowControl1);
            this.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormMain";
            this.Text = "零件测量";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Gain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_ExposureTime)).EndInit();
            this.gb1.ResumeLayout(false);
            this.gb1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView list_Info;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar tb_Gain;
        private System.Windows.Forms.Button btn_OneShot;
        private System.Windows.Forms.TextBox txt_ExposureTime;
        private System.Windows.Forms.TextBox txt_Gain;
        private System.Windows.Forms.TrackBar tb_ExposureTime;
        private System.Windows.Forms.Button btn_Grab;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btn_StopGrab;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_OpenImage;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btn_SaveImage;
        private System.Windows.Forms.CheckBox chk_RealTimeProc;
        private System.Windows.Forms.CheckBox chk_Trigger;
        private System.Windows.Forms.GroupBox gb1;
        private System.Windows.Forms.Button btn_ContinueFind;
        private System.Windows.Forms.Button btn_FindShapeModel;
        private System.Windows.Forms.Button btn_CreatShapeModel;
        private System.Windows.Forms.TextBox txt_NumMatchs;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_Overlap;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_Score;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_Range;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txt_Angle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_Calib;
        private System.Windows.Forms.Button btn_Measure;
        private System.Windows.Forms.Button btn_CircleFit;
        private System.Windows.Forms.Button btn_CircleROI;
        private System.Windows.Forms.ComboBox cmb_CirclePointSelect;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmb_CircleTransition;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt_Distance;
        private System.Windows.Forms.TextBox txt_CircleSigma;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_DisMax;
        private System.Windows.Forms.TextBox txt_DisMin;
        private System.Windows.Forms.TextBox txt_CircleThreshold;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_CircleElements;
        private System.Windows.Forms.Label label9;
    }
}

