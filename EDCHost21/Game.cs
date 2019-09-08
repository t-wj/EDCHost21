using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace EDC21HOST
{
    public enum GameState { Unstart = 0, Normal = 1, Pause = 2, End = 3 };
    public enum BallState { NotCarry = 0, Carry, GetScore };
    public class Game
    {
        public bool DebugMode; //调试模式，最大回合数 = 1,000,000
        public const int MaxSize = 270;
        public const int MaxPersonNum = 4;
        public const int MazeCrossNum = 6;
        public const int MazeCrossDist = 30;
        public const int MazeBorderPoint1 = 37;
        public const int MazeBorderPoint2 = MazeBorderPoint1 + MazeCrossDist * (1 + MazeCrossNum);
        public const int MaxCarryDistance = 10; //接上人员的最大距离
        public const int MaxCarBallDistance = 30; //拿到小球的最大距离
        public const int MinBallSept = 8; //小球最小可分辨距离
        public const int CollectBound = 36;
        public const int StorageBound = 43;
        public const int PersonGetScore = 15;
        public const int BallGetScore = 10;
        public const int BallOwnScore = 10;
        public const int BallOppoScore = 5;
        public int APauseNum = 0;
        public int BPauseNum = 0;
        public int AFoul1 = 0;
        public int AFoul2 = 0;
        public int BFoul1 = 0;
        public int BFoul2 = 0;
        public int MaxRound;  //最大回合数
        public int GameCount; //上下半场、加时等
        public int Round { get; set; }//当前回合
        public GameState state { get; set; }
        public Car CarA, CarB;
        public Person[] People;
        public List<Dot> BallsDot; //小球位置
        public Dot BallAtCollect; //物资收集点处的小球位置
        public PersonGenerator Generator { get; set; }
        public int CurrPersonNumber; //当前人员数量
        //public static bool[,] GameMap = new bool[MaxSize, MaxSize]; //地图信息
        public FileStream FoulTimeFS;
        public static byte CarInMaze(Dot dot)//车点是否有效->3*3方格白色大于4
        {
            //return true;
            int count = 0;
            for (int i = ((dot.x - 1 > 0) ? (dot.x - 1) : 0); i <= ((dot.x + 1 < MaxSize) ? (dot.x + 1) : MaxSize - 1); ++i)
                for (int j = ((dot.y - 1 > 0) ? (dot.y - 1) : 0); j <= ((dot.y + 1 < MaxSize) ? (dot.y + 1) : MaxSize - 1); ++j)
                    if (i >= MazeBorderPoint1 && i <= MazeBorderPoint2 && j >= MazeBorderPoint1 && j <= MazeBorderPoint2)
                        count++;
            if (count >= 4) return 1;
            else return 0;
        }
        //public static void LoadMap()//读取地图文件
        //{
        //    //FileStream MapFile = File.OpenRead("../../map/map.bmp");
        //    //byte[] buffer = new byte[MapFile.Length - 54]; //存储图片文件
        //    //MapFile.Position = 54;
        //    //MapFile.Read(buffer, 0, buffer.Length);
        //    //for (int i = 0; i != MaxSize; ++i)
        //    //    for (int j = 0; j != MaxSize; ++j)
        //    //        if (buffer[(i * MaxSize + j) * 3 + 2 * i] > 128)//白色
        //    //            GameMap[j, i] = true;
        //    //        else
        //    //            GameMap[j, i] = false;

        //    Bitmap mapData = new Bitmap("../../map/map.bmp");
        //    for (int i = 0; i != MaxSize; ++i)
        //        for (int j = 0; j != MaxSize; ++j)
        //            GameMap[j, i] = mapData.GetPixel(j, i).Equals(Color.FromArgb(255, 255, 255));

        //    //using (StreamWriter sw = new StreamWriter("../../map/map.txt"))
        //    //{
        //    //    for (int i = 0; i != MaxSize; ++i)
        //    //    {
        //    //        for (int j = 0; j != MaxSize; ++j)
        //    //            sw.Write((GameMap[j, i] = mapData.GetPixel(j, i).Equals(Color.FromArgb(255, 255, 255))) ? '1' : '0');
        //    //        sw.Write('\n');
        //    //    }
        //    //}
        //}
        public static Dot OppoDots(Dot prevDot)
        {
            Dot newDots = new Dot();
            newDots.x = prevDot.y;
            newDots.y = prevDot.x;
            return newDots;
        }
        public static double GetDistance(Dot A, Dot B)
        {
            return Math.Sqrt((A.x - B.x) * (A.x - B.x) + (A.y - B.y) * (A.y - B.y));
        }
        public Game()
        {
            GameCount = 1;
            MaxRound = 1200;
            BallsDot = new List<Dot>();
            BallAtCollect = new Dot(0, 0);
            CarA = new Car(Camp.CampA);
            CarB = new Car(Camp.CampB);
            People = new Person[MaxPersonNum];
            Round = 0;
            state = GameState.Unstart;
            InitialPerson();
            DebugMode = false;
            FoulTimeFS = null;
        }

        public void nextStage()
        {
            ++GameCount;
            if (GameCount >= 3)
                MaxRound = 600;
            else
                MaxRound = 1200;
            Round = 0;
            state = GameState.Unstart;
            InitialPerson();
            DebugMode = false;
            if (FoulTimeFS != null)
            {
                byte[] data = Encoding.Default.GetBytes($"nextStage\r\n");
                FoulTimeFS.Write(data, 0, data.Length);
            }
        }

        protected void InitialPerson()//初始化人员
        {
            Generator = new PersonGenerator();
            Generator.Generate(100);
            for (int i = 0; i < MaxPersonNum; ++i)
                People[i] = new Person();
            for (int i = 0; i < MaxPersonNum; ++i)
                People[i] = new Person(Generator.Next(People), i);
            CheckPersonNumber();
        }
        protected void CheckPersonNumber() //根据回合数更改最大人员数量
        {
            CurrPersonNumber = MaxPersonNum;
        }
        public void NewPerson(Dot currentPersonDot, int num) //刷新这一位置的新人员
        {
            Dot temp = new Dot();
            do
            {
                temp = Generator.Next(People);
            }
            while (temp == currentPersonDot); //防止与刚接上人员位置相同
            People[num] = new Person(temp, num);
        }

        //增加分数
        public void addScore(Camp c, int score)
        {
            switch (c)
            {
                case Camp.CampA:
                    CarA.Score += score;
                    if (CarA.Score < 0) CarA.Score = 0;
                    return;
                case Camp.CampB:
                    CarB.Score += score;
                    if (CarB.Score < 0) CarB.Score = 0;
                    return;
                default: return;
            }
        }

        public void Start() //开始比赛
        {
            state = GameState.Normal;
            CarA.Start();
            CarB.Start();
        }
        public void Pause() //暂停比赛
        {
            state = GameState.Pause;
            CarA.Stop();
            CarB.Stop();
        }
        public void End() //结束比赛
        {
            state = GameState.End;
        }
        //复位
        public void AskPause(Camp c)
        {
            Pause();
            Round -= 50;
            if (Round < 0) Round = 0;
            switch (c)
            {
                case Camp.CampA:
                    ++APauseNum;
                    break;
                case Camp.CampB:
                    ++BPauseNum;
                    break;
            }
        }

        public bool InStorageA(Dot pos)
        {
            return (pos.x <= StorageBound && pos.y >= MaxSize - StorageBound);
        }

        public bool InStorageB(Dot pos)
        {
            return (pos.y <= StorageBound && pos.x >= MaxSize - StorageBound);
        }

        //更新小球相关操作的状态、得分
        public void UpdateBallsState()
        {
            double ballDistanceA = 1000, ballDistanceB = 1000;
            bool currCarryingBallA = false, currCarryingBallB = false;
            bool NoBallInCollect = true;

            foreach (Dot ball in BallsDot)
            {
                if (ball.x < CollectBound && ball.y < CollectBound)
                {
                    BallAtCollect = ball;
                    NoBallInCollect = false;
                }
                if (GetDistance(ball, CarA.Pos) < MaxCarBallDistance)
                {
                    currCarryingBallA = true;
                }
                if (GetDistance(ball, CarB.Pos) < MaxCarBallDistance)
                {
                    currCarryingBallB = true;
                }
            }
            //Console.WriteLine($"{CarA.CarryingBall}, {currCarryingBallA}, {BallAtCollect.x}, {BallAtCollect.y}, {NoBallInCollect}");

            if (!NoBallInCollect && GetDistance(BallAtCollect, CarA.Pos) < MaxCarBallDistance)
            {
                if (CarA.CarryingBall == BallState.NotCarry)
                {
                    CarA.CarryingBall = BallState.Carry;
                    ballDistanceA = GetDistance(BallAtCollect, CarA.Pos);
                }
            }
            if (!NoBallInCollect && GetDistance(BallAtCollect, CarB.Pos) < MaxCarBallDistance)
            {
                if (CarB.CarryingBall == BallState.NotCarry)
                {
                    CarB.CarryingBall = BallState.Carry;
                    ballDistanceB = GetDistance(BallAtCollect, CarB.Pos);
                }
            }

            //小球运输至存放点计分
            if (!currCarryingBallA)
            {
                if (CarA.CarryingBall == BallState.GetScore)
                {
                    if (InStorageA(CarA.Pos))
                    {
                        addScore(Camp.CampA, BallOwnScore);
                        CarA.BallAtOwnCnt++;
                    }
                    else if (InStorageB(CarA.Pos))
                    {
                        addScore(Camp.CampA, BallOppoScore);
                        CarA.BallAtOppoCnt++;
                    }
                }
                CarA.CarryingBall = BallState.NotCarry;
            }
            if (!currCarryingBallB)
            {
                if (CarB.CarryingBall == BallState.GetScore)
                {
                    if (InStorageB(CarB.Pos))
                    {
                        addScore(Camp.CampB, BallOwnScore);
                        CarB.BallAtOwnCnt++;
                    }
                    else if (InStorageA(CarB.Pos))
                    {
                        addScore(Camp.CampB, BallOppoScore);
                        CarB.BallAtOppoCnt++;
                    }
                }
                CarB.CarryingBall = BallState.NotCarry;
            }

            //抓取到小球计分
            if (NoBallInCollect)
            {
                BallAtCollect = new Dot(0, 0);
                if (CarA.CarryingBall == BallState.Carry && CarB.CarryingBall != BallState.Carry)
                {
                    CarA.CarryingBall = BallState.GetScore;
                    addScore(Camp.CampA, BallGetScore);
                    CarA.BallGetCnt++;
                }
                else if (CarA.CarryingBall != BallState.Carry && CarB.CarryingBall == BallState.Carry)
                {
                    CarB.CarryingBall = BallState.GetScore;
                    addScore(Camp.CampB, BallGetScore);
                    CarB.BallGetCnt++;
                }
                else if (CarA.CarryingBall == BallState.Carry && CarB.CarryingBall == BallState.Carry)
                {
                    if (ballDistanceA < ballDistanceB)
                    {
                        CarA.CarryingBall = BallState.GetScore;
                        CarB.CarryingBall = BallState.NotCarry;
                        addScore(Camp.CampA, BallGetScore);
                        CarA.BallGetCnt++;
                    }
                    else
                    {
                        CarB.CarryingBall = BallState.GetScore;
                        CarA.CarryingBall = BallState.NotCarry;
                        addScore(Camp.CampB, BallGetScore);
                        CarB.BallGetCnt++;
                    }
                }
            }
        }
        public void Update()//每回合执行
        {
            if (state == GameState.Normal)
            {
                Round++;
                //GetInfoFromCameraAndUpdate();
                CheckPersonNumber();
                UpdateBallsState();
                #region PunishmentPhase
                //if (!CarDotValid(CarA.Pos)) CarA.Stop();
                //if (!CarDotValid(CarB.Pos)) CarB.Stop();
                #endregion

                //人员上车
                for (int i = 0; i != CurrPersonNumber; ++i)
                {
                    Person p = People[i];
                    if (CarA.UnderStop == false && GetDistance(p.StartPos, CarA.Pos) < MaxCarryDistance)
                    {
                        addScore(Camp.CampA, PersonGetScore);
                        CarA.PersonCnt++;
                        NewPerson(p.StartPos, i);
                    }

                    if (CarB.UnderStop == false && GetDistance(p.StartPos, CarB.Pos) < MaxCarryDistance)
                    {
                        addScore(Camp.CampB, PersonGetScore);
                        CarB.PersonCnt++;
                        NewPerson(p.StartPos, i);
                    }
                }

                if ((Round >= MaxRound && DebugMode == false) || (Round >= 1000000 && DebugMode == true)) //结束比赛
                {
                    End();
                }
            }
            //byte[] message = PackMessage();
            //SendMessage
        } 
        public byte[] PackMessage()
        {
            byte[] message = new byte[56]; //上位机传递的信息
            message[0] = (byte)(Round >> 8);
            message[1] = (byte)Round;
            message[2] = (byte)(((byte)state << 6) | (CarInMaze(CarA.Pos) << 5) | (CarInMaze(CarB.Pos) << 4)
                | (CarA.Pos.x >> 5 & 0x08) | (CarA.Pos.y >> 6 & 0x04) | (CarB.Pos.x >> 7 & 0x02) | (CarB.Pos.y >> 8 & 0x01));
            for (int i = 0; i < MaxPersonNum; ++i)
            {
                message[3] |= (byte)((People[i].StartPos.x & 0x100) >> (2 * i + 1));
                message[3] |= (byte)((People[i].StartPos.y & 0x100) >> (2 * i + 2));
            }
            message[4] = (CarInMaze(CarA.Pos) == 0) ? (byte)CarA.Pos.x : (byte)0;
            message[5] = (CarInMaze(CarA.Pos) == 0) ? (byte)CarA.Pos.y : (byte)0;
            message[6] = (CarInMaze(CarB.Pos) == 0) ? (byte)CarB.Pos.x : (byte)0;
            message[7] = (CarInMaze(CarB.Pos) == 0) ? (byte)CarB.Pos.y : (byte)0;
            for (int i = 0; i < MaxPersonNum; ++i)
            {
                message[8 + i * 2] = (byte)People[i].StartPos.x;
                message[9 + i * 2] = (byte)People[i].StartPos.y;
            }
            message[16] = (byte)BallAtCollect.x;
            message[17] = (byte)BallAtCollect.y;
            message[18] = (byte)(CarA.Score >> 8);
            message[19] = (byte)CarA.Score;
            message[20] = (byte)(CarB.Score >> 8);
            message[21] = (byte)CarB.Score;
            message[22] = (byte)CarA.PersonCnt;
            message[23] = (byte)CarB.PersonCnt;
            message[24] = (byte)CarA.BallGetCnt;
            message[25] = (byte)CarB.BallGetCnt;
            message[26] = (byte)CarA.BallAtOwnCnt;
            message[27] = (byte)CarA.BallAtOppoCnt;
            message[28] = (byte)CarB.BallAtOwnCnt;
            message[29] = (byte)CarB.BallAtOppoCnt;
            message[54] = 0x0D;
            message[55] = 0x0A;
            return message;
        }
    }
}