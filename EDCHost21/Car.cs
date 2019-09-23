using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDC21HOST
{
    public class Car //选手的车
    {
        public const int PunishScore = 100; //惩罚分数
        public const double BonusRate = 0.5; //增加总分比例
        public Dot Pos;
        public Camp Who { get; set; } //A or B
        public int Score { get; set; } //当前得分
        public int PersonCnt; //人员数
        public int BallGetCnt; //抓取小球数
        public int BallAtOwnCnt; //己方小球数
        public int BallAtOppoCnt; //对方小球数
        public bool HaveBonus; //是否增加总分
        public bool UnderStop; //是否正在强制停车
        public int FoulCnt; //惩罚次数
        public void Stop() { UnderStop = true; } //车辆强制停止
        public void Start() { UnderStop = false; } //从强制停止中恢复
        public void Out() //出界处理
        {
            Stop();
            Foul();
        }
        public void Foul() //犯规
        {
            FoulCnt++;
        }
        public Car(Camp c)
        {
            Who = c;
            Pos = new Dot();
            Score = 0;
            PersonCnt = 0;
            BallGetCnt = 0;
            BallAtOwnCnt = 0;
            BallAtOppoCnt = 0;
            UnderStop = false;
            HaveBonus = false;
            FoulCnt = 0;
        }
    }
}
