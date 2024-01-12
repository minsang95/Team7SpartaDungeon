using System.Security.Cryptography;

namespace Team7SpartaDungeon
{
    internal class Program
    {
        //-------- 플레이어 직업, 몬스터, 아이템 참조 ---------------------------------------------------------------------------------
        public class Player
        {
            public string Name { get; set; }
            public string Class { get; set; }
            public int Level { get; set; }
            public float Atk { get; set; }
            public int Def { get; set; }
            public int SkillAtk { get; set; }
            public int MaxHp { get; set; }
            public int Hp { get; set; }
            public int MaxMp { get; set; }
            public int Mp { get; set; }
            public int Gold { get; set; }
            public int Exp { get; set; }
            public int MaxExp { get; set; }

            public List<bool> AvailableSkill { get; set; }
            public List<string> Skill { get; set; }
            public int BurningDmg { get; set; }
        }

        public class Warrior : Player
        {
            public Warrior()
            {
                Name = "이름";
                Class = "전사";
                Level = 1;
                Atk = 10;
                Def = 5;
                SkillAtk = 0;
                MaxHp = 100;
                Hp = 100;
                MaxMp = 50;
                Mp = 50;
                Gold = 1500;
                Exp = 0;
                MaxExp = 30;
                AvailableSkill = new List<bool>();
                Skill = new List<string>();
            }
        }

        public class Wizard : Player
        {
            public Wizard()
            {
                Name = "이름";
                Class = "마법사";
                Level = 1;
                Atk = 5;
                Def = 0;
                SkillAtk = 10;
                MaxHp = 80;
                Hp = 80;
                MaxMp = 100;
                Mp = 100;
                Gold = 1500;
                Exp = 0;
                MaxExp = 30;
                AvailableSkill = new List<bool>();
                Skill = new List<string>();
                BurningDmg = 5;
            }
        }

        struct Monster
        {
            public string Name { get; }
            public int Level { set; get; }
            public int Atk { get; set; }
            public int Def { get; set; }
            public int Hp { get; set; }

            public int DropExp { get; }
            public int Burning { get; set; }
            public int BurningDmg { get; set; }
            public int Gold { get; set; }
            public Monster(string name, int level, int atk, int def, int hp, int dropExp, int gold)

            {
                Name = name;
                Level = level;
                Atk = atk;
                Def = def;
                Hp = hp;
                DropExp = dropExp;
                Gold = gold;
            }
        }

        public class Item
        {
            public string Name { get; }
            public int Type { get; }
            public int Atk { get; set; }
            public int Def { get; set; }
            public int Gold { get; set; }

            public Item(string name, int type, int atk, int def, int gold)
            {
                Name = name;
                Type = type;
                Atk = atk;
                Def = def;
                Gold = gold;
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
            static int dungeonFloor = 0;       // 던전 층수



            // 플레이어, 몬스터, 몬스터 리스트 dungeon 생성
            Player player = new Player();
            Monster minion = new Monster("미니언", 2, 10, 0, 15, 20, 500);
            Monster siegeMinion = new Monster("대포미니언", 5, 20, 0, 25, 80, 1000);
            Monster voidBug = new Monster("공허충", 3, 15, 0, 10, 50, 800);

            List<Monster> dungeon = new List<Monster>();
            List<Item> items = new List<Item>(); // 아이템 리스트 초기화
            public int ChoiceInput(int fst, int last) // 선택지 입력 메서드
            {
                Console.WriteLine();
                int cp = Console.CursorTop;
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice < fst || choice > last)
                {
                    Console.SetCursorPosition(0, cp);
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.Write("                    \r");
                    input = Console.ReadLine();
                }
                return choice;
            }
            public void ItemTable() // 드랍 테이블 보관용 아이템 메서드
            {
                items.Add(new Item("낡은 검", 0, 3, 0, 500));   // 무기, 공격력 3, 방어력 0, 가격 500
                items.Add(new Item("보통 검", 0, 7, 0, 1000));  // 무기, 공격력 7, 방어력 0, 가격 1000
                items.Add(new Item("낡은 갑옷", 1, 0, 7, 800));  // 방어구, 공격력 0, 방어력 7, 가격 800
                items.Add(new Item("보통 갑옷", 1, 0, 15, 1300)); // 방어구, 공격력 0, 방어력 15, 가격 1300
                items.Add(new Item("체력 포션", 2, 0, 0, 200));  // 소모품, 공격력 0, 방어력 0, 가격 200
            }

