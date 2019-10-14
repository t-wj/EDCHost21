using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EDC21HOST
{
    public struct Dot //点
    {
        public int x;
        public int y;
        public Dot(int _x, int _y) { x = _x; y = _y; }
        public static bool operator == (Dot a, Dot b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator != (Dot a, Dot b)
        {
            return !(a == b);
        }
    }

    public enum Camp
    {
        None = 0, CampA, CampB
    }
    public class Person
    {
        //private static int[] distanceLevel = new int[5] { 0, 68, 158, 270, 381 };
        //private static int[] scoreLevel = new int[4] { 20, 40, 80, 100 };
        public Dot StartPos; //起点
        public int Number { get; set; } //编号
        public Person(Dot startDot, int number)
        {
            StartPos = startDot;
            Number = number;
        }
        public Person() : this(new Dot(0, 0), 0) { }

        public void ResetInfo(Dot startDot, int number)
        {
            StartPos = startDot;
            Number = number;
        }
    }
    public class PersonGenerator //存储预备要用的人员信息
    {
        private Stack<Dot> PersonDotStack;
        public void Generate(int amount) //生成指定数量的人员
        {
            PersonDotStack = new Stack<Dot>();
            int nextX, nextY;
            Dot dots;
            Random NRand = new Random();
            for (int i = 0; i != amount; ++i)
            {
                do
                {
                    nextX = NRand.Next(Game.MazeCrossNum);
                    nextY = NRand.Next(Game.MazeCrossNum);
                }
                while (nextX < nextY); //保证人员出现在左上
                dots = CrossNo2Dot(nextX, nextY);
                PersonDotStack.Push(dots);
            }
        }
        //返回下一个人员的坐标
        public Dot Next(Person [] currentPeople)
        {
            Dot temp;
            bool exist;
            do
            {
                temp = PersonDotStack.Pop();
                exist = false;
                for (int i = 0; i < Game.MaxPersonNum; ++i)
                    if (temp == currentPeople[i].StartPos)
                        exist = true;
            }
            while (exist);
            return temp;
        }
        public Dot CrossNo2Dot(int CrossNoX, int CrossNoY)
        {
            Dot temp;
            temp.x = Game.MazeBorderPoint1 + Game.MazeCrossDist / 2 + Game.MazeCrossDist * CrossNoX;
            temp.y = Game.MazeBorderPoint1 + Game.MazeCrossDist / 2 + Game.MazeCrossDist * CrossNoY;
            return temp;
        }
    }
}