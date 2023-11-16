using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace CatMan
{
    public struct Coordinate
    {
        public int x;
        public int y;

        public List<Coordinate> history = new List<Coordinate>();

        // 구조체 생성자
        public Coordinate(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }
    public class MapGen
    {
        public List<Coordinate> wallCoord = null;
        public List<Coordinate> itemCoord = null;
        public Coordinate wallPoint;
        public Coordinate itemPoint;

        public string st_cat = "-ω-";
        public string st_fish = "<*)~";
        public string st_heart = "♡";
        public string st_ghost = "'_'";

        public void Init()
        {
            Console.SetBufferSize(80, 80);
            Console.SetWindowSize(80, 40);
            wallCoord = new List<Coordinate>();
            itemCoord = new List<Coordinate>();
        }

        public void BaseMap()
        {
            Console.WriteLine(" □□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____□");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____□");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____□");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____□");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____□");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□     |     |     |     |     |     |     |     |     |     |     |     □");
            Console.WriteLine("□_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____|_____□");
            Console.WriteLine(" □□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□");


            //Program.gotoxy(8, 1); // (2, 2)(2, 3) -> (8, 1)
            //Console.WriteLine("□□□");
            //Program.gotoxy(8, 2); // (2, 2)(2, 3)
            //Console.WriteLine("□□□");
            //Program.gotoxy(8, 3); // (2, 2)(2, 3)
            //Console.WriteLine("□□□");
            //Program.gotoxy(40, 25);

            Generate(0, 0, st_cat);
            BaseWall();

            BaseItem();
            Generate(6, 3, st_ghost);

            Program.gotoxy(40, 25);

        }
        public void BaseWall()
        {
            GenerateWall(1, 1);
            GenerateWall(2, 1);
            GenerateWall(2, 4);
            GenerateWall(2, 5);
            GenerateWall(3, 1);
            GenerateWall(4, 3);
            GenerateWall(5, 5);
            GenerateWall(6, 2);
            GenerateWall(6, 5);
            GenerateWall(7, 2);
            GenerateWall(8, 2);
            GenerateWall(8, 4);
            GenerateWall(9, 4);
            GenerateWall(9, 3);
            GenerateWall(9, 2);
            GenerateWall(9, 1);
            GenerateWall(10, 2);
        }
        public void BaseItem()
        {
            // 가로 0~11까지
            for(int i = 0; i < 12; i++)
            {
                // 세로 0~5까지
                for(int j=0; j < 6; j++)
                {
                    // 플레이어 시작 지점에는 아이템 생성 안 되도록
                    if (i == 0 && j == 0) continue;

                    bool isWall = false;
                    // (i, j)가 wallCoord에 없으면 (i, j)를 ItemCoord에 추가
                    for (int k = 0; k < wallCoord.Count(); k++)
                    {
                        if (wallCoord[k].x == i && wallCoord[k].y == j) isWall = true;
                    }

                    if (isWall) continue;

                    // --> 근데 이렇게 적을 수 있나??
                    //List에 좌표값이 순서대로 들어가 있는 게 아닌데?
                    itemPoint = new Coordinate(i, j);
                    itemCoord.Add(itemPoint);

                    Generate(i, j, st_heart);
                }
            }
        }

        public void Generate(int x, int y, string c)
        {
            x = x * 6 + 3;
            y = y * 3 + 2;// 2번째 줄에 그려져야 하니까 GenerateCat
            Program.gotoxy(x, y);
            // switch(c) { } 해서 case st_ghost: 빨간색으로 출력
            Console.Write(c);
        }
        
        public void GenerateWall(int i, int j)
        {
            // wall의 위치를 저장하기 위한 Coordinate 구조체가 들어가는 리스트
            // https://nonstop-antoine.-tistory.com/47
            // https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=burin&logNo=40192870928

            wallPoint = new Coordinate(i, j);
            wallCoord.Add(wallPoint);

            // 실제로 그리는 파트
            // 0일때 2, 1일때 8
            i = i * 6 + 2;
            //j는 0이 들어오면 123 / 1이 들어오면 456
            j = j * 3 + 1;

            // 3줄 채우는 거니까 w가 j부터 j+3까지
            // (0, 0)이 들어오면 (2, 1)/(2, 2)/(2, 3)이 색칠되도록 i, j
            for (int w = 0; w<3; w++)
            {
                Program.gotoxy(i, j+w);
                Console.WriteLine("/////");
            }
           
        }

        public void EmptyWall(int i, int j)
        {
            // 0일때 2, 1일때 8
            i = i * 6 + 2;
            //j는 0이 들어오면 123 / 1이 들어오면 456
            j = j * 3 + 1;

            // 3줄 채우는 거니까 w가 j부터 j+3까지
            // (0, 0)이 들어오면 (2, 1)/(2, 2)/(2, 3)이 색칠되도록 i, j
            for (int w = 0; w < 2; w++)
            {
                Program.gotoxy(i, j + w);
                Console.WriteLine("     ");
            }
            Program.gotoxy(i, j + 2);
            Console.Write("_____");

        }


    }
}
