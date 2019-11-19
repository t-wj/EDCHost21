using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

using OpenCvSharp;
using System.Threading;
using OpenCvSharp.Extensions;
using Point2i = OpenCvSharp.Point;
using Cuda = OpenCvSharp.Cuda;

using System.Net;
using System.IO.Ports;

namespace EDC21HOST
{
    public partial class Tracker : Form
    {
        public MyFlags flags = null;
        public VideoCapture capture = null;
        //private Thread threadCamera = null;
        private Point2f[] ptsShowCorners = null;
        private DateTime timeCamNow;
        private DateTime timeCamPrev;
        public CoordinateConverter cc;
        private Localiser localiser;
        private Point2f[] ball;
        private Point2i car1;
        private Point2i car2;
        private Game game;
        private VideoWriter vw = null;

        private bool alreadySet;
        public SerialPort serial;
        public string[] validPorts;

        private string[] gametext = { "上半场", "下半场", "加时1", "加时2",
            "加时3", "加时4", "加时5", "加时6", "加时7" , "加时8", "加时9", "加时10", "加时11", "加时12"};
        private Camp[] UI_LastRoundCamp = new Camp[5];

        public Dot CarALocation()
        {
            Dot D = new Dot();
            D.x = car1.X;
            D.y = car1.Y;
            return D;
        }
        public Dot CarBLocation()
        {
            Dot D = new Dot();
            D.x = car2.X;
            D.y = car2.Y;
            return D;
        }

        public Tracker()
        {
            InitializeComponent();
            //UI

            label_RedBG.SendToBack();
            label_BlueBG.SendToBack();
            label_RedBG.Controls.Add(label_CarA);
            label_RedBG.Controls.Add(labelAScore);
            label_BlueBG.Controls.Add(label_CarB);
            int newX = label_CarB.Location.X - label_BlueBG.Location.X;
            int newY = label_CarB.Location.Y - label_BlueBG.Location.Y;
            label_CarB.Location = new System.Drawing.Point(newX, newY);
            label_BlueBG.Controls.Add(labelBScore);
            newX = labelBScore.Location.X - label_BlueBG.Location.X;
            newY = labelBScore.Location.Y - label_BlueBG.Location.Y;
            labelBScore.Location = new System.Drawing.Point(newX, newY);
            label_GameCount.Text = "上半场";

            // Init
            flags = new MyFlags();
            flags.Init();
            flags.Start();
            capture = new VideoCapture();
            // threadCamera = new Thread(CameraReading);
            capture.Open(0);
            flags.cameraSize.Width = capture.FrameWidth;
            flags.cameraSize.Height = capture.FrameHeight;
            flags.showSize.Width = pbCamera.Width;
            flags.showSize.Height = pbCamera.Height;
            ptsShowCorners = new Point2f[4];
            cc = new CoordinateConverter(flags);
            localiser = new Localiser();
            timeCamNow = DateTime.Now;
            timeCamPrev = timeCamNow;

            ball = new Point2f[0];
            car1 = new Point2i();
            car2 = new Point2i();

            buttonStart.Enabled = true;
            buttonPause.Enabled = false;
            buttonEnd.Enabled = false;
            button_AReset.Enabled = false;
            button_BReset.Enabled = false;

            validPorts = SerialPort.GetPortNames();
            //try
            //{
            //    if (validPorts.Any())
            //    {
            //        serial = new SerialPort(validPorts[0], 115200, Parity.None, 8, StopBits.One);
            //        serial.Open();
            //    }
            //}
            //catch (UnauthorizedAccessException)
            //{

            //}
            alreadySet = false;

            //Game.LoadMap();
            game = new Game();

            if (capture.IsOpened())
            {
                capture.FrameWidth = flags.cameraSize.Width;
                capture.FrameHeight = flags.cameraSize.Height;
                capture.ConvertRgb = true;
                timer100ms.Interval = 75;
                timer100ms.Start();
                //Cv2.NamedWindow("binary");
            }

        }