            public void PlayGame() // 게임 시작 메서드
            {


                dungeon.Add(minion);                 // 던전에서 출현할 몬스터 추가
                dungeon.Add(siegeMinion);
                dungeon.Add(voidBug);

                ItemTable(); // 드랍 테이블 초기화
                Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.\n원하시는 직업을 선택해주세요.\n1. 전사\n2. 마법사");
                switch (ChoiceInput(1, 2)) // 직업 선택
                {
                    case 1:
                        player = new Warrior();
                        player.Skill.Add("알파 스트라이크 - MP 10\n   공격력 * 2 로 하나의 적을 공격합니다."); // 전사 1번 스킬 추가
                        player.AvailableSkill.Add(true);
                        player.Skill.Add("더블 스트라이크 - MP 15\n   공격력 * 1.5 로 2명의 적을 랜덤으로 공격합니다."); // 전사 2번 스킬 추가
                        player.AvailableSkill.Add(true);
                        break;
                    case 2:
                        player = new Wizard();
                        player.Skill.Add("파이어 브레스 - MP 20\n   마법력 * 0.5 로 모든 적을 공격하고, 화상 상태로 만듭니다.(화상 데미지 5 x 4)"); // 마법사 1번 스킬 추가
                        player.AvailableSkill.Add(true);
                        player.Skill.Add("아이스 스피어 - MP 10\n   마법력 + 10 으로 가장 앞에있는 적을 공격한다. 만약 적이 사망할 경우, 초과한 데미지만큼 다음 적이 데미지를 받는다."); // 마법사 2번스킬 추가
                        player.AvailableSkill.Add(true);
                        break;
                }
                Console.WriteLine("원하시는 이름을 설정해주세요.");
                player.Name = Console.ReadLine();    // 플레이어 이름 입력
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.\n이제 전투를 시작할 수 있습니다.\n\n1. 상태 보기\n2. 전투 시작( 현재 진행 : " + (dungeonFloor + 1) + " 층 )");
                    switch (ChoiceInput(1, 2)) // 최초 선택지
                    {
                        case 1:
                            Status();
                            break;
                        case 2:
                            BattleStart();
                            break;
                    }
                }
            }
            //---------------------레벨업
            public void LevelUp()
            {
                if (player.Exp < player.MaxExp) return;
                while (player.Exp >= player.MaxExp)
                {
                    Console.Clear();
                    Console.WriteLine("레벨이 올랐습니다!" + player.Level + "->" + (player.Level + 1));
                    Console.WriteLine("공격력이 5 올랐습니다.");
                    Console.WriteLine("방어력이 1 올랐습니다.");
                    Console.WriteLine("체력이 10 올랐습니다.");
                    player.Exp -= player.MaxExp;

                    for (int i = 1; i <= player.Level; i++)
                    {
                        player.MaxExp += (i * 30);
                    }


                    player.Level += 1;
                    player.Atk += 5;
                    player.Def += 1;
                    if (player.Class == "전사")
                    {
                        player.MaxHp += 10;
                        player.MaxMp += 5;
                    }
                    else
                    {
                        player.MaxHp += 5;
                        player.MaxMp += 10;
                    }
                    player.Hp = player.MaxHp; //레벨업 시 회복
                    player.Mp = player.MaxMp;

                    Console.ReadKey();

                }
            }

