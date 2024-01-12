using System.Net;

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
            public int HpPotion { get; set; }
            public int MpPotion { get; set; }
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
                HpPotion = 3;
                MpPotion = 1;
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
                HpPotion = 3;
                MpPotion = 1;
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
            public string Name { get; set; }
            public int Type { get; }
            public int Atk { get; set; }
            public int SkillAtk { get; set; }
            public int Def { get; set; }
            public int Gold { get; set; }
            public int Quantity { get; set; } // 아이템 수량
            public bool IsEquiped { get; set; }
            public static int itemCount;
            public static int dropItemCount;
            public Item(string name, int type, int atk,int skillAtk, int def, int gold, int quantity = 1, bool isEquiped = false)
            {
                Name = name;
                Type = type;
                Atk = atk;
                SkillAtk = skillAtk;
                Def = def;
                Gold = gold;
                Quantity = quantity;
                IsEquiped = isEquiped;
            }

            public void PlayerInventoryList(bool withNumber, int idx = 0)
            {
                Console.Write("-");
                if (withNumber)
                {
                    Console.Write("{0}", idx);
                }
                if (IsEquiped)
                {
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("E");
                    Console.ResetColor();
                    Console.Write("]");
                }
                Console.Write(Name);
                Console.Write("  |  ");
                Console.WriteLine($"{Quantity}개");

            }
        }



        //----- 메인 -----------------------------------------------------------------------------------------------------------------------
        static void Main(string[] args)
        {
            Console.Title = "Team7SpartaDungeon"; // 콘솔 타이틀
            SpartaDungeon sd = new SpartaDungeon();
            sd.PlayGame();
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        public class SpartaDungeon
        {
            // 플레이어, 몬스터, 몬스터 리스트 dungeon 생성
            Player player = new Player();
            Monster minion = new Monster("미니언", 2, 10, 0, 15, 20, 300);
            Monster siegeMinion = new Monster("대포미니언", 5, 20, 0, 25, 80, 800);
            Monster voidBug = new Monster("공허충", 3, 15, 0, 10, 50, 500);

            List<Monster> dungeon = new List<Monster>();
            List<Item> items = new List<Item>(); // 아이템 리스트 초기화
            List<Item> haveItem = new List<Item>();
            List<Item> dropItem = new List<Item>();
            List<Item> equipItem = new List<Item>();

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
            public void ItemTable() // 아이템 리스트 보관용 메서드
            {
                items.Add(new Item("낡은 검", 0, 3,0, 0, 500));   // 무기, 공격력 3, 방어력 0, 가격 500
                items.Add(new Item("보통 검", 0, 7,0, 0, 1000));  // 무기, 공격력 7, 방어력 0, 가격 1000
                items.Add(new Item("낡은 갑옷", 1, 0,0, 7, 800));  // 방어구, 공격력 0, 방어력 7, 가격 800
                items.Add(new Item("보통 갑옷", 1, 0, 0,15, 1300)); // 방어구, 공격력 0, 방어력 15, 가격 1300
                items.Add(new Item("잡동사니", 2, 0,0, 0, 300));  // 잡템, 공격력 0, 방어력 0, 가격 300
            }

            public void PlayGame() // 게임 시작 메서드
            {
                dungeon.Add(minion);                 // 던전에서 출현할 몬스터 추가
                dungeon.Add(siegeMinion);
                dungeon.Add(voidBug);
                ItemTable(); // 아이템 리스트 보관용 메서드 초기화
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
                    Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.\n이제 전투를 시작할 수 있습니다.\n\n1. 상태 보기\n2. 전투 시작\n3. 인벤토리");
                    switch (ChoiceInput(1, 3)) // 최초 선택지
                    {
                        case 1:
                            Status();
                            break;
                        case 2:
                            BattleStart();
                            break;
                        case 3:
                            InventoryMenu(); // 아이템 획득 테스트 때문에 임시로 추가
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

                    player.Def += 1;
                    if (player.Class == "전사")
                    {
                        player.Atk += 5;
                        player.MaxHp += 10;
                        player.MaxMp += 5;
                    }
                    else
                    {
                        player.SkillAtk += 5;
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

            public void InventoryMenu()  // 인벤토리
            {
                haveItem.Add(items[0]);
                Console.Clear();
                Console.WriteLine("인벤토리");
                Console.WriteLine("아이템을 관리할 수 있습니다.\n \n [아이템 목록]");
                if (haveItem.Count <= 0)
                {
                    Console.WriteLine("가진 아이템이 없습니다.");
                }
                else
                {
                    for (int i = 0; i <= Item.itemCount; i++)
                    {
                        haveItem[i].PlayerInventoryList(false, 0);
                    }
                }
                Console.WriteLine("1. 장착관리 \n0. 뒤로가기");
                switch (ChoiceInput(0, 1))
                {
                    case 0:
                        break;
                    case 1:
                        Equip();
                        break;
                }
            }
            public void Equip() //장착관리
            {
                Console.Clear();
                Console.WriteLine("장착관리 \n보유 중인 아이템을 관리할 수 있습니다.");
                Console.WriteLine("");
                Console.WriteLine("[아이템 목록]");
                for (int i = 0; i <= Item.itemCount; i++)
                {
                    haveItem[i].PlayerInventoryList(true, i + 1);
                }

                Console.WriteLine("\n0.돌아가기");
                int keyInput = ChoiceInput(0, haveItem.Count);
                switch (keyInput)
                {
                    case 0:
                        break;
                    default:
                        ItemEpuipToggle(keyInput - 1);
                        Equip();
                        break;
                }

            }

            

            private void ItemEpuipToggle(int idx)
            {
                haveItem[idx].IsEquiped = !haveItem[idx].IsEquiped;
                player.Atk += haveItem[idx].Atk;
               
            }

            public void BattleStart() // 2. 전투 시작
            {
                List<Monster> monsters = new List<Monster>(); // 몬스터 리스트 monsters 생성
                Random r = new Random();
                int beforeHp = player.Hp;
                int beforeMp = player.Mp;
                int beforeExp = player.Exp;
                for (int i = 0; i < r.Next(1, 5); i++)
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
                    Console.WriteLine("\n\n1. 공격\n2. 스킬\n3. 회복 아이템");
                    switch (ChoiceInput(1, 3))
                    {
                        case 1:
                            Attack();
                            break;
                        case 2:
                            Skill();
                            break;
                        case 3:
                            HpRecovery();
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
                                int next = 1;
                                for (int i = 0; i < monsters.Count; i++)
                                {
                                    if (0 < monsterHp[atk]) break;
                                    atk--;
                                }
                                while (true)
                                {
                                    if (0 < monsterHp[atk - next]) break;
                                    next++;
                                }
                                int hp1 = monsterHp[atk]; int hp2 = monsterHp[atk - next];
                                if (monsterHp[atk - next] < 0) next -= 1;
                                monsterHp[atk] -= player.SkillAtk + 10;
                                if (monsterHp[atk] < 0)
                                    monsterHp[atk - next] += monsterHp[atk];
                                BattleField();
                                Console.WriteLine($"\n\n{player.Name} 의 아이스 스피어!\n");
                                if (monsterHp[atk] < 0)
                                    Console.WriteLine($"Lv.{monsters[atk - next].Level} {monsters[atk - next].Name} 을(를) 맞췄습니다. [데미지 : {hp2 - monsterHp[atk - next]}]");
                                Console.WriteLine($"Lv.{monsters[atk].Level} {monsters[atk].Name} 을(를) 맞췄습니다. [데미지 : {hp1 - monsterHp[atk]}]");
                                if (monsterHp[atk - next] <= 0)
                                    Console.WriteLine($"\nLv.{monsters[atk - next].Level} {monsters[atk - next].Name}\nHP {hp2} -> Dead");
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
                    Console.Clear();
                    Random r = new Random(); // 랜덤 객체 생성, 랜덤 숫자를 생성하려고
                    int totalGold = 0; //획득 골드 표시하려고, 일단 초기화

                    for (int i = 0; i < monsters.Count; i++) // 몬스터 리스트 수 만큼 반복
                    {
                        totalGold += monsters[i].Gold;  // 몬스터의 골드를 획득 골드에 추가
                        if (items.Count > 0) // 아이템 리스트에 아이템이 있는지 확인
                        {
                            int itemIdx = r.Next(items.Count); // 아이템 리스트 내에서 랜덤한 인덱스 선택
                            dropItem.Add(items[itemIdx]);
                            Item.dropItemCount += itemIdx;

                            //
                            var existingItem = haveItem.FirstOrDefault(it => it.Name == dropItem[i].Name);
                            if (existingItem != null)
                            {
                                existingItem.Quantity++; // 이미 있는 아이템이면 수량 증가
                            }
                            else
                            {

                                haveItem.Add(dropItem[i]); // 새 아이템 추가
                                Item.itemCount++;



                            }
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\n[획득 아이템]\n");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(" " + $"{totalGold}");
                    Console.ResetColor();
                    player.Gold += totalGold;
                    Console.Write(" Gold\n");
                    foreach (var item in haveItem) // 아이템의 이름과 수량을 순회
                    {
                        Console.WriteLine("");
                        Console.Write($"{item.Name} - "); // 아이템 이름
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(item.Quantity); // 아이템 수량
                        Console.ResetColor();
                        Console.Write(" 개\n");
                    }
                }
                void HpRecovery()
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[회복]\n");
                    Console.ResetColor();
                    Console.Write("체력, 마나 포션을 사용하면 Hp/Mp를 ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("30");
                    Console.ResetColor();
                    Console.Write(" 회복 할 수 있습니다.\n");
                    Console.Write("\n(남은 체력 포션 : ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(player.HpPotion);
                    Console.ResetColor();
                    Console.Write(" )\n");
                    Console.Write("\n(남은 마나 포션 : ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(player.MpPotion);
                    Console.ResetColor();
                    Console.Write(" )\n");

                    while (true)
                    {
                        Console.WriteLine("\n1. 체력 회복 \n2. 마나 회복 \n3. 돌아가기");
                        switch (ChoiceInput(1, 3))
                        {
                            case 1:
                                UseHpPotion();
                                break;
                            case 2:
                                UseMpPotion();
                                break;
                            case 3:
                                // 전투 중인 곳으로 돌아가기
                                return;
                        }
                    }
                }
                void UseHpPotion()
                {
                    if (player.HpPotion > 0)
                    {
                        if (player.Hp < player.MaxHp)
                        {
                            int currentHp = player.Hp; // 현재 체력 저장, 플레이어hp
                            player.Hp = Math.Min(player.Hp + 30, player.MaxHp);
                            int recoveryHp = player.Hp - currentHp; // 회복량 계산 
                            player.HpPotion--; // 1개 소모
                            Console.Write("\n체력을 ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(recoveryHp); // 회복량 출력
                            Console.ResetColor();
                            Console.Write("회복 하였습니다.\n");
                            Console.WriteLine("\n");
                            Console.Write("플레이어의 현재 체력 : ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(player.Hp);
                            Console.ResetColor();
                            Console.WriteLine();
                            Console.Write("남은 체력 포션 : ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(player.HpPotion);
                            Console.ResetColor();
                            Console.WriteLine();
                        }
                        else if (player.Hp >= player.MaxHp)
                        {
                            Console.Write("이미 ");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("최대 체력");
                            Console.ResetColor();
                            Console.Write("입니다.");
                            Console.WriteLine();
                        }

                        Console.Write("입니다.");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("체력 포션이 ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("부족");
                        Console.ResetColor();
                        Console.Write("합니다!");
                        Console.WriteLine();
                    }
                }
                void UseMpPotion()
                {
                    if (player.MpPotion > 0)
                    {
                        if (player.Mp < player.MaxMp)
                        {
                            int currentMp = player.Mp; // 현재 마나 저장, 플레이어hp
                            player.Mp = Math.Min(player.Mp + 30, player.MaxMp);
                            int recoveryMp = player.Mp - currentMp; // 회복량 계산 
                            player.MpPotion--; // 1개 소모
                            Console.Write("\n체력을 ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(recoveryMp); // 회복량 출력
                            Console.ResetColor();
                            Console.Write("회복 하였습니다.\n");
                            Console.WriteLine("\n");
                            Console.Write("플레이어의 현재 마나 : ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(player.Mp);
                            Console.ResetColor();
                            Console.WriteLine();
                            Console.Write("남은 마나 포션 : ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(player.MpPotion);
                            Console.ResetColor();
                            Console.WriteLine();
                        }
                        else if (player.Mp >= player.MaxMp)
                        {
                            Console.Write("이미 ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("최대 마나");
                            Console.ResetColor();
                            Console.Write("입니다.");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.Write("마나 포션이 ");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("부족");
                            Console.ResetColor();
                            Console.Write("합니다!");
                            Console.WriteLine();
                        }
                    }
                }
            }

        }
    }
}
//