        private void Flush()
        {
            if (!alreadySet)
            {
                lock (flags)
                {
                    SetWindow st = new SetWindow(ref flags, ref game, this);
                    st.Show();
                }
                alreadySet = true;
            }
            CameraReading();
            lock (flags)
            {
                game.BallsDot.Clear();
                foreach (Point2i posBall in flags.posBalls)
                    game.BallsDot.Add(new Dot(posBall.X, posBall.Y));
                game.CarA.Pos.x = flags.posCarA.X;
                game.CarA.Pos.y = flags.posCarA.Y;
                game.CarB.Pos.x = flags.posCarB.X;
                game.CarB.Pos.y = flags.posCarB.Y;
            }
            game.Update();
            lock (flags)
            {
                flags.currPersonNum = game.CurrPersonNumber;
                for (int i = 0; i != Game.MaxPersonNum; ++i)
                {
                    flags.posPersonStart[i].X = game.People[i].StartPos.x;
                    flags.posPersonStart[i].Y = game.People[i].StartPos.y;
                    flags.gameState = game.State;
                }
            }
            byte[] Message = game.PackMessage();
            label_CountDown.Text = Convert.ToString(game.Round);
            if (serial != null && serial.IsOpen)
                serial.Write(Message, 0, 32);
            ShowMessage(Message);
            validPorts = SerialPort.GetPortNames();
        }

        private void CameraReading()
        {
            bool control = false;
            lock (flags)
            {
                control = flags.running;
            }
            if (control)
            {
                using (Mat videoFrame = new Mat())
                using (Mat showFrame = new Mat())
                {
                    if (capture.Read(videoFrame))
                    {
                        lock (flags)
                        {
                            cc.PeopleFilter(flags);
                            localiser.Locate(videoFrame, flags);

                            //绘制边界点
                            foreach (Point2f pt in cc.ShowToCamera(ptsShowCorners))
                            {
                                Cv2.Line(videoFrame, (int)(pt.X - 3), (int)(pt.Y), (int)(pt.X + 3), (int)(pt.Y), new Scalar(0x00, 0xff, 0x98));
                                Cv2.Line(videoFrame, (int)(pt.X), (int)(pt.Y - 3), (int)(pt.X), (int)(pt.Y + 3), new Scalar(0x00, 0xff, 0x98));
                            }
                        }
                        localiser.GetLocations(out ball, out car1, out car2);

                        lock (flags)
                        {
                            Point2f[] posBallsF = new Point2f[0];
                            if (flags.calibrated)
                            {
                                if (ball.Any())
                                    posBallsF = cc.CameraToLogic(ball);
                                Point2f[] car12 = { car1, car2 };
                                Point2f[] carAB = cc.CameraToLogic(car12);
                                flags.posCarA = carAB[0];
                                flags.posCarB = carAB[1];
                            }
                            else
                            {
                                posBallsF = ball;
                                flags.posCarA = car1;
                                flags.posCarB = car2;
                            }
                            Point2i[] posBallsI = new Point2i[posBallsF.Length];
                            for (int i = 0; i < posBallsF.Length; ++i)
                                posBallsI[i] = posBallsF[i];
                            List<Point2i> posBallsList = new List<Point2i>();
                            foreach (Point2i b in posBallsI)
                            {
                                if (!posBallsList.Any(bb => b.DistanceTo(bb) < Game.MinBallSept))
                                    posBallsList.Add(b);
                            }
                            flags.posBalls = posBallsList.ToArray();
                        }
                        timeCamNow = DateTime.Now;
                        TimeSpan timeProcess = timeCamNow - timeCamPrev;
                        timeCamPrev = timeCamNow;
                        Cv2.Resize(videoFrame, showFrame, flags.showSize, 0, 0, InterpolationFlags.Nearest);
                        BeginInvoke(new Action<Image>(UpdateCameraPicture), BitmapConverter.ToBitmap(showFrame));
                        //输出视频
                        if (flags.videomode == true)
                            vw.Write(showFrame);
                    }
                    lock (flags)
                    {
                        control = flags.running;
                    }
                }
            }
        }

