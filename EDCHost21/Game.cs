using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace EDC21HOST
{
    public enum Score { PersonGetScore, BallGetScore, BallStoreScore, BallCollectScore, GoOutOfMaze, GetBackScore, Foul1, Foul2 };
    public enum GameState { Unstart = 0, Normal = 1, Pause = 2, End = 3 };
    public class Game
    {
        public bool DebugMode; //调试模式，最大回合数 = 1,000,000
        public const int MaxSize = 270;
        public const int MaxPersonNum = 2;
        public const int MazeCrossNum = 6;
        public const int MazeCrossDist = 30;
        public const int MazeBorderPoint1 = 37;
        public const int MazeBorderPoint2 = MazeBorderPoint1 + MazeCrossDist * (1 + MazeCrossNum);
        public const int MaxCarryDistance = 10; //接上人员的最大距离
        public const int MaxCarBallDistance = 30; //拿到小球的最大距离
        public const int MinBallSept = 6; //小球最小可分辨距离
        public const int MinGetBallRound = 300; //小车取球的最小时间间隔
        public const int MinMazeEntryDistance = 42; //迷宫两个入口之间的最小距离
        public const int CollectBoundX = 36;
        public const int CollectBoundY = 67;
        public const int StorageBound = 43;

        public int APauseNum = 0;
        public int BPauseNum = 0;
        public int AFoul1 = 0;
        public int AFoul2 = 0;
        public int BFoul1 = 0;
        public int BFoul2 = 0;
        public int MaxRound;  //最大回合数
        public int GameCount; //上下半场、加时等
        public Camp GameCamp; //当前半场需完成“物资运输”任务的一方
        public int Round { get; set; }//当前回合
        public GameState State { get; set; }
        public Car CarA, CarB;
        public Person[] People;
        public bool RequestNewBall; //当前是否需要新球
        public bool NoBall; //是否有球
        public Camp CollectCamp; //物资收集点处为A车或B车
        public List<Dot> BallsDot; //小球位置
        public Dot BallDot; //场上唯一的小球位置
        public Dot BallAtCollect; //物资收集点处的小球位置
        public int BallCntA, BallCntB; //物资存放点处小球个数
        public PersonGenerator Generator { get; set; }
        public int CurrPersonNumber; //当前人员数量
        //public static bool[,] GameMap = new bool[MaxSize, MaxSize]; //地图信息
        public FileStream FoulTimeFS;

        //增加分数
        public void AddScore(Camp c, Score score)
        {
            int scoreGet = 0;
            switch (score)
            {
                case Score.BallCollectScore: scoreGet = (c != GameCamp) ? 0 : 10; break; //小车成功到达物资收集点
                case Score.BallGetScore: scoreGet = (c != GameCamp) ? 0 : 40; break; //小车成功抓取小球且有效运送
                case Score.BallStoreScore: scoreGet = (c != GameCamp) ? 0 : 45; break; //小车成功将小球带回出发点
                case Score.GoOutOfMaze: scoreGet = (c != GameCamp) ? 0 : 20; break; //小车成功将小球带回出发点
                case Score.PersonGetScore: scoreGet = (c == GameCamp) ? 0 : 15; break; //小车救起被困人员
                case Score.GetBackScore: scoreGet = 20; break; //比赛结束时，小车回到己方出发点

                case Score.Foul1: scoreGet = -10; break;  //失误
                case Score.Foul2: scoreGet = -50; break;  //犯规
                default: break;
            }

            switch (c)
            {
                case Camp.CampA:
                    CarA.Score += scoreGet;
                    if (CarA.Score < 0) CarA.Score = 0;
                    return;
                case Camp.CampB:
                    CarB.Score += scoreGet;
                    if (CarB.Score < 0) CarB.Score = 0;
                    return;
                default: return;
            }
        }

        public static bool InMaze(Dot dot)
        {
            if (InRegion((i, j) => (i >= MazeBorderPoint1 && i <= MazeBorderPoint2 && j >= MazeBorderPoint1 && j <= MazeBorderPoint2), dot))
                return true;
            else return false;
        }
        public static bool InCollect(Dot dot)
        {
            if (InRegion((i, j) => (i <= CollectBoundX && j <= CollectBoundY), dot))
                return true;
            else return false;
        }
        public static bool InStorageA(Dot dot)
        {
            if (InRegion((i, j) => (i <= StorageBound && j >= MaxSize - StorageBound), dot))
                return true;
            else return false;
        }
        public static bool InStorageB(Dot dot)
        {
            if (InRegion((i, j) => (j <= StorageBound && i >= MaxSize - StorageBound), dot))
                return true;
            else return false;
        }

        private static bool InRegion(Func<int, int, bool> inRegion, Dot dot)//点是否有效->3*3方格在区域内的格数大于4
        {
            int count = 0;
            for (int i = ((dot.x - 1 > 0) ? (dot.x - 1) : 0); i <= ((dot.x + 1 < MaxSize) ? (dot.x + 1) : MaxSize - 1); ++i)
                for (int j = ((dot.y - 1 > 0) ? (dot.y - 1) : 0); j <= ((dot.y + 1 < MaxSize) ? (dot.y + 1) : MaxSize - 1); ++j)
                    if (inRegion(i, j))
                        count++;
            if (count >= 4) return true;
            else return false;
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
            GameCamp = Camp.CampA;
            MaxRound = 1200;
            BallsDot = new List<Dot>();
            BallDot = new Dot(0, 0);
            BallAtCollect = new Dot(0, 0);
            RequestNewBall = false;
            NoBall = false;
            BallCntA = BallCntB = 0;
            CollectCamp = Camp.None;
            CarA = new Car(Camp.CampA);
            CarB = new Car(Camp.CampB);
            People = new Person[MaxPersonNum];
            Round = 0;
            State = GameState.Unstart;
            InitialPerson();
            DebugMode = false;
            FoulTimeFS = null;
        }

        public void NextStage()
        {
            ++GameCount;
            if (GameCount % 2 == 1)
                GameCamp = Camp.CampA;
            else
                GameCamp = Camp.CampB;
            if (GameCount >= 3)
                MaxRound = 600;
            else
                MaxRound = 1200;
            Round = 0;
            State = GameState.Unstart;
            CarA.LastInCollectRound = -MinGetBallRound;
            CarB.LastInCollectRound = -MinGetBallRound;
            InitialPerson();
            DebugMode = false;
            if (FoulTimeFS != null)
            {
                byte[] data = Encoding.Default.GetBytes($"nextStage\r\n");
                FoulTimeFS.Write(data, 0, data.Length);
            }
        }

        protected void InitialPerson() //初始化人员
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

        public void Start() //开始比赛
        {
            State = GameState.Normal;
            CarA.LastInCollectRound = -MinGetBallRound;
            CarB.LastInCollectRound = -MinGetBallRound;
            CarA.HaveBall = false;
            CarB.HaveBall = false;
            CarA.Start();
            CarB.Start();
        }
        public void Pause() //暂停比赛
        {
            State = GameState.Pause;
            CarA.LastInCollectRound = -MinGetBallRound;
            CarB.LastInCollectRound = -MinGetBallRound;
            CarA.HaveBall = false;
            CarB.HaveBall = false;
            CarA.Stop();
            CarB.Stop();
        }
        public void End() //结束比赛
        {
            State = GameState.End;
            //结算当前半场分数
            switch (GameCamp)
            {
                case Camp.CampA:
                    if (InStorageA(CarA.Pos))
                        AddScore(Camp.CampA, Score.GetBackScore);
                    if (InStorageB(CarB.Pos))
                        AddScore(Camp.CampB, Score.GetBackScore);
                    break;
                case Camp.CampB:
                    if (InStorageB(CarA.Pos))
                        AddScore(Camp.CampA, Score.GetBackScore);
                    if (InStorageA(CarB.Pos))
                        AddScore(Camp.CampB, Score.GetBackScore);
                    break;
                default: break;
            }
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

        //更新小球相关操作的状态、得分
        public void UpdateBallsState()
        {
            NoBall = true;
            bool noBallInCollect = true;
            int currBallCntA = 0;
            Camp currCollectCamp = Camp.None;
            //BallAtCollect = new Dot(0, 0);
            foreach (Dot ball in BallsDot)
            {
                if (InCollect(ball))
                {
                    BallAtCollect = ball;
                    noBallInCollect = false;
                }
                else if (InStorageA(ball)) currBallCntA++;
                //else if (InStorageB(ball)) currBallCntB++;

                if (!InStorageA(ball))
                {
                    BallDot = ball;
                    NoBall = false;
                }
            }

            //更新CollectCamp：物资收集点处是A车还是B车
            switch (GameCamp)
            {
                case Camp.CampA: if (InCollect(CarA.Pos)) currCollectCamp = Camp.CampA; break;
                case Camp.CampB: if (InCollect(CarB.Pos)) currCollectCamp = Camp.CampB; break;
                default: currCollectCamp = Camp.None;  break;
            }

            //进入物资收集点计分
            if (CollectCamp == Camp.None && currCollectCamp != Camp.None)
            {
                switch (currCollectCamp)
                {
                    case Camp.CampA:
                        if (Round - CarA.LastInCollectRound > MinGetBallRound)
                        { //防止重复计分
                            CarA.LastInCollectRound = Round;
                            AddScore(currCollectCamp, Score.BallCollectScore); //小车成功到达物资收集点
                        }
                        break;
                    case Camp.CampB:
                        if (Round - CarB.LastInCollectRound > MinGetBallRound)
                        {
                            CarB.LastInCollectRound = Round;
                            AddScore(currCollectCamp, Score.BallCollectScore); //小车成功到达物资收集点
                        }
                        break;
                    default: break;
                }
            }

            RequestNewBall = noBallInCollect && !InCollect(CarA.Pos) && !InCollect(CarB.Pos); //物资收集点处没有车和球时才可请求新球
            
            //抓取到小球计分
            if (RequestNewBall) 
            {
                switch (CollectCamp)
                {
                    case Camp.CampA:
                        if (!CarA.HaveBall)
                        {
                            AddScore(Camp.CampA, Score.BallGetScore);
                            CarA.BallGetCnt++;
                            CarA.HaveBall = true;
                        }
                        break;
                    case Camp.CampB:
                        if (!CarB.HaveBall)
                        {
                            AddScore(Camp.CampB, Score.BallGetScore);
                            CarB.BallGetCnt++;
                            CarB.HaveBall = true;
                        }
                        break;
                    default: break; 
                }
                CollectCamp = Camp.None;
            }

            CollectCamp = currCollectCamp;

            //小球运输至存放点计分
            if (currBallCntA == BallCntA + 1)
            {
                if (GameCamp == Camp.CampA && CarA.HaveBall)
                {
                    AddScore(Camp.CampA, Score.BallStoreScore);
                    CarA.BallOwnCnt++;
                    CarA.HaveBall = false;
                }
                if (GameCamp == Camp.CampB && CarB.HaveBall)
                {
                    AddScore(Camp.CampB, Score.BallStoreScore);
                    CarB.BallOwnCnt++;
                    CarB.HaveBall = false;
                }
            }
            BallCntA = currBallCntA;
        }

        //更新小车走出迷宫得分
        public void UpdateMazeState()
        {
            switch (GameCamp)
            {
                case Camp.CampA:
                    bool currCarAInMaze = InMaze(CarA.Pos);
                    if (currCarAInMaze && !CarA.InMaze)
                    {
                        CarA.MazeEntryPos = CarA.Pos;
                    }
                    else if (!currCarAInMaze && CarA.InMaze && !CarA.FinishedMaze)
                    {
                        if (GetDistance(CarA.MazeEntryPos, CarA.Pos) > MinMazeEntryDistance)
                        {
                            AddScore(Camp.CampA, Score.GoOutOfMaze);
                            CarA.FinishedMaze = true;
                        }
                    }

                    CarA.InMaze = InMaze(CarA.Pos);
                    break;
                case Camp.CampB:
                    bool currCarBInMaze = InMaze(CarB.Pos);
                    if (currCarBInMaze && !CarB.InMaze)
                    {
                        CarB.MazeEntryPos = CarB.Pos;
                    }
                    else if (!currCarBInMaze && CarB.InMaze && !CarB.FinishedMaze)
                    {
                        if (GetDistance(CarB.MazeEntryPos, CarB.Pos) > MinMazeEntryDistance)
                        {
                            AddScore(Camp.CampB, Score.GoOutOfMaze);
                            CarB.FinishedMaze = true;
                        }
                    }

                    CarB.InMaze = InMaze(CarB.Pos);
                    break;
                default: break;
            }
        }

        //更新人员上车
        public void UpdatePerson()
        {
            for (int i = 0; i != CurrPersonNumber; ++i)
            {
                Person p = People[i];
                if (CarA.UnderStop == false && GetDistance(p.StartPos, CarA.Pos) < MaxCarryDistance)
                {
                    AddScore(Camp.CampA, Score.PersonGetScore);
                    CarA.PersonCnt++;
                    NewPerson(p.StartPos, i);
                }

                if (CarB.UnderStop == false && GetDistance(p.StartPos, CarB.Pos) < MaxCarryDistance)
                {
                    AddScore(Camp.CampB, Score.PersonGetScore);
                    CarB.PersonCnt++;
                    NewPerson(p.StartPos, i);
                }
            }
        }

        public void Update()//每回合执行
        {
            if (State == GameState.Normal)
            {
                Round++;
                //GetInfoFromCameraAndUpdate();
                CheckPersonNumber();
                UpdateBallsState();
                UpdateMazeState();
                UpdatePerson();
                #region PunishmentPhase
                //if (!CarDotValid(CarA.Pos)) CarA.Stop();
                //if (!CarDotValid(CarB.Pos)) CarB.Stop();
                #endregion

                if ((Round >= MaxRound && DebugMode == false) || (Round >= 1000000 && DebugMode == true)) //结束比赛
                {
                    End();
                }
            }
        } 
        public byte[] PackMessage()
        {
            byte[] message = new byte[32]; //上位机传递的信息
            int messageCnt = 0;
            message[messageCnt++] = (byte)(Round >> 8);
            message[messageCnt++] = (byte)Round;
            message[messageCnt++] = (byte)(((byte)State << 6) | ((byte)(InMaze(CarA.Pos) ? 1 : 0) << 5) | ((byte)(InMaze(CarB.Pos) ? 1 : 0) << 4)
                | (CarA.Pos.x >> 5 & 0x08) | (CarA.Pos.y >> 6 & 0x04) | (CarB.Pos.x >> 7 & 0x02) | (CarB.Pos.y >> 8 & 0x01));
            message[messageCnt++] = (byte)((People[0].StartPos.x >> 1 & 0x80) | (People[0].StartPos.y >> 2 & 0x40)
                | (People[1].StartPos.x >> 3 & 0x20) | (People[1].StartPos.y >> 4 & 0x10)
                | (BallDot.x >> 5 & 0x08) | (BallDot.y >> 6 & 0x04)
                | ((byte)(NoBall ? 0 : 1) << 1) | ((byte)(GameCamp == Camp.CampA ? 1 : 0)));
            message[messageCnt++] = !InMaze(CarA.Pos) ? (byte)CarA.Pos.x : (byte)0;
            message[messageCnt++] = !InMaze(CarA.Pos) ? (byte)CarA.Pos.y : (byte)0;
            message[messageCnt++] = !InMaze(CarB.Pos) ? (byte)CarB.Pos.x : (byte)0;
            message[messageCnt++] = !InMaze(CarB.Pos) ? (byte)CarB.Pos.y : (byte)0;
            for (int i = 0; i < MaxPersonNum; ++i)
            {
                message[messageCnt++] = (byte)People[i].StartPos.x;
                message[messageCnt++] = (byte)People[i].StartPos.y;
            }
            message[messageCnt++] = (byte)BallDot.x;
            message[messageCnt++] = (byte)BallDot.y;
            message[messageCnt++] = (byte)(CarA.Score >> 8);
            message[messageCnt++] = (byte)CarA.Score;
            message[messageCnt++] = (byte)(CarB.Score >> 8);
            message[messageCnt++] = (byte)CarB.Score;
            message[messageCnt++] = (byte)CarA.PersonCnt;
            message[messageCnt++] = (byte)CarB.PersonCnt;
            message[messageCnt++] = (byte)CarA.BallGetCnt;
            message[messageCnt++] = (byte)CarB.BallGetCnt;
            message[messageCnt++] = (byte)CarA.BallOwnCnt;
            message[messageCnt++] = (byte)CarB.BallOwnCnt;
            message[30] = 0x0D;
            message[31] = 0x0A;
            return message;
        }
    }
}