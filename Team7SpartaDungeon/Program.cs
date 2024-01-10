using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace Team7SpartaDungeon
{
    internal class Program
    {
//----- 플레이어 직업, 몬스터, 아이템 참조 -----------------------------------------------------------------------------------------
        struct Warrior
        {
            public string Name { get; set; }
            public string Class { get; }
            public int Level { get; set; }
            public float Atk { get; set; }
            public int Def { get; set; }
            public int MaxHp { get; set; }
            public int Hp { get; set; }
            public int Mp { get; set; }
            public int Gold { get; set; }
            public int Exp { get; set; }
            public Warrior()
            {
                Name = "name";
                Class = "전사";
                Level = 1;
                Atk = 10;
                Def = 5;
                MaxHp = 100;
                Hp = 100;
                Mp = 50;
                Gold = 1500;
                Exp = 0;
            }
        }

        struct Monster
        {
            public string Name { get; }
            public int Level { set; get; }
            public int Atk { get; set; }
            public int Def { get; set; }
            public int Hp { get; set; }
            public Monster(string name,int level,int atk,int def, int hp)
            {
                Name = name;
                Level = level;
                Atk = atk;
                Def = def;
                Hp = hp;
            }
        }
//----- 메인 -----------------------------------------------------------------------------------------------------------------------
        static void Main(string[] args)
        {
            SpartaDungeon sd = new SpartaDungeon();
            sd.PlayGame();
        }
//----------------------------------------------------------------------------------------------------------------------------------
        public class SpartaDungeon
        {
            // 플레이어, 몬스터, 몬스터 리스트 dungeon 생성
            Warrior player = new Warrior();
            Monster minion = new Monster("미니언", 2, 10, 0, 15);
            Monster siegeMinion = new Monster("대포미니언", 5, 20, 0, 25);
            Monster voidBug = new Monster("공허충", 3, 15, 0, 10);
            List<Monster> dungeon = new List<Monster>();
            public void PlayGame() // 게임 시작 메서드
            {
                dungeon.Add(minion);                 // 던전에서 출현할 몬스터 추가
                dungeon.Add(siegeMinion);
                dungeon.Add(voidBug);
                Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.\n원하시는 이름을 설정해주세요.");
                player.Name = Console.ReadLine();    // 플레이어 이름 입력
                while (true)
                {
                    int choice = Choice0();
                    if (choice == 1)
                    {
                        Status();
                    }
                    if (choice == 2)
                    {
                        BattleStart();
                    }
                }
            }
            public int Choice0() // 0. 마을 선택지
            {
                Console.Clear();
                Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.\r\n이제 전투를 시작할 수 있습니다.");
                Console.WriteLine("\n1. 상태 보기\n2. 전투 시작");
                return ChoiceInput(1, 2, "잘못된 입력입니다.\n1. 상태보기 2.전투 시작");
            }
            public void Status() // 1. 상태 보기
            {
                Console.Clear();
                Console.WriteLine($"캐릭터의 정보가 표시됩니다.\n\n" +
                                  $" 이름   : {player.Name}\n"+
                                  $" 레벨   : {player.Level}\n 직업   : {player.Class}\n 공격력 : {player.Atk}\n 방어력 : {player.Def}\n" +
                                  $" 체 력  : {player.Hp}/{player.MaxHp}\n Gold   : {player.Gold} G\n 경험치 : {player.Exp} / {player.Level}\n\n" +
                                  $"Enter. 나가기");
                Console.ReadLine();
            }
            public void BattleStart() // 2. 전투 시작
            {
                List<Monster> monsters = new List<Monster>(); // 몬스터 리스트 monsters 생성
                Random r = new Random();
                int beforeHp = player.Hp;
                for (int i = 0; i < r.Next(1, 5); i++)
                {
                    monsters.Add(dungeon[r.Next(0, 3)]);      // 몬스터 리스트 monsters 에 게임시작시 만들어둔 몬스터 리스트 dungeon 에 저장된 몬스터 랜덤 추가
                }
                int[] monsterHp = new int[monsters.Count];
                for (int i = 0; i < monsters.Count; i++)
                {
                    monsterHp[i] = monsters[i].Hp;            // 몬스터의 마릿수와 동일한크기의 int 배열 생성 후, 몬스터의 체력 정보 저장
                }

                while (0 < player.Hp) // 전투 시작 플레이어 턴
                {
                    int choice = ChoiceAction();
                    if (choice == 1)
                    {
                        Attack();
                    }
                    if (CheckMonsters()) break;
                }
                if (0 < player.Hp) Victory();
                else Lose();

                void BattleField() // 현재 전투 필드 출력 메서드
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("   Battle!!\n\n");
                    Console.ResetColor();
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        if (!(monsterHp[i] <= 0))
                            Console.WriteLine($"   Lv.{monsters[i].Level} {monsters[i].Name} HP {monsterHp[i]}/{monsters[i].Hp}");
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($"   Lv.{monsters[i].Level} {monsters[i].Name} Dead");
                            Console.ResetColor();
                        }
                    }
                    Console.WriteLine($"\n\n   [내정보]\n\n   Lv.{player.Level} {player.Name} ({player.Class})\n   Hp {player.Hp}/{player.MaxHp}");
                }

                int ChoiceAction() // 플레이어 턴 선택지
                {
                    BattleField();
                    Console.WriteLine("\n\n1. 공격");
                    int choice = ChoiceInput(1, 1, "잘못된 입력입니다.\n1. 공격");
                    return choice;
                }

                void Attack() // 플레이어 공격
                {
                    BattleField();
                    Console.SetCursorPosition(0, 3);
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        if (0 < monsterHp[i])
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"{i + 1}");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($"{i + 1}");
                            Console.ResetColor();
                        }
                    }
                    Console.SetCursorPosition(0, 11 + monsters.Count);
                    Console.WriteLine("0. 취소\n\n대상을 선택해주세요.");
                    int atk = ChoiceInput(0, monsters.Count, "잘못된 입력입니다.\n0. 취소");
                    if (!(atk == 0))
                    {
                        if (0 < monsterHp[atk - 1])
                        {
                            int bh = monsterHp[atk - 1];
                            BattleField();
                            Console.WriteLine($"\n\n{player.Name} 의 공격!\n");
                            monsterHp[atk - 1] -= r.Next((int)Math.Ceiling(player.Atk * 0.9f), (int)Math.Ceiling(player.Atk * 1.1f)+1); // 플레이어 공격 데미지
                            Console.WriteLine($"Lv.{monsters[atk - 1].Level} {monsters[atk - 1].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[atk - 1]}]");
                            if (monsterHp[atk - 1] <= 0)
                                Console.WriteLine($"\nLv.{monsters[atk - 1].Level} {monsters[atk - 1].Name}\nHP {bh} -> Dead");
                            Console.WriteLine("\n\nEnter. 다음");
                            Console.ReadLine();
                            if (!(CheckMonsters())) EnemyPhase(); // 공격 종료 후, 몬스터가 남아있으면 몬스터 턴
                        }
                        else
                        {
                            Console.WriteLine("잘못된 입력입니다.\n\nEnter. 다음");
                            Console.ReadLine();
                        }
                    }
                }

                void EnemyPhase() // 몬스터 턴, 몬스터 행동
                {
                    for(int i = 0; i < monsters.Count; i++)
                    {
                        BattleField();
                        int befHp = player.Hp;
                        int damage = monsters[i].Atk - player.Def;    // 몬스터 데미지
                        if (damage < 0) damage = 0;
                        if (0 < monsterHp[i])
                        {
                            player.Hp -= damage;
                            Console.SetCursorPosition(0, 3+i);
                            Console.WriteLine($"▶");
                            Console.SetCursorPosition(0, 11 + monsters.Count);
                            Console.WriteLine("\n");
                            Console.WriteLine($"Lv.{monsters[i].Level} {monsters[i].Name} 의 공격!\n{player.Name} 을(를) 맞췄습니다. [데미지 : {befHp - player.Hp}]\n");
                            if(player.Hp <= 0)
                            {
                                Console.WriteLine($"Lv.{player.Level} {player.Name}\nHP {befHp} -> Dead\n\nEnter. 다음");
                                Console.ReadLine();
                                break;
                            }
                            else
                            {
                                Console.Write($"Lv.{player.Level} {player.Name}\nHP {befHp} -> {player.Hp}\n\nEnter. 다음");
                                Console.ReadLine();
                            }
                        }
                    }
                }

                bool CheckMonsters() // 몬스터가 모두 죽었는지 확인하는 메서드. 모두 죽었으면 true, 아니면 false 반환
                {
                    int dead = 0;
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        if (monsterHp[i] <= 0)
                            dead++;
                    }
                    if (dead == monsters.Count)
                    {
                        return true;
                    }
                    return false;
                }

                void Victory() // 전투 승리 결과출력
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("   Battle!! - Result\n\n");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("   Victory\n\n");
                    Console.ResetColor();
                    Console.WriteLine($"던전에서 몬스터 {monsters.Count}마리를 잡았습니다.\n\n");
                    Console.WriteLine($"Lv. {player.Level} {player.Name}\nHP {beforeHp} -> {player.Hp}\n\n");
                    Console.WriteLine("Enter. 다음");
                    Console.ReadLine();
                }

                void Lose() // 전투 패배 결과출력
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("   Battle!! - Result\n\n");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   You Lose\n\n");
                    Console.ResetColor();
                    Console.WriteLine($"Lv. {player.Level} {player.Name}\nHP {beforeHp} -> 0\n\n");
                    Console.WriteLine("Enter. 다음");
                    Console.ReadLine();
                    player.Hp = 0;
                }
            }
            public int ChoiceInput(int limit, int limit2, string action) // 선택지 입력 메서드
            {
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice < limit || choice > limit2)
                {
                    Console.WriteLine(action);
                    input = Console.ReadLine();
                }
                return choice;
            }
        }
    }
}