        private void UpdateCameraPicture(Image img)
        {
            pbCamera.Image = img;
        }

        private void Tracker_FormClosed(object sender, FormClosedEventArgs e)
        {
            lock (flags)
            {
                flags.End();
            }
            timer100ms.Stop();
            //threadCamera.Join();
            capture.Release();
            if (serial != null && serial.IsOpen)
                serial.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            lock (flags)
            {
                flags.clickCount = 0;
                flags.calibrated = false;
                for (int i = 0; i < 4; ++i)
                    ptsShowCorners[i].X = ptsShowCorners[i].Y = 0;
            }
        }

        private void pbCamera_MouseClick(object sender, MouseEventArgs e)
        {
            int widthView = pbCamera.Width;
            int heightView = pbCamera.Height;

            int xMouse = e.X;
            int yMouse = e.Y;

            int idx = -1;
            lock (flags)
            {
                if (flags.clickCount < 4) idx = flags.clickCount++;
            }

            if (idx == -1) return;

            if (xMouse >= 0 && xMouse < widthView && yMouse >= 0 && yMouse < heightView)
            {
                ptsShowCorners[idx].X = xMouse;
                ptsShowCorners[idx].Y = yMouse;
                if (idx == 3)
                {
                    cc.UpdateCorners(ptsShowCorners, flags);
                    MessageBox.Show($"边界点设置完成\n"
                        + $"0: {ptsShowCorners[0].X,5}, {ptsShowCorners[0].Y,5}\t"
                        + $"1: {ptsShowCorners[1].X,5}, {ptsShowCorners[1].Y,5}\n"
                        + $"2: {ptsShowCorners[2].X,5}, {ptsShowCorners[2].Y,5}\t"
                        + $"3: {ptsShowCorners[3].X,5}, {ptsShowCorners[3].Y,5}");
                }
            }
        }

        private void timer100ms_Tick(object sender, EventArgs e)
        {
            Flush();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            game.Start();
            buttonPause.Enabled = true;
            buttonEnd.Enabled = true;
            buttonStart.Enabled = false;
            button_AReset.Enabled = true;
            button_BReset.Enabled = true;
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            game.Pause();
            buttonPause.Enabled = false;
            buttonEnd.Enabled = true;
            buttonStart.Enabled = true;
            button_AReset.Enabled = false;
            button_BReset.Enabled = false;
        }

        private void ShowMessage(byte[] M) //通过Message显示信息到UI上
        {
            label_CountDown.Text = $"{(game.MaxRound - game.Round) / 600}:{((game.MaxRound - game.Round) / 10) % 60 / 10}{((game.MaxRound - game.Round) / 10) % 60 % 10}";

            labelAScore.Text = $"{game.CarA.Score}";
            labelBScore.Text = $"{game.CarB.Score}";

            label_GameCount.Text = gametext[game.GameCount - 1];
            label_APauseNum.Text = $"{game.APauseNum}";
            label_BPauseNum.Text = $"{game.BPauseNum}";
            label_AFoul1Num.Text = $"{game.AFoul1}";
            label_BFoul1Num.Text = $"{game.BFoul1}";
            label_AFoul2Num.Text = $"{game.AFoul2}";
            label_BFoul2Num.Text = $"{game.BFoul2}";

            label_AMessage.Text = $"接到人员数　　{game.CarA.PersonCnt}\n抓取物资数　　{game.CarA.BallGetCnt}\n运回物资数　　{game.CarA.BallOwnCnt}";
            label_BMessage.Text = $"{game.CarB.PersonCnt}　　接到人员数\n{game.CarB.BallGetCnt}　　抓取物资数\n{game.CarB.BallOwnCnt}　　运回物资数";
            label_Debug.Text = $"A车坐标：({game.CarA.Pos.x}, {game.CarA.Pos.y})\nB车坐标：({game.CarB.Pos.x}, {game.CarB.Pos.y})\n小球坐标：({game.BallDot.x}, {game.BallDot.y})";
            //if (game.CarA.HaveBonus)
            //    label_CarA.Text = "+" + Car.BonusRate.ToString("0%") + "  " + label_CarA.Text;
            //if (game.CarB.HaveBonus)
            //    label_CarB.Text = label_CarB.Text + "  +" + Car.BonusRate.ToString("0%");
            //  groupBox_Person.Refresh();
        }

