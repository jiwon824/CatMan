using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatMan
{
    public class MainGame
    {
        Cat cat = new Cat();
        MapGen mapGen = new MapGen();
        Ghost ghost = new Ghost();

        const int LEFTKEY = 75;
        const int RIGHTKEY = 77;
        const int UPKEY = 72;
        const int DOWNKEY = 80;

        int[] dx = new int[4] { 1, 0, -1, 0 }; // 오른쪽, 아래, 왼쪽, 위
        int[] dy = new int[4] { 0, -1, 0, 1 }; // 오른쪽, 아래, 왼쪽, 위

        int isVic = 2; // 0 게임오버, 1 게임 클리어, 2 게임 중
        public void Reset()
        {
            Console.Clear();
            cat.Init();
            ghost.Init();
            mapGen.Init();
            mapGen.BaseMap();

        }

        public void KeyInput()
        {
            int pressKey; // 정수형 변수 선언 키값 받을 거임
            // 게임 중이면서 키가 눌렸을 때
            if (isVic == 2 && Console.KeyAvailable) // 키가 눌렸을 때 true
            {
                pressKey = Program._getch(); //아스키값 왼쪽 오른쪽
                if (pressKey == 224)
                {
                    pressKey = Program._getch();
                }

                switch (pressKey)
                {
                    case UPKEY:
                        Move(cat.currX, cat.currY - 1);
                        break;
                    case LEFTKEY:
                        Move(cat.currX - 1, cat.currY);
                        break;
                    case RIGHTKEY:
                        Move(cat.currX + 1, cat.currY);
                        break;
                    case DOWNKEY:
                        Move(cat.currX, cat.currY + 1);
                        break;

                    // r이나 R 누르면 재시작되도록
                    case 114:
                    case 82:
                        Console.Clear();
                        Reset();
                        break;

                }


            }
        }

        public void Move(int nx, int ny)
        {
            BfsGhost();
            if (ghost.currX == cat.currX && ghost.currY == cat.currY)
            {
                isVic = 0; // isVic==0 이 되면 Progress()에서 GameOver()호출
            }
            //nx나 ny가 범위를 벗어나면 움직이지 않고 return
            if (nx < 0 || nx > 11 || ny < 0 || ny > 5) return;
            //nx, ny에 벽이 있으면 return
            for (int i = 0; i < mapGen.wallCoord.Count(); i++)
            {
                if (mapGen.wallCoord[i].x == nx && mapGen.wallCoord[i].y == ny) return;
            }
            // nx, ny에 아이템이 있으면 아이템 리스트에서 제거
            for (int i = 0; i < mapGen.itemCoord.Count(); i++)
            {
                if (mapGen.itemCoord[i].x == nx && mapGen.itemCoord[i].y == ny)
                {
                    //Coordinate itemToRemove = new Coordinate(nx, ny);
                    mapGen.itemCoord.RemoveAt(i);
                    break;
                }
            }

            //위 if문에 안 걸렸으면 고양이의 위치를 지우고
            mapGen.EmptyWall(cat.currX, cat.currY);
            //cat의 좌표를 (nx, ny)로 변경
            cat.currX = nx;
            cat.currY = ny;
            // 고양이 생성
            mapGen.Generate(cat.currX, cat.currY, mapGen.st_cat);

            // 이동하면 아이템리스트의 좌표에 아이템 Generate되도록
            GenerateItem();

        }

        public void BfsGhost()
        {
            Queue<Coordinate> ghost_queue = new Queue<Coordinate>();
            ghost_queue.Enqueue(new Coordinate(ghost.currX, ghost.currY));
            Coordinate moveCoord;

            // 큐가 빌 때까지 반복
            while (ghost_queue.Count() != 0)
            {
                Coordinate currCoord = ghost_queue.Dequeue();
                if (currCoord.x == cat.currX && currCoord.y == cat.currY)
                {
                    // 히스토리의 첫 번째 노드가 다음에 이동 할 노드?
                    if (currCoord.history.Count() > 1)
                    {
                        moveCoord = currCoord.history[1];
                        // Console.WriteLine(moveCoord.x + " " + moveCoord.y);

                        // moveCoord로 이동 (현재좌표 귀신 없애기 + moveCoord에 Generate 귀신)
                        mapGen.EmptyWall(ghost.currX, ghost.currY);
                        ghost.currX = moveCoord.x;
                        ghost.currY = moveCoord.y;
                        mapGen.Generate(ghost.currX, ghost.currY, mapGen.st_ghost);
                    }

                    break;
                }
                int nx, ny; // 다음에 이동할 x, y 좌표

                for (int i = 0; i < 4; i++)
                {
                    nx = currCoord.x + dx[i];
                    ny = currCoord.y + dy[i];
                    // next좌표가 벽이라면 넘어가기
                    if (nx < 0 || nx > 11 || ny < 0 || ny > 5) continue;
                    bool isWall = false;
                    //nx, ny에 맵 내부 벽 있으면 return
                    for (int j = 0; j < mapGen.wallCoord.Count(); j++)
                    {
                        if (mapGen.wallCoord[j].x == nx && mapGen.wallCoord[j].y == ny)
                        {
                            isWall = true;
                            break;
                        }
                    }
                    if (isWall) continue;
                    // continue가 안 먹혔으면 queue에 넣고 history 리스트에 넣기
                    // currCoord의 히스토리를 전부 n_coord의 히스토리에 넣고, currCoord를 n_coord의 히스토리에 넣음
                    Coordinate n_coord = new Coordinate(nx, ny);
                    for (int j = 0; j < currCoord.history.Count(); j++)
                    {
                        n_coord.history.Add(currCoord.history[j]);
                    }
                    n_coord.history.Add(currCoord);

                    ghost_queue.Enqueue(n_coord);
                }

            }

        }

        public void GenerateItem()
        {
            for (int i = 0; i < mapGen.itemCoord.Count(); i++)
            {
                // 플레이어(cat) 위치나 유령(ghost) 위치에는 아이템이 생성되지 않도록 continue
                if ((mapGen.itemCoord[i].x == cat.currX && mapGen.itemCoord[i].y == cat.currY) ||
                    (mapGen.itemCoord[i].x == ghost.currX && mapGen.itemCoord[i].y == ghost.currY)) continue;
                mapGen.Generate(mapGen.itemCoord[i].x, mapGen.itemCoord[i].y, mapGen.st_heart);
            }
        }

        public void GameOver()
        {
            isVic = 0;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(3, 3);
            Console.WriteLine(",------. ,---.  ,--.,--.                   ");
            Console.WriteLine("|  .--- '/  O / |  ||  |                   ");
            Console.WriteLine("|  `--,|  .-.  ||  ||  |                 ");
            Console.WriteLine("|  |`  |  | |  ||  || '--.                 ");
            Console.WriteLine("`--'   `--' `--'`--'`-----'               ");



            Console.SetCursorPosition(10, 10);
            Console.Write(",------. ,---.  ,--.,--.                   ");
            Console.SetCursorPosition(10, 11);
            Console.Write("|  .--- '/  O / |  ||  |                   ");
            Console.SetCursorPosition(10, 12);
            Console.Write("|  `--,|  .-.  ||  ||  |                 ");
            Console.SetCursorPosition(10, 13);
            Console.Write("|  |`  |  | |  ||  || '--.                 ");
            Console.SetCursorPosition(10, 14);
            Console.Write("`--'   `--' `--'`--'`-----'               ");
            Console.ResetColor();
            Thread.Sleep(1500);


            Program.gotoxy(40, 25);
        }
        public void Clear()
        {
            isVic = 1;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(3, 3);
            Console.WriteLine("  __         _           .__ ");
            Console.WriteLine(" /      ||  |     |   __|   /   \\     |     \\     ");
            Console.WriteLine("|  ,----'|  |     |  |     /  ^  \\    |  |_)  | ");
            Console.WriteLine("|  ----.|  ----.|  |__ /    \\  |  |\\  \\----.");
            Console.WriteLine(" \\__||____//     \\ | | `.|");




            Console.SetCursorPosition(10, 10);
            Console.WriteLine("  __         __     _      .__ ");
            Console.SetCursorPosition(10, 11);
            Console.WriteLine(" /        |     |   _|   /   \\     |     \\     ");
            Console.SetCursorPosition(10, 12);
            Console.WriteLine("|  ,----'|  |     |  |     /  ^  \\    |  |)  | ");
            Console.SetCursorPosition(10, 13);
            Console.Write("|  |  |  | |  ||  || '--.                 ");
            Console.SetCursorPosition(10, 14);
            Console.WriteLine("|  ----.|  `----.|  | /    \\  |  |\\  \\----.");
            Console.ResetColor();
            Thread.Sleep(1500);

            Program.gotoxy(40, 25);


        }

        public int Progress()
        {

            mapGen.Init();
            mapGen.BaseMap();

            while (true)
            {
                KeyInput();
                if (mapGen.itemCoord.Count() == 0)
                {
                    Clear();
                    break;
                }
                if (isVic == 0)
                {
                    GameOver();
                    break;
                }

            }

            return isVic;

        }
    }
}