            public void Status() // 1. 상태 보기
            {
                Console.Clear();
                Console.WriteLine($"캐릭터의 정보가 표시됩니다.\n\n" +
                                  $" 이름   : {player.Name}\n" +
                                  $" 레벨   : {player.Level}\n 직업   : {player.Class}\n 공격력 : {player.Atk}\n 방어력 : {player.Def}\n 마법력 : {player.SkillAtk}\n" +
                                  $" 체 력  : {player.Hp}/{player.MaxHp}\n 마 나  : {player.Mp}/{player.MaxMp}\n Gold   : {player.Gold} G\n 경험치 : {player.Exp} / {player.MaxExp}\n\n" +
                                  $"Enter. 나가기");
                Console.ReadLine();
            }
            public void BattleStart() // 2. 전투 시작
            {
                int diff = dungeonFloor * 2;  // 난이도 보정


                List<Monster> monsters = new List<Monster>(); // 몬스터 리스트 monsters 생성
                Random r = new Random();
                int beforeHp = player.Hp;
                int beforeMp = player.Mp;
                int beforeExp = player.Exp;
                for (int i = 0; i < dungeon.Count; i++)
                {



                    Monster newHp = dungeon[i];
                    newHp.Hp += diff;
                    dungeon[i] = newHp;

                    Monster newAtk = dungeon[i];
                    newAtk.Atk += diff;
                    dungeon[i] = newAtk;

                    Monster newGold = dungeon[i];
                    newAtk.Gold += 40 * diff;
                    dungeon[i] = newGold;

                }

                for (int i = 0; i < r.Next((1 + diff), (5 + diff)); i++)   //난이도에 따른 몹 마릿수 증가
                {
                    monsters.Add(dungeon[r.Next(0, dungeon.Count)]);      // 몬스터 리스트 monsters 에 게임시작시 만들어둔 몬스터 리스트 dungeon 에 저장된 몬스터 랜덤 추가
                }
                int[] monsterHp = new int[monsters.Count];
                for (int i = 0; i < monsters.Count; i++)
                {

                    monsterHp[i] = monsters[i].Hp;            // 몬스터의 마릿수와 동일한크기의 int 배열 생성 후, 몬스터의 체력 정보 저장

                }
                int[] monsterBurn = new int[monsters.Count];
                while (0 < player.Hp) // 전투 시작 플레이어 턴
                {
                    BattleField();

                    Console.WriteLine("\n\n1. 공격\n2. 스킬");
                    switch (ChoiceInput(1, 2))
                    {
                        case 1:
                            Attack();
                            break;
                        case 2:
                            Skill();
                            break;
                    }
                    if (CheckMonsters() == 0) break;
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
                    Console.WriteLine($"\n\n   [내정보]\n\n   Lv.{player.Level} {player.Name} ({player.Class})\n   HP {player.Hp}/{player.MaxHp}\n   MP {player.Mp}/{player.MaxMp}");
                }

                void MonsterNumber() // 몬스터 번호 출력 메서드
                {
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
                }

                void Attack() // 플레이어 공격
                {

                    BattleField();
                    MonsterNumber();
                    Console.WriteLine("0. 취소\n\n대상을 선택해주세요.");
                    int atk = ChoiceInput(0, monsters.Count);
                    if (!(atk == 0))
                    {
                        if (0 < monsterHp[atk - 1])
                        {
                            int Critical = r.Next(1, 101);
                            int bh = monsterHp[atk - 1];
                            if (Critical <= 15)
                            {
                                monsterHp[atk - 1] -= r.Next((int)Math.Ceiling(player.Atk * 1.44f), (int)Math.Ceiling(player.Atk * 1.76f) + 1); // 플레이어 공격 데미지
                                BattleField();
                                Console.WriteLine($"\n\n{player.Name} 의 공격!\n");
                                Console.WriteLine($"Lv.{monsters[atk - 1].Level} {monsters[atk - 1].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[atk - 1]}] - 치명타!!");
                            }
                            else if (Critical >= 90)
                            {
                                BattleField();
                                Console.WriteLine($"\n\n{player.Name} 의 공격!\n");
                                Console.WriteLine($"Lv.{monsters[atk - 1].Level} {monsters[atk - 1].Name} 을(를) 공격했지만 아무일도 일어나지 않았습니다.");
                            }
                            else
                            {
                                monsterHp[atk - 1] -= r.Next((int)Math.Ceiling(player.Atk * 0.9f), (int)Math.Ceiling(player.Atk * 1.1f) + 1); // 플레이어 공격 데미지
                                BattleField();
                                Console.WriteLine($"\n\n{player.Name} 의 공격!\n");
                                Console.WriteLine($"Lv.{monsters[atk - 1].Level} {monsters[atk - 1].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[atk - 1]}]");
                            }
                            if (monsterHp[atk - 1] <= 0)
                            {
                                Console.WriteLine($"\nLv.{monsters[atk - 1].Level} {monsters[atk - 1].Name}\nHP {bh} -> Dead");
                            }
                            Console.WriteLine("\n\nEnter. 다음");
                            Console.ReadLine();
                            if ((CheckMonsters() != 0)) EnemyFrontPhase(); // 공격 종료 후, 몬스터가 남아있으면 몬스터 턴
                        }
                        else
                        {
                            Console.WriteLine("Dead 상태의 몬스터는 공격할 수 없습니다.\n\nEnter. 다음");
                            Console.ReadLine();
                        }
                    }
                }

                void Skill()
                {
                    BattleField();
                    Console.WriteLine("\n");
                    for (int i = 0; i < player.Skill.Count; i++)
                    {
                        if (player.AvailableSkill[i])
                        {
                            Console.WriteLine($"{i + 1}. {player.Skill[i]}\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($"{i + 1}. {player.Skill[i]}\n");
                            Console.ResetColor();
                        }
                    }
                    Console.WriteLine("사용할 스킬을 선택해주세요.\n\n0. 취소");
                    int use = ChoiceInput(0, player.Skill.Count);
                    //------------------ 전사 스킬 ------------------------------------------------------------------------------------------------------------------------------
                    if (player is Warrior && use == 1 && player.AvailableSkill[use - 1]) // 전사 1번 스킬 // 알파 스트라이크 - MP 10, 공격력 * 2 로 하나의 적을 공격합니다.
                    {
                        if (10 <= player.Mp)
                        {
                            BattleField();
                            MonsterNumber();
                            Console.WriteLine("0. 취소\n\n대상을 선택해주세요.");
                            int atk = ChoiceInput(0, monsters.Count);
                            if (!(atk == 0))
                            {
                                if (0 < monsterHp[atk - 1])
                                {
                                    int Critical = r.Next(1, 101);

                                    int bh = monsterHp[atk - 1];
                                    player.Mp -= 10;
                                    if (Critical <= 15)
                                    {
                                        monsterHp[atk - 1] -= (int)Math.Ceiling(player.Atk * 3.2f); //  알파 스트라이크 데미지
                                        BattleField();
                                        Console.WriteLine($"\n\n{player.Name} 의 알파 스트라이크!\n");
                                        Console.WriteLine($"Lv.{monsters[atk - 1].Level} {monsters[atk - 1].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[atk - 1]}]- 치명타!!");
                                    }
                                    else
                                    {
                                        monsterHp[atk - 1] -= (int)Math.Ceiling(player.Atk * 2);
                                        BattleField();
                                        Console.WriteLine($"\n\n{player.Name} 의 알파 스트라이크!\n");
                                        Console.WriteLine($"Lv.{monsters[atk - 1].Level} {monsters[atk - 1].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[atk - 1]}]");
                                    }
                                    if (monsterHp[atk - 1] <= 0)
                                    {
                                        Console.WriteLine($"\nLv.{monsters[atk - 1].Level} {monsters[atk - 1].Name}\nHP {bh} -> Dead");
                                    }
                                    Console.WriteLine("\n\nEnter. 다음");
                                    Console.ReadLine();
                                    if (CheckMonsters() != 0) EnemyFrontPhase(); // 스킬 종료 후, 몬스터가 남아있으면 몬스터 턴
                                }
                                else
                                {
                                    Console.WriteLine("Dead 상태의 몬스터는 공격할 수 없습니다.\n\nEnter. 다음");
                                    Console.ReadLine();
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Mp 가 부족합니다.\n\nEnter. 다음");
                            Console.ReadLine();
                        }
                    }
                    else if (player is Warrior && use == 1)
                    {
                        Console.WriteLine("사용할 수 없는 스킬입니다.\n\nEnter. 다음");
                        Console.ReadLine();
                    }
                    if (player is Warrior && use == 2 && player.AvailableSkill[use - 1]) // 전사 2번 스킬 // 더블 스트라이크 - MP 15, 공격력 * 1.5 로 2명의 적을 랜덤으로 공격합니다.
                    {
                        if (15 <= player.Mp)
                        {
                            if (2 <= CheckMonsters())

                            {
                                int atk1; int atk2;
                                while (true)
                                {
                                    atk1 = r.Next(0, monsters.Count);
                                    atk2 = r.Next(0, monsters.Count);
                                    if (atk1 != atk2 && 0 < monsterHp[atk1] && 0 < monsterHp[atk2]) break;
                                }
                                int Critical = r.Next(1, 101);

                                int hp1 = monsterHp[atk1]; int hp2 = monsterHp[atk2];
                                player.Mp -= 15;
                                if (Critical <= 15)
                                {
                                    monsterHp[atk1] -= (int)Math.Ceiling(player.Atk * 2.4);
                                    monsterHp[atk2] -= (int)Math.Ceiling(player.Atk * 2.4);
                                    BattleField();
                                    Console.WriteLine($"\n\n{player.Name} 의 더블 스트라이크!\n");
                                    Console.WriteLine($"Lv.{monsters[atk1].Level} {monsters[atk1].Name} 을(를) 맞췄습니다. [데미지 : {hp1 - monsterHp[atk1]}]- 치명타!!");
                                    Console.WriteLine($"Lv.{monsters[atk2].Level} {monsters[atk2].Name} 을(를) 맞췄습니다. [데미지 : {hp2 - monsterHp[atk2]}]- 치명타!!");
                                }
                                else
                                {
                                    monsterHp[atk1] -= (int)Math.Ceiling(player.Atk * 1.5);
                                    monsterHp[atk2] -= (int)Math.Ceiling(player.Atk * 1.5);
                                    BattleField();
                                    Console.WriteLine($"\n\n{player.Name} 의 더블 스트라이크!\n");
                                    Console.WriteLine($"Lv.{monsters[atk1].Level} {monsters[atk1].Name} 을(를) 맞췄습니다. [데미지 : {hp1 - monsterHp[atk1]}]");
                                    Console.WriteLine($"Lv.{monsters[atk2].Level} {monsters[atk2].Name} 을(를) 맞췄습니다. [데미지 : {hp2 - monsterHp[atk2]}]");
                                }


                                if (monsterHp[atk1] <= 0)
                                {
                                    Console.WriteLine($"\nLv.{monsters[atk1].Level} {monsters[atk1].Name}\nHP {hp1} -> Dead");
                                }
                                if (monsterHp[atk2] <= 0)
                                {
                                    Console.WriteLine($"Lv.{monsters[atk2].Level} {monsters[atk2].Name}\nHP {hp2} -> Dead");
                                }
                                Console.WriteLine("\n\nEnter. 다음");
                                Console.ReadLine();
                                if (CheckMonsters() != 0) EnemyFrontPhase(); // 스킬 종료 후, 몬스터가 남아있으면 몬스터 턴
                            }
                            else if (CheckMonsters() == 1)
                            {
                                int atk;
                                while (true)
                                {
                                    atk = r.Next(0, monsters.Count);
                                    if (0 < monsterHp[atk]) break;
                                }
                                int Critical = r.Next(1, 101);
                                int bh = monsterHp[atk];
                                player.Mp -= 15;
                                if (Critical <= 15)
                                {
                                    monsterHp[atk] -= (int)Math.Ceiling(player.Atk * 2.4);
                                    BattleField();
                                    Console.WriteLine($"\n\n{player.Name} 의 더블 스트라이크!\n");
                                    Console.WriteLine($"Lv.{monsters[atk].Level} {monsters[atk].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[atk]}]- 치명타!!");
                                }
                                else
                                {
                                    monsterHp[atk] -= (int)Math.Ceiling(player.Atk * 1.5);
                                    BattleField();
                                    Console.WriteLine($"\n\n{player.Name} 의 더블 스트라이크!\n");
                                    Console.WriteLine($"Lv.{monsters[atk].Level} {monsters[atk].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[atk]}]");
                                }
                                if (monsterHp[0] <= 0)
                                {
                                    Console.WriteLine($"\nLv.{monsters[0].Level} {monsters[0].Name}\nHP {bh} -> Dead");
                                }
                                Console.WriteLine("\n\nEnter. 다음");
                                Console.ReadLine();
                                if (CheckMonsters() != 0) EnemyFrontPhase(); // 스킬 종료 후, 몬스터가 남아있으면 몬스터 턴
                            }
                        }
                        else
                        {
                            Console.WriteLine("MP 가 부족합니다.\n\nEnter. 다음");
                            Console.ReadLine();
                        }
                    }
                    else if (player is Warrior && use == 2)
                    {
                        Console.WriteLine("사용할 수 없는 스킬입니다.\n\nEnter. 다음");
                        Console.ReadLine();
                    }
                    //---------------- 마법사 스킬 -----------------------------------------------------------------------------------------------------------------------------------------------
                    if (player is Wizard && use == 1 && player.AvailableSkill[use - 1]) // 마법사 1번 스킬 "파이어 브레스 - MP 20, 마법력 * 0.5 로 모든 적을 공격하고, 화상 상태로 만듭니다.(화상 데미지x4)
                    {
                        if (20 <= player.Mp)
                        {
                            player.Mp -= 20;
                            for (int i = 0; i < monsters.Count; i++)
                            {
                                int bh = monsterHp[i];
                                if (0 < monsterHp[i])
                                {
                                    monsterHp[i] -= (int)Math.Ceiling(player.SkillAtk * 0.5f);
                                    monsterBurn[i] = 4;
                                    BattleField();
                                    Console.SetCursorPosition(0, 3 + i);
                                    Console.WriteLine($"◎");
                                    Console.SetCursorPosition(0, 11 + monsters.Count);
                                    Console.WriteLine($"\n\n{player.Name} 의 파이어 브레스!\n");
                                    Console.WriteLine($"Lv.{monsters[i].Level} {monsters[i].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[i]}]");
                                    Console.WriteLine($"Lv.{monsters[i].Level} {monsters[i].Name} 이(가) 화상을 입었습니다.");
                                    if (monsterHp[i] <= 0)
                                        Console.WriteLine($"\nLv.{monsters[i].Level} {monsters[i].Name}\nHP {bh} -> Dead");
                                    Console.WriteLine("\n\nEnter. 다음");
                                    Console.ReadLine();
                                }
                            }
                            if (CheckMonsters() != 0) EnemyFrontPhase();
                        }
                        else
                        {
                            Console.WriteLine("MP 가 부족합니다.\n\nEnter. 다음");
                            Console.ReadLine();
                        }
                    }
                    else if (player is Wizard && use == 1)
                    {
                        Console.WriteLine("사용할 수 없는 스킬입니다.\n\nEnter. 다음");
                        Console.ReadLine();
                    }
                    if (player is Wizard && use == 2 && player.AvailableSkill[use - 1]) // 마법사 2번 스킬 아이스 스피어 - MP 10, 마법력 + 10 으로 가장 앞에있는 적을 공격한다. 만약 적이 사망할 경우, 초과한 데미지만큼 다음 적이 데미지를 받는다.
                    {
                        if (10 <= player.Mp)
                        {
                            player.Mp -= 10;
                            if (2 <= CheckMonsters())
                            {
                                int atk = monsters.Count - 1;
                                for (int i = 0; i < monsters.Count; i++)
                                {
                                    if (0 < monsterHp[atk]) break;
                                    atk--;
                                }
                                int hp1 = monsterHp[atk]; int hp2 = monsterHp[atk - 1];
                                monsterHp[atk] -= player.SkillAtk + 10;
                                if (monsterHp[atk] < 0)
                                    monsterHp[atk - 1] += monsterHp[atk];
                                BattleField();
                                Console.WriteLine($"\n\n{player.Name} 의 아이스 스피어!\n");
                                if (monsterHp[atk] < 0)
                                    Console.WriteLine($"Lv.{monsters[atk - 1].Level} {monsters[atk - 1].Name} 을(를) 맞췄습니다. [데미지 : {hp2 - monsterHp[atk - 1]}]");
                                Console.WriteLine($"Lv.{monsters[atk].Level} {monsters[atk].Name} 을(를) 맞췄습니다. [데미지 : {hp1 - monsterHp[atk]}]");
                                if (monsterHp[atk - 1] <= 0)
                                    Console.WriteLine($"\nLv.{monsters[atk - 1].Level} {monsters[atk - 1].Name}\nHP {hp2} -> Dead");
                                if (monsterHp[atk] <= 0)
                                    Console.WriteLine($"\nLv.{monsters[atk].Level} {monsters[atk].Name}\nHP {hp1} -> Dead");
                                Console.WriteLine("\n\nEnter. 다음");
                                Console.ReadLine();
                                if (CheckMonsters() != 0) EnemyFrontPhase();
                            }
                            else
                            {
                                int atk;
                                while (true)
                                {
                                    atk = r.Next(0, monsters.Count);
                                    if (0 < monsterHp[atk]) break;
                                }
                                int bh = monsterHp[atk];
                                monsterHp[atk] -= player.SkillAtk + 10;
                                BattleField();
                                Console.WriteLine($"Lv.{monsters[atk].Level} {monsters[atk].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[atk]}]");
                                if (monsterHp[atk] <= 0)
                                    Console.WriteLine($"\nLv.{monsters[atk].Level} {monsters[atk].Name}\nHP {bh} -> Dead");
                                Console.WriteLine("\n\nEnter. 다음");
                                Console.ReadLine();
                                if (CheckMonsters() != 0) EnemyFrontPhase();
                            }
                        }
                        else
                        {
                            Console.WriteLine("MP 가 부족합니다.\n\nEnter. 다음");
                            Console.ReadLine();
                        }
                    }
                    else if (player is Wizard && use == 2)
                    {
                        Console.WriteLine("사용할 수 없는 스킬입니다.\n\nEnter. 다음");
                        Console.ReadLine();
                    }
                }

                void EnemyFrontPhase()
                {
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        int bh = monsterHp[i];
                        if (0 < monsterHp[i] && 0 < monsterBurn[i])
                        {
                            monsterHp[i] -= player.BurningDmg;
                            monsterBurn[i]--;
                            BattleField();
                            Console.SetCursorPosition(0, 3 + i);
                            Console.WriteLine($"♨");
                            Console.SetCursorPosition(0, 11 + monsters.Count);
                            Console.WriteLine($"\nLv.{monsters[i].Level} {monsters[i].Name} 이(가) 화상으로 데미지를 받았다. [데미지 : {bh - monsterHp[i]}]");
                            if (monsterHp[i] <= 0)
                                Console.WriteLine($"\nLv.{monsters[i].Level} {monsters[i].Name}\nHP {bh} -> Dead");
                            Console.WriteLine("\n\nEnter. 다음");
                            Console.ReadLine();
                        }
                    }
                    if (CheckMonsters() != 0) EnemyPhase();
                }

                void EnemyPhase() // 몬스터 턴, 몬스터 행동
                {
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        int befHp = player.Hp;
                        int damage = monsters[i].Atk - player.Def;    // 몬스터 데미지
                        if (damage < 0) damage = 0;
                        if (0 < monsterHp[i])
                        {

                            player.Hp -= damage;
                            BattleField();
                            Console.SetCursorPosition(0, 3 + i);
                            Console.WriteLine($"▶");
                            Console.SetCursorPosition(0, 11 + monsters.Count);
                            Console.WriteLine("\n");
                            Console.WriteLine($"Lv.{monsters[i].Level} {monsters[i].Name} 의 공격!\n{player.Name} 을(를) 맞췄습니다. [데미지 : {befHp - player.Hp}]\n");
                            if (player.Hp <= 0)
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

                int CheckMonsters()
                {
                    int live = 0;
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        if (0 < monsterHp[i]) live++;
                    }
                    return live;
                }

                void Victory() // 전투 승리 결과출력
                {
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        player.Exp += monsters[i].DropExp;
                    }
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("   Battle!! - Result\n\n");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("   Victory\n\n");
                    Console.ResetColor();
                    Console.WriteLine($"던전에서 몬스터 {monsters.Count}마리를 잡았습니다.\n\n");
                    Console.WriteLine($"Lv. {player.Level} {player.Name}\nHP {beforeHp} -> {player.Hp}\nMP {beforeMp} -> {player.Mp}\nExp {beforeExp} -> {player.Exp}\n\n");
                    Console.Write("던전 층수" + (dungeonFloor + 1) + "층 - >");
                    dungeonFloor++;

                    Console.Write((dungeonFloor + 1) + "층");
                    Console.ReadKey();
                    LevelUp();
                    GetRewards();
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
                    Console.WriteLine($"Lv. {player.Level} {player.Name}\nGold {player.Gold} - > 0\nExp {player.Exp} -> 0\n\n");
                    Console.WriteLine("\n마을로 돌아가 체력과 마나를 회복하였습니다.\n\nEnter. 다음");
                    Console.ReadLine();
                    player.Hp = player.MaxHp;
                    player.Mp = player.MaxMp;
                    player.Gold = 0;
                    player.Exp = 0;
                }
                void GetRewards() // 던전 보상 메서드
                {
                    Random r = new Random(); // 랜덤 객체 생성, 랜덤 숫자를 생성하려고
                    Dictionary<string, int> itemCounts = new Dictionary<string, int>();
                    // 딕셔너리? => string(아이템 이름), int(아이템 수량)을 가진 객체 생성,  아이템 이름으로 수량 추적
                    int totalGold = 0; //획득 골드 표시하려고, 일단 초기화

                    for (int i = 0; i < monsters.Count; i++) // 몬스터 리스트 수 만큼 반복
                    {
                        totalGold += monsters[i].Gold;  // 몬스터의 골드를 획득 골드에 추가
                        if (items.Count > 0) // 아이템 리스트에 아이템이 있는지 확인
                        {
                            int itemIdx = r.Next(items.Count); // 아이템 리스트 내에서 랜덤한 인덱스 선택
                            Item dropItem = items[itemIdx]; // 선택한 인덱스에 해당하는 아이템 가져 옴

                            if (itemCounts.ContainsKey(dropItem.Name)) // 이미 획득한 아이템이면
                            // ContainsKey -> 딕셔너리랑 같이 씀        아이템 이름 - 번호   
                            // dropItem.Name = 아이템 이름 문자열
                            {
                                itemCounts[dropItem.Name]++;  // 이미 있는 템이면 수량 1 +
                            }
                            else
                            {
                                itemCounts.Add(dropItem.Name, 1); // 없으면 새로 만들고 1개
                            }
                        }
                    }
                    Console.WriteLine("[획득 아이템]");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(" " + $"{totalGold}");
                    Console.ResetColor();
                    Console.Write(" Gold\n");
                    foreach (var item in itemCounts) // 획득한 아이템의 이름과 수량을 순회
                    {
                        Console.WriteLine("");
                        Console.Write(item.Key + " - "); // item.Key = 아이템 이름
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(item.Value); // item.Value = 아이템 수량
                        Console.ResetColor();
                        Console.Write(" 개");
                        Console.WriteLine("");
                    }
                }
            }
        }
    }
}
//