        private void button_restart_Click(object sender, EventArgs e)
        {
            lock (game) { game = new Game(); }
            buttonStart.Enabled = true;
            buttonPause.Enabled = false;
            buttonEnd.Enabled = false;
            button_AReset.Enabled = false;
            button_BReset.Enabled = false;
            label_CarA.Text = "A车";
            label_CarB.Text = "B车";
        }

        //private void buttonChangeScore_Click(object sender, EventArgs e)
        //{
        //    int AScore = (int)numericUpDownScoreA.Value;
        //    int BScore = (int)numericUpDownScoreB.Value;
        //    numericUpDownScoreA.Value = 0;
        //    numericUpDownScoreB.Value = 0;
        //    lock (game)
        //    {
        //        game.CarA.Score += AScore;
        //        game.CarB.Score += BScore;
        //    }
        //}



        private void button_video_Click(object sender, EventArgs e)
        {
            lock (flags)
            {
                if (flags.videomode == false)
                {
                    string time = DateTime.Now.ToString("MMdd_HH_mm_ss");
                    vw = new VideoWriter("../../video/" + time + ".avi",
                        FourCC.XVID, 10.0, flags.showSize);
                    flags.videomode = true;
                    ((Button)sender).Text = "停止录像";
                    game.FoulTimeFS = new FileStream("../../video/" + time + ".txt", FileMode.CreateNew);
                }
                else
                {
                    vw.Release();
                    vw = null;
                    flags.videomode = false;
                    ((Button)sender).Text = "开始录像";
                    game.FoulTimeFS = null;
                }
            }
        }

        private void button_AReset_Click(object sender, EventArgs e)
        {
            if (game.APauseNum < 3)
            {
                game.AskPause(Camp.CampA);
                buttonPause.Enabled = false;
                buttonEnd.Enabled = false;
                buttonStart.Enabled = true;
                button_AReset.Enabled = false;
                button_BReset.Enabled = false;
            }
        }

        private void button_BReset_Click(object sender, EventArgs e)
        {
            if (game.BPauseNum < 3)
            {
                game.AskPause(Camp.CampB);
                buttonPause.Enabled = false;
                buttonEnd.Enabled = false;
                buttonStart.Enabled = true;
                button_AReset.Enabled = false;
                button_BReset.Enabled = false;
            }
        }

        private void button_set_Click(object sender, EventArgs e)
        {
            lock (flags)
            {
                SetWindow st = new SetWindow(ref flags, ref game, this);
                st.Show();
            }
        }

        //private void numericUpDownScoreA_ValueChanged(object sender, EventArgs e)
        //{
        //    game.AddScore(Camp.CampA, (int)((NumericUpDown)sender).Value);
        //    ((NumericUpDown)sender).Value = 0;
        //}

        //private void numericUpDownScoreB_ValueChanged(object sender, EventArgs e)
        //{
        //    game.AddScore(Camp.CampB, (int)((NumericUpDown)sender).Value);
        //    ((NumericUpDown)sender).Value = 0;
        //}

        private void Tracker_Load(object sender, EventArgs e)
        {
            if (File.Exists("data.txt"))
            {
                FileStream fsRead = new FileStream("data.txt", FileMode.Open);
                int fsLen = (int)fsRead.Length;
                byte[] heByte = new byte[fsLen];
                int r = fsRead.Read(heByte, 0, heByte.Length);
                string myStr = System.Text.Encoding.UTF8.GetString(heByte);
                string[] str = myStr.Split(' ');
                flags.configs.hue0Lower = Convert.ToInt32(str[0]);
                flags.configs.hue0Upper = Convert.ToInt32(str[1]);
                flags.configs.hue1Lower = Convert.ToInt32(str[2]);
                flags.configs.hue1Upper = Convert.ToInt32(str[3]);
                flags.configs.hue2Lower = Convert.ToInt32(str[4]);
                flags.configs.hue2Upper = Convert.ToInt32(str[5]);
                flags.configs.saturation0Lower = Convert.ToInt32(str[6]);
                flags.configs.saturation1Lower = Convert.ToInt32(str[7]);
                flags.configs.saturation2Lower = Convert.ToInt32(str[8]);
                flags.configs.valueLower = Convert.ToInt32(str[9]);
                flags.configs.areaLower = Convert.ToInt32(str[10]);
                fsRead.Close();
            }
        }

        private void button_Continue_Click(object sender, EventArgs e)
        {
            //if (game.state == GameState.End)
            game.NextStage();
            buttonPause.Enabled = false;
            buttonEnd.Enabled = true;
            buttonStart.Enabled = true;
            button_AReset.Enabled = false;
            button_BReset.Enabled = false;
        }

        private void button_AFoul1_Click(object sender, EventArgs e)
        {
            game.AFoul1++;
            game.AddScore(Camp.CampA, Score.Foul1);
            if (game.FoulTimeFS != null)
            {
                byte[] data = Encoding.Default.GetBytes($"A -10 {game.Round}\r\n");
                game.FoulTimeFS.Write(data, 0, data.Length);
            }
        }

        private void button_AFoul2_Click(object sender, EventArgs e)
        {
            game.AFoul2++;
            game.AddScore(Camp.CampA, Score.Foul2);
            if (game.FoulTimeFS != null)
            {
                byte[] data = Encoding.Default.GetBytes($"A -50 {game.Round}\r\n");
                game.FoulTimeFS.Write(data, 0, data.Length);
            }
        }

        private void button_BFoul1_Click(object sender, EventArgs e)
        {
            game.BFoul1++;
            game.AddScore(Camp.CampB, Score.Foul1);
            if (game.FoulTimeFS != null)
            {
                byte[] data = Encoding.Default.GetBytes($"B -10 {game.Round}\r\n");
                game.FoulTimeFS.Write(data, 0, data.Length);
            }
        }

        private void button_BFoul2_Click(object sender, EventArgs e)
        {
            game.BFoul2++;
            game.AddScore(Camp.CampB, Score.Foul2);
            if (game.FoulTimeFS != null)
            {
                byte[] data = Encoding.Default.GetBytes($"B -50 {game.Round}\r\n");
                game.FoulTimeFS.Write(data, 0, data.Length);
            }
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            game.End();
            buttonStart.Enabled = true;
            buttonPause.Enabled = false;
            buttonEnd.Enabled = false;
            button_AReset.Enabled = false;
            button_BReset.Enabled = false;
        }

        ////绘制人员信息
        //private void groupBox_Person_Paint(object sender, PaintEventArgs e)
        //{
        //    Brush br_No_NV = new SolidBrush(Color.Silver);
        //    Brush br_No_V = new SolidBrush(Color.DimGray);
        //    Brush br_A_NV = new SolidBrush(Color.Pink);
        //    Brush br_A_V = new SolidBrush(Color.Red);
        //    Brush br_B_NV = new SolidBrush(Color.SkyBlue);
        //    Brush br_B_V = new SolidBrush(Color.RoyalBlue);
        //    Graphics gra = e.Graphics;
        //    int vbargin = 100;
        //    for(int i = 0;i!=game.CurrPersonNumber;++i)
        //    {
        //        switch(game.People[i].Owner)
        //        {
        //            case Camp.None:
        //                gra.FillEllipse(br_No_V, 40, 100 + i * vbargin, 30, 30);
        //                gra.FillEllipse(br_A_NV, 100, 100 + i * vbargin, 30, 30);
        //                gra.FillEllipse(br_B_NV, 160, 100 + i * vbargin, 30, 30);
        //                break;
        //            case Camp.CampA:
        //                gra.FillEllipse(br_No_NV, 40, 100 + i * vbargin, 30, 30);
        //                gra.FillEllipse(br_A_V, 100, 100 + i * vbargin, 30, 30);
        //                gra.FillEllipse(br_B_NV, 160, 100 + i * vbargin, 30, 30);
        //                break;
        //            case Camp.CampB:
        //                gra.FillEllipse(br_No_NV, 40, 100 + i * vbargin, 30, 30);
        //                gra.FillEllipse(br_A_NV, 100, 100 + i * vbargin, 30, 30);
        //                gra.FillEllipse(br_B_V, 160, 100 + i * vbargin, 30, 30);
        //                break;
        //            default:break;
        //        }
        //    }
        //}
    }

    public class MyFlags
    {
        public bool showMask; //调试颜色识别
        public bool running;
        public bool calibrated;
        public bool videomode;
        public int clickCount;
        public struct LocConfigs
        {
            public int hue0Lower;
            public int hue0Upper;
            public int hue1Lower;
            public int hue1Upper;
            public int hue2Lower;
            public int hue2Upper;
            public int saturation0Lower;
            public int saturation1Lower;
            public int saturation2Lower;
            public int valueLower;
            public int areaLower;
        }
        public LocConfigs configs;
        public OpenCvSharp.Size showSize;
        public OpenCvSharp.Size cameraSize;
        public OpenCvSharp.Size logicSize;
        public Point2i[] posBalls;
        public Point2i posCarA;
        public Point2i posCarB;

        public Point2i[] posPersonStart;
        public int currPersonNum;
        public GameState gameState;

        public void Init()
        {
            showMask = false;
            running = false;
            calibrated = false;
            videomode = false;
            configs = new LocConfigs();
            posBalls = new Point2i[0];
            posCarA = new Point2i();
            posCarB = new Point2i();
            showSize = new OpenCvSharp.Size(960, 720);
            cameraSize = new OpenCvSharp.Size(1280, 960);
            logicSize = new OpenCvSharp.Size(Game.MaxSize, Game.MaxSize);
            clickCount = 0;
            posPersonStart = new Point2i[Game.MaxPersonNum];
        }

        public void Start()
        {
            running = true;
        }

        public void End()
        {
            running = false;
        }
    }

    public class CoordinateConverter : IDisposable
    {
        private Mat cam2logic;
        private Mat logic2cam;
        private Mat show2cam;
        private Mat cam2show;
        private Mat show2logic;
        private Mat logic2show;
        private Point2f[] logicCorners;
        private Point2f[] camCorners;
        private Point2f[] showCorners;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)(cam2logic)).Dispose();
                ((IDisposable)(logic2cam)).Dispose();
                ((IDisposable)(show2cam)).Dispose();
                ((IDisposable)(cam2show)).Dispose();
                ((IDisposable)(show2logic)).Dispose();
                ((IDisposable)(logic2show)).Dispose();
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public CoordinateConverter(MyFlags myFlags)
        {
            camCorners = new Point2f[4];
            logicCorners = new Point2f[4];
            showCorners = new Point2f[4];
            cam2logic = new Mat();
            show2cam = new Mat();
            logic2show = new Mat();
            show2logic = new Mat();
            cam2show = new Mat();
            logic2cam = new Mat();

            logicCorners[0].X = 0;
            logicCorners[0].Y = 0;
            logicCorners[1].X = myFlags.logicSize.Width;
            logicCorners[1].Y = 0;
            logicCorners[2].X = 0;
            logicCorners[2].Y = myFlags.logicSize.Height;
            logicCorners[3].X = myFlags.logicSize.Width;
            logicCorners[3].Y = myFlags.logicSize.Height;

            showCorners[0].X = 0;
            showCorners[0].Y = 0;
            showCorners[1].X = myFlags.showSize.Width;
            showCorners[1].Y = 0;
            showCorners[2].X = 0;
            showCorners[2].Y = myFlags.showSize.Height;
            showCorners[3].X = myFlags.showSize.Width;
            showCorners[3].Y = myFlags.showSize.Height;

            camCorners[0].X = 0;
            camCorners[0].Y = 0;
            camCorners[1].X = myFlags.cameraSize.Width;
            camCorners[1].Y = 0;
            camCorners[2].X = 0;
            camCorners[2].Y = myFlags.cameraSize.Height;
            camCorners[3].X = myFlags.cameraSize.Width;
            camCorners[3].Y = myFlags.cameraSize.Height;

            show2cam = Cv2.GetPerspectiveTransform(showCorners, camCorners);
            cam2show = Cv2.GetPerspectiveTransform(camCorners, showCorners);
        }

        public void UpdateCorners(Point2f[] corners, MyFlags myFlags)
        {
            if (corners == null) return;
            if (corners.Length != 4) return;
            else showCorners = corners;

            logic2show = Cv2.GetPerspectiveTransform(logicCorners, showCorners);
            show2logic = Cv2.GetPerspectiveTransform(showCorners, logicCorners);
            camCorners = Cv2.PerspectiveTransform(showCorners, show2cam);
            cam2logic = Cv2.GetPerspectiveTransform(camCorners, logicCorners);
            logic2cam = Cv2.GetPerspectiveTransform(logicCorners, camCorners);
            myFlags.calibrated = true;
        }

        public Point2f[] ShowToCamera(Point2f[] ptsShow)
        {
            return Cv2.PerspectiveTransform(ptsShow, show2cam);
        }

        public Point2f[] CameraToShow(Point2f[] ptsCamera)
        {
            return Cv2.PerspectiveTransform(ptsCamera, cam2show);
        }

        public Point2f[] CameraToLogic(Point2f[] ptsCamera)
        {
            return Cv2.PerspectiveTransform(ptsCamera, cam2logic);
        }

        public Point2f[] LogicToCamera(Point2f[] ptsLogic)
        {
            return Cv2.PerspectiveTransform(ptsLogic, logic2cam);
        }

        public Point2f[] LogicToShow(Point2f[] ptsLogic)
        {
            return Cv2.PerspectiveTransform(ptsLogic, logic2show);
        }

        public Point2f[] ShowToLogic(Point2f[] ptsShow)
        {
            return Cv2.PerspectiveTransform(ptsShow, show2logic);
        }

        public void PeopleFilter(MyFlags flags)
        {
            if (!flags.calibrated) return;

            for (int i = 0; i < flags.currPersonNum; ++i)
            {
                Point2f[] res = LogicToCamera(new Point2f[] { flags.posPersonStart[i] });
                flags.posPersonStart[i] = res[0];
            }
        }
    }

    public class Localiser
    {
        private List<Point2i> centres0;
        private List<Point2i> centres1;
        private List<Point2i> centres2;

        public Localiser()
        {
            centres0 = new List<Point2i>();
            centres1 = new List<Point2i>();
            centres2 = new List<Point2i>();

        }

        public void GetLocations(out Point2f[] pts0, out Point2i pt1, out Point2i pt2)
        {
            List<Point2f> ptsList0 = new List<Point2f>();
            if (centres0.Count != 0)
            {
                foreach (Point2i c0 in centres0)
                    ptsList0.Add(c0);
                centres0.Clear();
            }
            // else ptsList0.Add(new Point2f(-1, -1));
            pts0 = ptsList0.ToArray();

            if (centres1.Count != 0)
            {
                pt1 = centres1[0];
                centres1.Clear();
            }
            else pt1 = new Point2i(-1, -1);
            if (centres2.Count != 0)
            {
                pt2 = centres2[0];
                centres2.Clear();
            }
            else pt2 = new Point2i(-1, -1);
        }

        public void Locate(Mat mat, MyFlags localiseFlags)
        {
            if (mat == null || mat.Empty()) return;
            if (localiseFlags == null) return;
            using (Mat hsv = new Mat())
            using (Mat ball = new Mat())
            using (Mat car1 = new Mat())
            using (Mat car2 = new Mat())
            //using (Mat merged = new Mat())
            using (Mat black = new Mat(mat.Size(), MatType.CV_8UC1))
            {
                Cv2.CvtColor(mat, hsv, ColorConversionCodes.RGB2HSV);
                MyFlags.LocConfigs configs = localiseFlags.configs;
                Cv2.InRange(hsv,
                    new Scalar(configs.hue0Lower, configs.saturation0Lower, configs.valueLower),
                    new Scalar(configs.hue0Upper, 255, 255),
                    ball);
                Cv2.InRange(hsv,
                    new Scalar(configs.hue1Lower, configs.saturation1Lower, configs.valueLower),
                    new Scalar(configs.hue1Upper, 255, 255),
                    car1);
                Cv2.InRange(hsv,
                    new Scalar(configs.hue2Lower, configs.saturation2Lower, configs.valueLower),
                    new Scalar(configs.hue2Upper, 255, 255),
                    car2);

                if (localiseFlags.showMask)
                {
                    Cv2.ImShow("Ball", ball);
                    Cv2.ImShow("CarA", car1);
                    Cv2.ImShow("CarB", car2);
                }
                else
                {
                    Cv2.DestroyAllWindows();
                }

                Point2i[][] contours0, contours1, contours2;

                contours0 = Cv2.FindContoursAsArray(ball, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                contours1 = Cv2.FindContoursAsArray(car1, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                contours2 = Cv2.FindContoursAsArray(car2, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                foreach (Point2i[] c0 in contours0)
                {
                    Point2i centre = new Point2i();
                    Moments moments = Cv2.Moments(c0);
                    centre.X = (int)(moments.M10 / moments.M00);
                    centre.Y = (int)(moments.M01 / moments.M00);
                    double area = moments.M00;
                    if (area <= configs.areaLower / 4) continue;
                    centres0.Add(centre);
                }
                foreach (Point2i[] c1 in contours1)
                {
                    Point2i centre = new Point2i();
                    Moments moments = Cv2.Moments(c1);
                    centre.X = (int)(moments.M10 / moments.M00);
                    centre.Y = (int)(moments.M01 / moments.M00);
                    double area = moments.M00;
                    if (area <= configs.areaLower) continue;
                    centres1.Add(centre);
                }
                foreach (Point2i[] c2 in contours2)
                {
                    Point2i centre = new Point2f();
                    Moments moments = Cv2.Moments(c2);
                    centre.X = (int)(moments.M10 / moments.M00);
                    centre.Y = (int)(moments.M01 / moments.M00);
                    double area = moments.M00;
                    if (area <= configs.areaLower) continue;
                    centres2.Add(centre);
                }

                foreach (Point2i c0 in centres0) Cv2.Circle(mat, c0, 3, new Scalar(0x1b, 0xa7, 0xff), -1);
                foreach (Point2i c1 in centres1) Cv2.Circle(mat, c1, 10, new Scalar(0x1b, 0xff, 0xa7), -1);
                foreach (Point2i c2 in centres2) Cv2.Circle(mat, c2, 10, new Scalar(0x00, 0x98, 0xff), -1);
                if (localiseFlags.gameState != GameState.Unstart)
                {
                    for (int i = 0; i < localiseFlags.currPersonNum; ++i)
                    {
                        int x10 = localiseFlags.posPersonStart[i].X - 8;
                        int y10 = localiseFlags.posPersonStart[i].Y - 8;
                        Cv2.Rectangle(mat, new Rect(x10, y10, 16, 16), new Scalar(0xf3, 0x96, 0x21), -1);
                    }
                }

                //Cv2.Merge(new Mat[] { car1, car2, black }, merged);
                //Cv2.ImShow("binary", merged);
            }
        }
    }
}

