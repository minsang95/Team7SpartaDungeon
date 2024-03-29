using System.Net;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.IO;
using static Team7SpartaDungeon.Program;
using System.Threading;
using System.Media;
using System.Threading.Channels;
using System.Text;


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
            public int Dex { get; set; }
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
                Dex = 5;
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
                Dex = 5;
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

                BurningDmg = 6;

            }
        }

        public class Musician : Player
        {
            public Musician()
            {
                Name = "이름";
                Class = "음악가";
                Level = 1;
                Atk = 6;
                Def = 3;
                SkillAtk = 6;
                MaxHp = 70;
                Hp = 70;
                MaxMp = 70;
                Mp = 70;
                Gold = 1500;
                Exp = 0;
                MaxExp = 30;
                HpPotion = 3;
                MpPotion = 1;
                AvailableSkill = new List<bool>();
                Skill = new List<string>();
            }
        }

        struct Monster
        {
            public string Name { get; set; }
            public int Level { set; get; }
            public int Atk { get; set; }
            public int Def { get; set; }
            public int Dex { get; set; }
            public int Hp { get; set; }
            public int DropExp { get; }
            public int Gold { get; set; }
            public Monster(string name, int level, int atk, int def, int dex, int hp, int dropExp, int gold)

            {
                Name = name;
                Level = level;
                Atk = atk;
                Def = def;
                Dex = dex;
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
            public bool IsPurchased { get; set; }
            public static int shopItemCount;        //상점 쇼핑 카운트 증가 해야 예외처리가 가능해서 추가
            public static int itemCount;
            public static int dropItemCount;
            public Item(string name, int type, int atk, int skillAtk, int def, int gold, int quantity = 1, bool isEquiped = false)
            {
                Name = name;
                Type = type;
                Atk = atk;
                SkillAtk = skillAtk;
                Def = def;
                Gold = gold;
                Quantity = quantity;
                IsEquiped = isEquiped;
                IsPurchased = false;
            }
            public void HighlightPurchased(string s)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(s);
                Console.ResetColor();
            }
            public void ShopList(bool withNumber, int idx = 0)  // 상점 쇼핑할때
            {
                Console.Write("-");

                if (withNumber)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write("{0}", idx);
                    Console.ResetColor();
                }
                Console.Write(Name);
                Console.Write("  |  ");

                if (Atk != 0) Console.Write($"Atk {(Atk >= 0 ? " + " : "")}{Atk}");
                if (Def != 0) Console.Write($"Def {(Def >= 0 ? " + " : "")}{Def}");
                Console.Write("  |  ");
                if (IsPurchased)
                {
                    HighlightPurchased("구매완료");
                }
                if (!IsPurchased)
                {
                    Console.WriteLine(Gold + "G");
                }


            }
            public void PlayerInventoryList(bool withNumber, int idx = 0)
            {

                Console.Write("-");
                if (withNumber)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write("{0}", idx);
                    Console.ResetColor();
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
                Console.Write($"ATk: {Atk}");
                Console.Write("  |  ");
                Console.Write($"Def: {Def}");
                Console.Write("  |  ");
                Console.WriteLine(Quantity + "개");
            }
            public void SellItemList(bool withNumber, int idx)
            {
                if (withNumber)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write("{0}", idx);
                    Console.ResetColor();
                }
                Console.Write(Name);
                Console.Write("  |  ");
                Console.Write((int)Gold * 0.85f + "G");
                Console.Write("  |  ");
                Console.Write($"ATk: {Atk}");
                Console.Write("  |  ");
                Console.Write($"Def: {Def}");
                Console.Write("  |  ");
                Console.WriteLine($"{Quantity}개");
            }
        }

        //----- 메인 -----------------------------------------------------------------------------------------------------------------------
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // 아스키 특수문자 호환용
            Console.Title = "Team7SpartaDungeon"; // 콘솔 타이틀
            SoundPlayer player = new SoundPlayer(@"C:\bgm.wav"); // C드라이브 bgm.wav 재생 
            player.PlayLooping(); // bgm 루프
            PrintStartLogo();
           
            WarriorIntroduce();
            WizardIntroduce();
            MusicianIntroduce();
            SpartaDungeon sd = new SpartaDungeon();
            sd.PlayGame();
            Console.Beep();
        }
        public class SpartaDungeon
        {
            static int dungeonFloor = 0;       // 던전 층수
                                               // 플레이어, 몬스터, 몬스터 리스트 dungeon 생성
            Player player = new Player();
            Monster minion = new Monster("미니언", 2, 10, 0, 10, 15, 20, 300);
            Monster siegeMinion = new Monster("대포미니언", 5, 20, 0, 10, 25, 80, 800);
            Monster voidBug = new Monster("공허충", 3, 15, 0, 10, 10, 50, 500);
            Monster Hansole = new Monster("이한솔매니저님", 5, 20, 0, 10, 30, 100, 1000);

            List<Monster> dungeon = new List<Monster>();
            List<Item> items = new List<Item>(); // 아이템 리스트 초기화
            List<Item> haveItem = new List<Item>();
            List<Item> dropItem = new List<Item>();
            List<Item> shopItem = new List<Item>();
            List<Item> hands = new List<Item>(); //장착부위 - 손
            List<Item> body = new List<Item>();  //장착부위 - 몸통


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

            public void ShopItemTable()  //상점 아이템
            {
                shopItem.Add(new Item("[상점] 빛을 잃은 검", 0, 10, 0, 0, 500));   // 무기, 공격력 3, 방어력 0, 가격 500
                shopItem.Add(new Item("[상점] 빛나는 검", 0, 7, 0, 0, 1000));  // 무기, 공격력 7, 방어력 0, 가격 1000
                shopItem.Add(new Item("[상점] 빛을 잃은 갑옷", 1, 0, 0, 7, 800));  // 방어구, 공격력 0, 방어력 7, 가격 800
                shopItem.Add(new Item("[상점] 빛나는 갑옷", 1, 0, 0, 15, 1300)); // 방어구, 공격력 0, 방어력 15, 가격 1300

            }

            public void ItemTable() // 아이템 리스트 보관용 메서드
                                    // 아이템 리스트 보관용
            {
                items.Add(new Item("낡은 검", 0, 3, 0, 0, 500));   // 무기, 공격력 3, 방어력 0, 가격 500
                items.Add(new Item("보통 검", 0, 7, 0, 0, 1000));  // 무기, 공격력 7, 방어력 0, 가격 1000
                items.Add(new Item("낡은 갑옷", 1, 0, 0, 7, 800));  // 방어구, 공격력 0, 방어력 7, 가격 800
                items.Add(new Item("보통 갑옷", 1, 0, 0, 15, 1300)); // 방어구, 공격력 0, 방어력 15, 가격 1300
                items.Add(new Item("잡동사니", 2, 0, 0, 0, 300));  // 잡템, 공격력 0, 방어력 0, 가격 300
            }

            public void PlayStartLogo2() // 직업 선택
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("⠀⠀⠀Warrior ⣠⣴⣶⣶⣶⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀                  ⠀⠀⠀⠀⠀⠀Musician");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⢀⠀⣴⣿⣿⣿⣿⣿⣿⣿⣿⣧⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
                Console.WriteLine("⠀⠀⠀⠀⢀⣀⣸⣽⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠢⠀⠀⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠤⣀⡠⠔⠋⠉⠉⠉⠉⠒⢄⠀");
                Console.WriteLine("⠀⠀⢠⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡀⠀⠀⠀⠀⠀⠀⠀     ⠀⠀⠀⠀Wizard⠀⠀⠀⠀⢡⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠴⠊⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢇");
                Console.WriteLine("⠀⢠⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡀⠤⠒⠒⠒⠉⠉⡗⠗⠢⠤⣀⠀⠀⠀⠀⠀⠀⠀⡔⠁⠀⠀⠀⠀⠀⠀⠀⣀⠤⠤⢄⣀⠀⠀⠘⡆");
                Console.WriteLine("⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠔⠊⠁⠀⠀⠀⠀⠀⠀⠀⠁⠀⠀⠀⠀⠙⠢⡀⠀⠀⠀⡞⠀⠀⠀⠀⢀⡠⠤⠄⠎⠀⠀⠀⠀⠈⠑⡄⠀");
                Console.WriteLine("⠀⠘⣿⣿⣿⣿⣿⣿⣿⠛⠛⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠣⡄⠀⡇⠀⠀⢀⠔⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢳⠀⠸⠄");
                Console.WriteLine("⠀⠀⠘⣿⣿⣿⣿⣿⡿⠀⠀⠀⠀⠀⠉⠛⠿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⡜⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢆⣳⠂⢀⠏⠀⡠⠀⠀⢄⠀⠀⠀⢠⠊⠀⠃⢸⠀⠀⠀⠈⠓");
                Console.WriteLine("⠀⠀⠀⠘⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢻⠻⢿⣿⠀⠀⠀⠀⠀⠀⠀⡞⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣷⡀⢸⡀⠀⠃⠀⡀⠀⠃⠀⠀⣣⡖⠀⠀⢸⠀⠀⠀⠀");
                Console.WriteLine("⠀⠀⠀⠠⠚⠛⠿⣿⡇⠐⠉⠉⢒⣄⡀⠀⢀⣴⣧⢸⠀⠀⠉⠀⠀⠀⠀⠀⠀⡜⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢧⢸⠃⠀⠀⠈⠘⠃⠀⢀⣀⣁⣀⠀⠀⠛⠲⡄⠀⠀⠀⠂");
                Console.WriteLine("⠀⠀⠀⠐⡄⠀⠀⠀⠁⠀⠀⠨⠻⠿⠃⠀⠀⠨⡁⢸⠀⠀⠀⠀⠀⠀⠀⠀⢀⠃⠀⠀⠀⢀⡤⠶⠒⠲⢤⡀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠘⡎⢆⠀⠀⠀⠀⠀⣰⣿⣿⣿⣿⣿⣆⠀⠀⢸⠀⢀⡤⠃⠀");
                Console.WriteLine("⠀⠀⠀⠀⠈⠢⢀⣀⠀⠀⠀⠀⠀⠀⠀⢄⠤⠤⠂⠀⠱⣄⠀⠀⠀⠀⠀⠀⢸⠀⠀⠀⠀⠚⠀⠀⠀⠀⠀⠁⠀⠀⠀⠀⠀⠀⠐⠋⠈⠉⠃⠀⠀⠀⠀⠀⠀⠀⡇⠀⡇⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⡇⠀⠰⣼⠤⠎⠀⠀");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⢣⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡪⠃⠀⠀⠀⠀⠀⠈⡀⠀⠀⠀⠀⠀⠀⢀⠤⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⠃⠀⡇⡀⠀⠀⠀⠘⢿⣿⣿⣿⠟⠀⢀⠜⠀⠀");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⢆⠀⠀⠀⠒⠉⠉⠉⠐⠒⠀⠒⠁⡀⠀⠀⠀⠀⠀⠀⠀⢣⠀⠀⠀⠀⠀⠀⡎⠎⣹⣷⠀⠀⠀⠀⠀⠀⡰⣮⡉⠓⡄⠀⠀⠀⠀⠀⠀⡎⠒⠒⠛⠄⣰⣀⣀⡤⠤⡗⠒⢲⣶⣏");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠈⢢⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠇⠀⠀⠀⠀⠀⠀⠀⠀⠳⡀⠀⠀⠀⠀⡇⢿⣿⣿⠖⠒⠤⢄⠀⢰⣥⣿⡿⠀⡀⠀⠀⠀⠀⠀⡼⠀⠀⠀⠀⡠⣿⣿⣿⣧⣴⣷⣶⣿⣿⣷⡈⠂⢄⠀⠀");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠡⠀⠀⠀⠀⠀⠀⠀⠀⠀⠜⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢈⠶⠶⠟⠃⠘⠛⢻⠉⠀⠀⠀⠀⢳⠘⠿⠿⠔⠺⠛⠽⠟⠒⢂⠜⠀⠀⠀⢀⠊⣰⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣦⣄⠉⠄⠀");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⢀⠀⠤⠰⠕⢄⠀⠀⠀⠀⢀⡤⣊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡎⠀⠀⠀⠀⠀⠀⠘⡄⠀⠀⠀⠀⡼⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⡄⠀⠀⡠⠁⢠⣿⣿⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣧⡀⠈⠂");
                Console.WriteLine("⠀⠀⠀⠀⡠⠒⠉⠀⠀⠀⠀⡆⠀⢂⠀⠀⠀⡌⡄⠰⠈⠑⠢⢀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠈⠓⠒⠒⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠀⢠⠁⠀⢸⠏⠀⢀⡾⠛⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣧⢀");
                Console.WriteLine("⠀⠀⠀⡘⠀⠀⠀⠀⠀⠀⠀⢰⠀⠀⠑⡀⠀⠇⠀⠀⠀⠀⠀⠀⠡⡀⠀⠀⠀⠀⠀⠱⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡸⠀⡨⠀⠀⠆⠀⣠⠋⠀⢀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡆⠁⠀");
                Console.WriteLine("⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠈⣄⠊⠉⠚⢆⡐⠀⠶⠀⠀⠀⠀⠀⠑⡄⠀⠀⠀⠀⠀⠈⠢⢤⣀⣀⣤⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡠⠖⠁⠎⠀⠀⢸⣀⡀⠁⢀⣴⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀");
                Console.WriteLine("⠀⠀⡐⠁⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⠀⠈⠃⠀⠀⠐⠒⠒⠠⡀⢸⠘⠢⡀⠀⠀⠀⠀⠀⠀⠀⢀⠇⡹⠉⠣⡤⣆⣀⣠⠤⡶⢲⠖⢖⠊⠉⠀⠀⠀⠀⠣⠤⣺⠉⠀⢈⠆⠁⠀⢹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠁⠀");
                Console.WriteLine("⠀⠰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠀⠀⠀⠀⠀⠀⠀⠈⠢⠀⠀⠈⢺⡄⠀⢡⠀⠀⠀⠀⠀⠀⢀⠆⣴⠃⠀⡘⠀⠀⠀⢠⠊⠀⠘⡀⠈⠻⠲⠤⠤⠤⠤⠒⠉⠈⡗⠒⠉⠀⠢⡤⠊⢻⣿⣿⣿⣿⣿⣿⣿⣿⠟⠁⠀");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠃⠀⢸⠀⠀⠀⠀⢀⡴⠋⡰⠉⡄⠀⠀⠀⠀⡠⠁⠀⠀⠀⢣⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢄⣀⣀⣠⣀⣠⣾⣿⣿⣿⣿⣿⣿⠟⡇⠀");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠓⠄⡐⠒⠺⣀⠀⣀⠴⠊⠀⢀⠇⠀⠈⠢⠤⠄⠊⠀⠀⠀⠀⠀⠸⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡸⢻⠛⠻⠛⠛⠋⠉⠉⠀⡇⠀");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡈⠀⠁⢰⠀⠀⢠⠀⠷⡀⠀⠀⠀⣸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢳⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡰⠃⡸⠀⠀⠀⠀⠀⠀⢰⠀⠧⣀⠀");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢈⠘⢄⠀⠈⢄⠐⠁⠀⢀⠇⠀⠀⢠⢷⠒⠀⠉⠉⠀⠒⠢⢄⠀⠀⠀⠀⠀⠀⡆⠀⠀⠀⢀⡀⠀⠀⠀⠀⢀⡠⠊⡔⠉⠀⠀⡠⠀⠀⠀⠀⢘⠀⠀⠀⠈⠐⠄⠀");
                Console.WriteLine("\r\n⠀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢄⠀⢁⡹⠀⠀⢀⣠⠎⠀⢀⡴⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠇⠀⠀⠀⠀⠙⡒⠒⠒⠒⠉⠀⠀⢄⣀⠠⣒⣀⠀⠀⠀⠀⠈⠢⣀⡀");
                Console.WriteLine("⠀⠈⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⠸⣍⠉⠉⠁⠀⢀⣠⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⢀⣠⠤⠂⠉⠀⠀⠉⢢⠀");
                Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠉⠉⠁⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡆⠀⠀⠀⠀⠀⢱⠤⠔⠂⠉⠁⠀⠀⠀⠀⠀⠀⠀⡸⠀");
                Console.WriteLine("");
                ShowHighlightText("==================================================================================================================");
                Console.WriteLine("");
                ShowHighlightText("                                    <<    7조의 대모험이⠀시작됩니다 !⠀  >>");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("                                                                                                 Press Any Key");                                                                                        
                ShowHighlightText("=================================================================================================================");
                Console.ReadKey();
            }

            public void PlayGame() // 게임 시작 메서드
            {
                ItemTable();
                dungeon.Add(minion);                 // 던전에서 출현할 몬스터 추가
                dungeon.Add(siegeMinion);
                dungeon.Add(voidBug);
                dungeon.Add(minion);
                dungeon.Add(siegeMinion);
                dungeon.Add(voidBug);
                dungeon.Add(minion);
                dungeon.Add(siegeMinion);
                dungeon.Add(voidBug);

                ShopItemTable();

                Console.Clear();
                PlayStartLogo2(); // 직업 선택씬
                Console.WriteLine("");
                ShowHighlightText("                                           직업을  선택 해주세요!");
                Console.WriteLine("");
                Console.WriteLine("                    1. 전사                      2. 마법사                   3. 음악가       ");
                switch (ChoiceInput(1, 3)) // 직업 선택
                {
                    case 1:
                        player = new Warrior();
                        player.Skill.Add("알파 스트라이크 - MP 10\n   공격력 * 2 로 하나의 적을 공격합니다."); // 전사 1번 스킬 추가
                        player.AvailableSkill.Add(true);
                        player.Skill.Add("더블 스트라이크 - MP 15\n   공격력 * 1.5 로 2명의 적을 랜덤으로 공격합니다."); // 전사 2번 스킬 추가
                        player.AvailableSkill.Add(false);
                        break;
                    case 2:
                        player = new Wizard();

                        player.Skill.Add("파이어 브레스 - MP 20\n   마법력 * 0.4 로 모든 적을 공격하고, 화상 상태로 만듭니다.(화상 데미지 6 x 4)"); // 마법사 1번 스킬 추가
                        player.AvailableSkill.Add(true);
                        player.Skill.Add("아이스 스피어 - MP 10\n   마법력 + 10 으로 가장 앞에있는 적을 공격한다. 만약 적이 사망할 경우, 초과한 데미지만큼 다음 적이 데미지를 받는다."); // 마법사 2번스킬 추가
                        player.AvailableSkill.Add(false);
                        player.Skill.Add("메디테이션 - MP +10\n   일시적으로 마법력 * 0.5 의 방어력을 얻고 명상에 빠진다.");

                        player.AvailableSkill.Add(true);
                        break;
                    case 3:
                        player = new Musician();
                        player.Skill.Add("타임 코스모스 \"깐따삐아\" ! - MP 70\n  모든 마나를 소모하고 시간을 게임 시작 전으로 되돌립니다.");
                        player.AvailableSkill.Add(true);
                        break;
                }
                ShowHighlightText(" 원하시는 이름을 설정해주세요! \n");
                player.Name = Console.ReadLine();    // 플레이어 이름 입력
                while (true)
                {
                    Console.Clear();

                    Console.WriteLine($" {player.Name} 님 반갑습니다! \n이제 전투를 시작할 수 있습니다.\n\n1. 상태 보기\n2. 전투 시작( 현재 진행 : " + (dungeonFloor + 1) + " 층 )\n3. 인벤토리\n4. 상점\n5. 저장 / 불러오기\n");
                    switch (ChoiceInput(1, 5)) // 최초 선택지
                    {
                        case 1:
                            Status();
                            break;
                        case 2:
                            BattleStart();
                            break;
                        case 3:
                            InventoryMenu();
                            break;
                        case 4:
                            StoreMenu();
                            break;
                        case 5:
                            SaveGameMenu(); // 저장 및 불러오기 메뉴
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


                    if (!(player is Musician) && 3 == player.Level)
                    {
                        player.AvailableSkill[1] = true;
                        if (player is Warrior) Console.WriteLine("\n\n스킬 \"더블 스트라이크\"를 사용할 수 있습니다.");
                        if (player is Wizard) Console.WriteLine("\n\n스킬 \"아이스 스피어\"를 사용할 수 있습니다");
                    }

                    Console.ReadKey();

                }
            }
            public void Status() // 1. 상태 보기
            {
                Console.Clear();
                Console.WriteLine($"캐릭터의 정보가 표시됩니다.\n\n" +
                                  $" 이름   : {player.Name}\n" +
                                  $" 레벨   : {player.Level}\n 직업   : {player.Class}\n 공격력 : {player.Atk}\n 방어력 : {player.Def}\n 마법력 : {player.SkillAtk}\n 민첩성 : {player.Dex}\n" +
                                  $" 체 력  : {player.Hp}/{player.MaxHp}\n 마 나  : {player.Mp}/{player.MaxMp}\n Gold   : {player.Gold} G\n 경험치 : {player.Exp} / {player.MaxExp}\n\n" +
                                  $"Enter. 나가기");
                Console.ReadLine();
            }
            public void StoreMenu()   //상점페이지
            {
                Console.Clear();
                Console.WriteLine("■ 상 점 ■");
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
                Console.WriteLine("");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine(player.Gold + "G");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("[아이템 목록]");
                for (int i = 0; i < 4; i++)
                {
                    shopItem[i].ShopList(false, 0);
                }
                Console.WriteLine("");
                Console.WriteLine("0. 나가기");
                Console.WriteLine("1. 아이템 구매");
                Console.WriteLine("2. 아이템 판매");
                switch (ChoiceInput(0, 2))
                {
                    case 0:
                        break;
                    case 1:
                        Shop();
                        break;
                    case 2:
                        SellMenu();
                        break;
                }
            }

            public void InventoryMenu()  // 인벤토리
            {
                Console.Clear();
                Console.WriteLine("인벤토리");
                Console.WriteLine("아이템을 관리할 수 있습니다.\n \n [아이템 목록]");
                if (haveItem.Count <= 0)
                {

                    Console.WriteLine(" --현재 보유한 아이템이 없습니다--");

                }
                else
                {
                    for (int i = 0; i < haveItem.Count; i++)
                    {
                        haveItem[i].PlayerInventoryList(false, 0);
                    }
                }


                Console.WriteLine("");
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

                if (haveItem.Count <= 0 && Item.shopItemCount <= 0)
                {
                    Console.WriteLine("가진 아이템이 없습니다.");
                }
                else
                {
                    for (int i = 0; i < Item.itemCount; i++)
                    {
                        haveItem[i].PlayerInventoryList(true, i + 1);
                    }

                }
                Console.WriteLine("");
                Console.WriteLine("장착하고 싶은 아이템 숫자를 선택하세요");
                Console.WriteLine("\n0.돌아가기");
                int keyInput = ChoiceInput(0, haveItem.Count);
                switch (keyInput)
                {
                    case 0:
                        InventoryMenu();
                        break;
                    default:
                        ItemEpuipToggle(keyInput - 1);
                        Equip();
                        break;
                }

            }

            public void Shop()
            {
                Console.Clear();
                Console.WriteLine("■ 상 점 - 구매하기 ■");
                Console.WriteLine("필요한 아이템을 구매 할 수 있습니다.\n");
                Console.WriteLine("");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine(player.Gold + "G");
                Console.WriteLine("");
                Console.WriteLine("[아이템 목록]");
                Console.WriteLine("");
                for (int i = 0; i < 4; i++)
                {
                    shopItem[i].ShopList(true, i + 1);
                }
                Console.WriteLine("\n구매하고 싶은 아이템 번호를 입력 해주세요.");
                Console.WriteLine("0을 입력하면 상점으로 돌아갑니다.");
                Console.WriteLine("");

                int choice = ChoiceInput(0, 4); // 0 입력 가능
                if (choice == 0) // 0 입력 시 상점 메뉴로 복귀
                {
                    StoreMenu();
                    return;
                }

                choice -= 1;
                Item selectedItem = shopItem[choice];
                if (selectedItem.IsPurchased)
                {
                    Console.WriteLine("이미 구매한 아이템입니다.");
                }
                else if (player.Gold >= selectedItem.Gold)
                {
                    selectedItem.IsPurchased = true;
                    haveItem.Add(shopItem[choice]);
                    player.Gold -= selectedItem.Gold;


                    Console.WriteLine($"\n{selectedItem.Name} 구매를 완료했습니다.");
                    Item.shopItemCount++;
                    Item.itemCount++;

                }
                else
                {
                    Console.WriteLine("Gold가 부족합니다.");
                }

                Console.WriteLine("아무 키나 누르면, 상점으로 돌아갑니다.");
                Console.ReadKey();
                StoreMenu();
            }


            private void ItemEpuipToggle(int idx) //아이템 장착과 스탯 증감
            {
                if (!haveItem[idx].IsEquiped)
                {
                    if (haveItem[idx].Type == 0)
                    {
                        if (hands.Count == 0)
                        {
                            haveItem[idx].IsEquiped = true;
                            hands.Add(haveItem[idx]);
                            StatIncrease(idx);
                        }
                        else
                        {
                            hands[0].IsEquiped = false;
                            haveItem[idx].IsEquiped = true;
                            hands.Clear();
                            hands.Add(haveItem[idx]);
                            StatIncrease(idx);
                        }
                    }
                    else if (haveItem[idx].Type == 1)
                    {
                        if (body.Count == 0)
                        {
                            haveItem[idx].IsEquiped = true;
                            body.Add(haveItem[idx]);
                            StatIncrease(idx);
                        }
                        else
                        {
                            body[0].IsEquiped = false;
                            haveItem[idx].IsEquiped = true;
                            body.Clear();
                            body.Add(haveItem[idx]);
                            StatIncrease(idx);
                        }
                    }
                    else
                    {
                        Console.WriteLine("장비 아이템이 아닙니다.");
                    }
                }
                else
                {
                    haveItem[idx].IsEquiped = false;
                    StatIncrease(idx);
                }



            }
            private void StatIncrease(int idx)
            {
                if (haveItem[idx].IsEquiped)
                {
                    player.Atk += haveItem[idx].Atk;
                    player.Def += haveItem[idx].Def;
                    player.SkillAtk += haveItem[idx].SkillAtk;
                }

                if (!haveItem[idx].IsEquiped)
                {
                    player.Atk -= haveItem[idx].Atk;
                    player.Def -= haveItem[idx].Def;
                    player.SkillAtk -= haveItem[idx].SkillAtk;
                }
            }

            private void SellMenu()
            {

                Console.Clear();
                Console.WriteLine("■ 상 점 - 판매하기 ■");
                Console.WriteLine("어떤 아이템을 판매하시겠습니까?");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" ※상점에서 구매한 장비는 구매 시의 85% 가격으로 판매가능합니다.");
                Console.ResetColor();
                Console.WriteLine("");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine(player.Gold + "G");
                Console.WriteLine("");
                Console.WriteLine("[판매가능 아이템 목록]");
                if (haveItem.Count > 0)
                {
                    for (int i = 0; i < haveItem.Count; i++)
                    {
                        haveItem[i].SellItemList(true, i + 1);
                    }
                }
                else
                {
                    Console.WriteLine("--판매 가능한 아이템이 없습니다--");
                }
                Console.WriteLine("");
                Console.WriteLine("판매하고 싶은 아이템의 숫자를 입력해주세요");
                Console.WriteLine("0. 돌아가기");
                int keyInput = ChoiceInput(0, haveItem.Count);
                switch (keyInput)
                {
                    case 0:
                        Shop();
                        break;
                    default:
                        ItemSell(keyInput - 1);
                        SellMenu();
                        break;
                }

            }
            private void ItemSell(int Idx)
            {
                if (haveItem[Idx].Quantity > 1)
                {
                    Console.WriteLine("몇 개를 판매하시겠습니까?");
                    Console.WriteLine("0. 취소");
                    int keyInput = ChoiceInput(0, haveItem[Idx].Quantity);
                    switch (keyInput)
                    {
                        case 0:
                            SellMenu();
                            break;
                        default:
                            {
                                if (haveItem[Idx].IsEquiped && keyInput >= haveItem[Idx].Quantity)
                                {
                                    Console.WriteLine("장비중인 아이템은 팔 수 없습니다.");
                                    Console.ReadKey();
                                }
                                else
                                {
                                   
                                    haveItem[Idx].IsPurchased = false;
                                    player.Gold += (int)(haveItem[Idx].Gold * 0.85f) * keyInput;
                                    haveItem[Idx].Quantity -= keyInput;
                                    if (haveItem[Idx].Quantity <= 0)
                                    {
                                        haveItem.Remove(haveItem[Idx]);
                                        Item.itemCount--;
                                    }
                                }
                                break;
                            }
                    }
                }
                else
                {
                    if (haveItem[Idx].IsEquiped)
                    {
                        Console.WriteLine("장비중인 아이템은 팔 수 없습니다.");
                        Console.ReadKey();

                    }
                    else
                    {

                       
                        haveItem[Idx].IsPurchased = false;
                        player.Gold += (int)(haveItem[Idx].Gold * 0.85f);
                        haveItem.Remove(haveItem[Idx]);
                        Item.itemCount--;
                    }
                }

            }



            public void BattleStart() // 2. 전투 시작
            {
                int diff = dungeonFloor * 2;  // 난이도 보정


                List<Monster> monsters = new List<Monster>(); // 몬스터 리스트 monsters 생성
                Random r = new Random();
                int beforeHp = player.Hp;
                int beforeMp = player.Mp;
                int beforeExp = player.Exp;


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
                        if (monsterHp[i] <= 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($"   Lv.{monsters[i].Level} {monsters[i].Name} Dead");
                            Console.ResetColor();
                        }
                        else if (0 < monsterHp[i] && 0 < monsterBurn[i])
                        {
                            Console.Write($"   Lv.{monsters[i].Level} {monsters[i].Name} HP {monsterHp[i]}/{monsters[i].Hp}");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" ♨");
                            Console.ResetColor();
                        }
                        else if (0 < monsterHp[i])
                            Console.WriteLine($"   Lv.{monsters[i].Level} {monsters[i].Name} HP {monsterHp[i]}/{monsters[i].Hp}");

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
                                        WarriorSkillSceneOne();
                                        monsterHp[atk - 1] -= (int)Math.Ceiling(player.Atk * 3.2f); //  알파 스트라이크 데미지
                                        BattleField();
                                        Console.WriteLine($"\n\n{player.Name} 의 알파 스트라이크!\n");
                                        Console.WriteLine($"Lv.{monsters[atk - 1].Level} {monsters[atk - 1].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[atk - 1]}]- 치명타!!");
                                    }
                                    else
                                    {
                                        WarriorSkillSceneOne();
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
                                    WarriorSkillSceneTwo();
                                    BattleField();
                                    Console.SetCursorPosition(0, 3 + atk1);
                                    Console.WriteLine($"◈");
                                    Console.SetCursorPosition(0, 3 + atk2);
                                    Console.WriteLine($"◈");
                                    Console.SetCursorPosition(0, 11 + monsters.Count);
                                    Console.WriteLine($"\n\n{player.Name} 의 더블 스트라이크!\n");
                                    Console.WriteLine($"Lv.{monsters[atk1].Level} {monsters[atk1].Name} 을(를) 맞췄습니다. [데미지 : {hp1 - monsterHp[atk1]}]- 치명타!!");
                                    Console.WriteLine($"Lv.{monsters[atk2].Level} {monsters[atk2].Name} 을(를) 맞췄습니다. [데미지 : {hp2 - monsterHp[atk2]}]- 치명타!!");
                                }
                                else
                                {
                                    WarriorSkillSceneTwo();
                                    monsterHp[atk1] -= (int)Math.Ceiling(player.Atk * 1.5);
                                    monsterHp[atk2] -= (int)Math.Ceiling(player.Atk * 1.5);
                                    BattleField();
                                    Console.SetCursorPosition(0, 3 + atk1);
                                    Console.WriteLine($"◈");
                                    Console.SetCursorPosition(0, 3 + atk2);
                                    Console.WriteLine($"◈");
                                    Console.SetCursorPosition(0, 11 + monsters.Count);
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
                                    Console.SetCursorPosition(0, 3 + atk);
                                    Console.WriteLine($"◈");
                                    Console.SetCursorPosition(0, 11 + monsters.Count);
                                    Console.WriteLine($"\n\n{player.Name} 의 더블 스트라이크!\n");
                                    Console.WriteLine($"Lv.{monsters[atk].Level} {monsters[atk].Name} 을(를) 맞췄습니다. [데미지 : {bh - monsterHp[atk]}]- 치명타!!");
                                }
                                else
                                {
                                    monsterHp[atk] -= (int)Math.Ceiling(player.Atk * 1.5);
                                    BattleField();
                                    Console.SetCursorPosition(0, 3 + atk);
                                    Console.WriteLine($"◈");
                                    Console.SetCursorPosition(0, 11 + monsters.Count);
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
                            WizardSkillSceneOne();
                            for (int i = monsters.Count - 1; 0 <= i; i--)

                            {
                                int bh = monsterHp[i];
                                if (0 < monsterHp[i])
                                {

                                    monsterHp[i] -= (int)Math.Ceiling(player.SkillAtk * 0.4f);
                                    monsterBurn[i] = 4;
                                    BattleField();
                                    Console.SetCursorPosition(0, 3 + i);
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"◎▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒·");
                                    Console.ResetColor();
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
                            WizardSkillSceneTwo();
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


                                Console.SetCursorPosition(0, 3 + atk);
                                if (monsterHp[atk] < 0) Console.SetCursorPosition(0, 3 + atk - next);
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write($"◎");
                                Console.CursorLeft = 12;
                                Console.WriteLine("▒▲▒");
                                Console.CursorLeft = 12;
                                Console.WriteLine("▒§▒");
                                Console.CursorLeft = 12;
                                Console.WriteLine("▒§▒");
                                Console.ResetColor();
                                Console.SetCursorPosition(0, 11 + monsters.Count);
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
                                Console.SetCursorPosition(0, 3 + atk);
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write($"◎");
                                Console.CursorLeft = 12;
                                Console.WriteLine("▒▲▒");
                                Console.CursorLeft = 12;
                                Console.WriteLine("▒§▒");
                                Console.CursorLeft = 12;
                                Console.WriteLine("▒§▒");
                                Console.ResetColor();
                                Console.SetCursorPosition(0, 11 + monsters.Count);
                                Console.WriteLine($"\n\n{player.Name} 의 아이스 스피어!\n");
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
                    if (player is Wizard && use == 3 && player.AvailableSkill[use - 1]) // 마법사 3번 스킬 // 메디테이션 - MP +10, 일시적으로 마법력 * 0.5 의 방어력을 얻고 명상에 빠진다.
                    {
                        WizardSkillSceneOne();
                        int bd = player.Def;
                        player.Def += (int)Math.Ceiling(player.SkillAtk * 0.5);
                        BattleField();
                        Console.WriteLine($"\n{player.Name}의 방어력이 {player.Def - bd} 증가하였습니다. [방어력 : {player.Def}]\n\nEnter. 다음");
                        Console.ReadLine();
                        EnemyFrontPhase();
                        player.Def = bd;
                        player.Mp += 10;
                        if (player.MaxMp < player.Mp) player.Mp = player.MaxMp;
                    }
                    else if (player is Wizard && use == 3)
                    {
                        Console.WriteLine("사용할 수 없는 스킬입니다.\n\nEnter. 다음");
                        Console.ReadLine();
                    }
                    //---------------- 음악가 스킬 -----------------------------------------------------------------------------------------------------------------------------------------------
                    if (player is Musician && use == 1 && player.AvailableSkill[use - 1]) // 음악가 1번 스킬 - 타임 코스모스, 그냥 냅다 게임 시작 페이지로 돌아갑니다.
                    {
                        if (50 <= player.Mp)
                        {
                            player.Mp -= 50;
                            MusicionSkillScene();
                            PlayGame(); // 다시 게임 시작
                            if (CheckMonsters() != 0) EnemyFrontPhase();
                        }
                        else
                        {
                            Console.WriteLine("MP 가 부족합니다.\n\nEnter. 다음");
                            Console.ReadLine();
                        }
                    }
                    else if (player is Musician && use == 1)
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


                            Console.WriteLine($"▷");

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
                        BattleField();
                        int befHp = player.Hp;
                        int damage = monsters[i].Atk - player.Def;    // 몬스터 데미지
                        if (damage < 1) damage = 1;
                        if (0 < monsterHp[i])
                        {
                            Console.SetCursorPosition(0, 3 + i);
                            Console.WriteLine($"▶");
                            Console.SetCursorPosition(0, 11 + monsters.Count);
                            Console.WriteLine($"Lv.{monsters[i].Level} {monsters[i].Name} 의 공격!\n");
                            Console.WriteLine("1. 방어하기\n2. 피하기");
                            switch (ChoiceInput(1, 2))
                            {
                                case 1:
                                    player.Hp -= damage;
                                    BattleField();
                                    Console.WriteLine($"\n{player.Name} 을(를) 맞췄습니다. [데미지 : {befHp - player.Hp}]\n");
                                    break;
                                case 2:
                                    int Dodge = r.Next(0, player.Dex + monsters[i].Dex);
                                    if (Dodge < player.Dex)
                                    {
                                        BattleField();
                                        Console.WriteLine($"\n{player.Name} 이(가) 공격을 피했습니다!");
                                        break;
                                    }
                                    else
                                    {
                                        player.Hp -= monsters[i].Atk;
                                        BattleField();
                                        Console.WriteLine($"\n{player.Name} 을(를) 맞췄습니다. [데미지 : {befHp - player.Hp}]\n");
                                        break;
                                    }
                            }
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
                    int diff = dungeonFloor * 2;                                // 난이도 보정
                    Console.Write((dungeonFloor + 1) + "층");
                    for (int i = 0; i < dungeon.Count; i++)                    // 승리시 층수와 함께 몹 능력치 증가
                    {
                        Monster newLv = dungeon[i];
                        newLv.Level += diff;
                        dungeon[i] = newLv;

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
                    Console.ReadKey();
                    LevelUp();
                    GetRewards();

                    if (dungeonFloor % 3 == 0)                                          // 3층마다 이한솔매니저님 몹 추가
                    {
                        dungeon.Add(Hansole);
                    }
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
                    Random r = new Random();
                    int totalGold = 0;

                    dropItem.Clear();

                    for (int i = 0; i < monsters.Count; i++)
                    {
                        totalGold += monsters[i].Gold;
                        if (items.Count > 0)
                        {
                            int itemIdx = r.Next(items.Count); // 아이템 리스트 내에서 랜덤한 인덱스 선택
                            Item newItem = items[itemIdx];

                            var existingDropItem = dropItem.FirstOrDefault(item => item.Name == newItem.Name);
                            if (existingDropItem != null)
                            {
                                existingDropItem.Quantity++; // dropItem 리스트에 이미 존재하는 경우 수량 증가
                            }
                            else
                            {
                                dropItem.Add(new Item(newItem.Name, newItem.Type, newItem.Atk, newItem.SkillAtk, newItem.Def, newItem.Gold, 1, false)); // 새 아이템 추가
                            }

                            var existingHaveItem = haveItem.FirstOrDefault(item => item.Name == newItem.Name);
                            if (existingHaveItem != null)
                            {
                                existingHaveItem.Quantity++; // haveItem 리스트에 이미 존재하는 경우 수량 증가
                            }
                            else
                            {
                                haveItem.Add(new Item(newItem.Name, newItem.Type, newItem.Atk, newItem.SkillAtk, newItem.Def, newItem.Gold, 1, false)); // 새 아이템 추가
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

                    foreach (var item in dropItem) // 
                    {
                        Console.WriteLine("");
                        Console.Write($"{item.Name} - "); // 아이템 이름
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(item.Quantity); // dropItem 수
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
                            Console.Write("\n마나를 ");
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
            void SaveGameMenu()
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("[ 게임 저장 / 불러오기 ]\n");
                Console.ResetColor();
                Console.WriteLine("게임을 저장하거나 불러올 수 있습니다.\n");
                Console.WriteLine("1. 게임 저장하기");
                Console.WriteLine("2. 게임 불러오기");
                Console.WriteLine("0. 돌아가기\n");

                switch (ChoiceInput(0, 2))
                {
                    case 0:
                        break;
                    case 1:
                        SaveGame("gameData.json");
                        Console.WriteLine("게임이 저장되었습니다! 아무 키나 누르면 돌아갑니다.");
                        Console.ReadKey();
                        break;
                    case 2:
                        LoadGame("gameData.json");
                        Console.WriteLine("게임을 불러왔습니다! 아무 키나 누르면 돌아갑니다.");
                        Console.ReadKey();
                        break;
                }
            }
            void SaveGame(string filePath) // 게임 저장
            {
                var gameData = new // 저장용 객체
                {
                    Player = player,
                    HaveItem = haveItem.ToList(), // 내가 가지고 있는 아이템 리스트화
                    DungeonFloor = dungeonFloor,
                    Dungeon = dungeon
                };
                string json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            public void LoadGame(string filePath) // 게임 불러오기
            {
                string json = File.ReadAllText(filePath);

                var gameData = JsonConvert.DeserializeObject<dynamic>(json);

                var haveItemArray = gameData.HaveItem;

                var classData = gameData.Player;
                switch (classData.Class.ToString()) // 직업 Class 문자열 참조해서 플레이어 캐릭터 스탯 로드
                {
                    case "전사":
                        player = classData.ToObject<Warrior>();
                        break;
                    case "마법사":
                        player = classData.ToObject<Wizard>();
                        break;
                    case "음악가":
                        player = classData.ToObject<Musician>();
                        break;
                }
                haveItem = haveItemArray.ToObject<List<Item>>();
                Item.itemCount = haveItem.Count;

                dungeonFloor = gameData.DungeonFloor;
                Monster[] dungeonArray = gameData.Dungeon.ToObject<Monster[]>();
                dungeon = new List<Monster>(dungeonArray);
            }
        }
        private static void ShowHighlightText(string text) // 첫 줄 색 변경 함수, 녹색
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void PrintStartLogo() // 게임 스타트 로고
        {
            Console.Clear();
            ShowHighlightText("=====================================================================================================================");
            Console.WriteLine("   ");
            Console.WriteLine("                         ███████╗     ██████╗ ██████╗  ██████╗ ██╗   ██╗██████╗ ███████");
            Console.WriteLine("                         ╚════██║    ██╔════╝ ██╔══██╗██╔═══██╗██║   ██║██╔══██╗██╔════╝  ");
            Console.WriteLine("                             ██╔╝    ██║  ███╗██████╔╝██║   ██║██║   ██║██████╔╝███████╗ ");
            Console.WriteLine("                            ██╔╝     ██║   ██║██╔══██╗██║   ██║██║   ██║██╔═══╝ ╚════██║");
            Console.WriteLine("                            ██║      ╚██████╔╝██║  ██║╚██████╔╝╚██████╔╝██║     ███████║  ");
            Console.WriteLine("                            ╚═╝       ╚═════╝ ╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═╝     ╚══════╝    ");
            Console.WriteLine("                                     ██████╗ ██████╗ ███████╗ █████╗ ████████╗  ");
            Console.WriteLine("                                    ██╔════╝ ██╔══██╗██╔════╝██╔══██╗╚══██╔══╝");
            Console.WriteLine("                                    ██║  ███╗██████╔╝█████╗  ███████║   ██║  ");
            Console.WriteLine("                                    ██║   ██║██╔══██╗██╔══╝  ██╔══██║   ██║   ");
            Console.WriteLine("                                    ╚██████╔╝██║  ██║███████╗██║  ██║   ██║");
            Console.WriteLine("                                     ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝   ╚═╝ ");
            Console.WriteLine("                       █████╗ ██████╗ ██╗   ██╗███████╗███╗   ██╗████████╗██╗   ██╗██████╗ ███████╗");
            Console.WriteLine("                      ██╔══██╗██╔══██╗██║   ██║██╔════╝████╗  ██║╚══██╔══╝██║   ██║██╔══██╗██╔════╝");
            Console.WriteLine("                      ███████║██║  ██║██║   ██║█████╗  ██╔██╗ ██║   ██║   ██║   ██║██████╔╝█████╗ ");
            Console.WriteLine("                      ██╔══██║██║  ██║╚██╗ ██╔╝██╔══╝  ██║╚██╗██║   ██║   ██║   ██║██╔══██╗██╔══╝  ");
            Console.WriteLine("                      ██║  ██║██████╔╝ ╚████╔╝ ███████╗██║ ╚████║   ██║   ╚██████╔╝██║  ██║███████╗");
            Console.WriteLine("                      ╚═╝  ╚═╝╚═════╝   ╚═══╝  ╚══════╝╚═╝  ╚═══╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝╚══════╝");
            Console.WriteLine();
            ShowHighlightText("==================================================================================================================");
            Console.WriteLine();
            Console.WriteLine("                                          PRESS ANY KEY TO CONTINUE . . .                              ");
            Console.WriteLine();
            ShowHighlightText("==================================================================================================================");
            Console.ReadKey();
        }
        
        private static void WarriorSkillSceneOne() // 전사 스킬씬1, 알파 스트라이크
        {
            Console.Clear();
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("                                                              Thank you, Michael ! ");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠐⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⣾⣿⣿⣷⣦⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⡀⠀⠀⠀⠀⠀⠀⢠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⣿⣿⣿⣿⣿⣿⣿⣷⠀⣠⣴⣶⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⣿⣦⡀⠀⠀⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡆");
            Console.WriteLine("⠈⠈⠙⢦⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀");
            Console.WriteLine("⠀⠖⠀⠀⠑⢷⡤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠤⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⠛⠻⠿⣿⣿⣿⣇⢤");
            Console.WriteLine("⠈⠀⠀⠀⠀⠀⠈⠪⣓⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⠏⠀⠀⠀⠀⢀⠀⠙⢿⡏⠀⡇⠀⡠⠴⠒⠒⠒⠤⡀⠀");
            Console.WriteLine("⠀⢠⠀⠀⠀⠀⠀⠀⠈⠒⣌⠢⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⡿⠋⠁⠀⠀⠀⢀⡐⠁⠀⠀⠼⠀⢠⣷⣋⠤⠒⠂⠀⠀⠀⠹");
            Console.WriteLine("⠀⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⢦⣉⠒⢤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣋⡾⠑⣢⣤⠄⠒⡾⠟⠀⠀⠀⠀⢔⣪⠋⠀⠀⠀⠀⠀⠀⠀⠀⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠳⣤⠈⠑⢄⡀⠀⠀⠀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣿⣿⣿⡟⢝⠀⢻⠀⠁⠚⣦⣀⡠⠀⠀⠀⠀⡸⠀⢯⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠦⣄⠈⠑⠢⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⠟⠀⠸⡀⠈⡄⠀⠀⠪⠚⢟⢉⣿⠄⢠⢹⠀⢸⠀⢀⠀⠀⠀⠀⠀⠀⠀⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠓⠤⡀⣆⠈⠒⠤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠏⠀⠀⠀⢡⠀⠈⠒⠒⠢⠤⣠⢛⣁⠀⡎⡜⢀⡀⡇⢸⠀⠀⠀⠀⠀⠀⢰");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠛⠤⡀⠀⠈⠉⠢⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢇⠀⠀⠀⠀⠀⡞⡟⢆⠈⢀⢃⠎⢸⡇⡼⠀⠀⠀⠀⠀⠀⡸⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠒⠤⣀⠀⠀⠈⠑⠢⢄⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢆⠀⠀⠀⠀⠷⠁⠀⠓⢸⠎⠀⢸⡷⣹⠀⠀⠀⠀⠀⢠⠃⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠑⠢⢄⠀⠀⠀⠈⠉⠲⠤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢢⠀⠈⡆⠀⠀⠀⠀⠀⠀⠀⡰⢡⠃⠀⠀⠀⠀⣳⡎⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠒⠤⣀⠀⠀⠀⠀⠉⠒⠢⢄⡀⠀⠀⠀⠀⠀⠳⡀⠘⡄⠀⠀⠀⠀⢀⡜⠁⠁⠀⠀⠀⠀⢠⠿⢀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠒⠢⢄⡀⠀⠀⠀⠀⠉⠑⠲⠤⣀⡐⢾⢆⢱⡀⠀⠀⢀⠞⠀⠀⠀⠀⠀⠀⣴⠋⠀⠈⡆");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠂⠤⣄⡀⠀⠀⢀⡎⢉⠙⣦⠋⠁⠉⢒⠁⠀⠀⠀⠀⠀⢠⠞⠁⢀⡄⠀⡇⠀⠀⠀⢠⠏⢦⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠑⠒⢼⣀⡇⢀⡇⠀⠀⣄⠀⡇⠀⠀⠀⠀⢠⠚⠒⠊⠁⠀⠀⡞⠀⠀⠀⢸⠀⠈⠆");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠤⠯⣹⢈⣦⡔⣈⢢⣸⣄⡶⠒⠒⠋⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⡆⠀⠀⠸⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠁⠀⠑⠹⠤⢃⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠁⢀⠤⡀⡇⠀⠀⠀⢣⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣯⠋⠀⠀⠀⠀⠀⠀⠀⠀⢀⡎⢀⠃⠀⠹⡡⠀⠀⠀⠸⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠃⠀⠀⠀⠀⠀⠀⠀⠀⢀⠏⡇⡞⠀⠀⠀⢣⣎⠀⠀⠀⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠃⠀⠀⠀⠀⠀⠀⠀⠀⢀⠏⡇⡞⠀⠀⠀⢣⣎⠀⠀⠀⡇⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠃⠀⠀⠀⠀⠀⠀⠀⠀⢀⠎⠀⠻⠀⠀⠀⠀⡎⠸⠀⠀⠀⢰");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠋⠀⠀⠀⠀⠀⠀⠀⠀⢀⠎⠀⠀⠀⠀⠀⠀⡜⠀⠀⢣⠀⠀⢸⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠆⠀⠀⠀⠀⠀⠀⠀⠀⢠⠎⠀⠀⠀⠀⠀⠀⡸⠀⠀⠀⠘⡄⠀⠀⡇⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠎⠀⠀⠀⠀⠀⠀⠀⠀⢀⠻⡄⠀⠀⠀⠀⠀⡜⠀⠀⠀⠀⠀⠱⢄⡰⠃⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡎⠀⠀⠀⠀⠀⠀⠀⠀⢠⠏⠀⢱⠀⠀⠀⢀⠞⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡞⠀⠀⠀⠀⠀⠀⠀⠀⢠⠂⠀⠀⠀⢣⣀⠴⠁⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡼⠀⠀⠀⠀⠀⠀⠀⠀⡠⠃⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⠁⠀⠀⠀⠀⠀⠀⠀⡰⠁⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣀⠀⠀⠀⠀⠀⠀⠀⢠⠃⠀⠀⠀⠀⠀⠀⠀⡴⠁⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡎⠀⠀⠉⠒⠤⡀⠀⠀⠀⠎⠀⠀⠀⠀⠀⠀⢀⠜⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢣⠀⠀⠀⠀⠀⠈⠑⢤⡸⠀⠀⠀⠀⠀⠀⢀⠎⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠣⡀⠀⠀⠀⠀⠀⠀⠯⣢⠄⠀⠀⠀⢠⠃⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢦⡀⠀⠀⠀⠀⠀⠀⠉⠉⠒⡴⠁⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠢⡀⠀⠀⠀⠀⠀⠀⠘⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠑⠤⣀⡀⠀⠀⠀");
            ShowHighlightText("===============================================================================================================");
            Console.WriteLine("");
            Console.WriteLine("                                \"기나긴 모멸과 핍박의 시간이었다..\"");
            Console.WriteLine("");
            ShowHighlightText("                                   Warrior의 알파 스트라이크 !! ");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("                                                                                          Press Any Key >> ");
            ShowHighlightText("===============================================================================================================");
            Console.ReadLine();
        }

        private static void WarriorSkillSceneTwo() // 전사 스킬씬1, 더블 스트라이크
        {
            Console.Clear();
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("                                                              고맙네 마이콜! ");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠐⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⣾⣿⣿⣷⣦⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⡀⠀⠀⠀⠀⠀⠀⢠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⣿⣿⣿⣿⣿⣿⣿⣷⠀⣠⣴⣶⣤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⣿⣦⡀⠀⠀⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡆");
            Console.WriteLine("⠈⠈⠙⢦⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀");
            Console.WriteLine("⠀⠖⠀⠀⠑⢷⡤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠤⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⠛⠻⠿⣿⣿⣿⣇⢤");
            Console.WriteLine("⠈⠀⠀⠀⠀⠀⠈⠪⣓⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⠏⠀⠀⠀⠀⢀⠀⠙⢿⡏⠀⡇⠀⡠⠴⠒⠒⠒⠤⡀⠀");
            Console.WriteLine("⠀⢠⠀⠀⠀⠀⠀⠀⠈⠒⣌⠢⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⡿⠋⠁⠀⠀⠀⢀⡐⠁⠀⠀⠼⠀⢠⣷⣋⠤⠒⠂⠀⠀⠀⠹");
            Console.WriteLine("⠀⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⢦⣉⠒⢤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣋⡾⠑⣢⣤⠄⠒⡾⠟⠀⠀⠀⠀⢔⣪⠋⠀⠀⠀⠀⠀⠀⠀⠀⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠳⣤⠈⠑⢄⡀⠀⠀⠀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣿⣿⣿⡟⢝⠀⢻⠀⠁⠚⣦⣀⡠⠀⠀⠀⠀⡸⠀⢯⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠦⣄⠈⠑⠢⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⠟⠀⠸⡀⠈⡄⠀⠀⠪⠚⢟⢉⣿⠄⢠⢹⠀⢸⠀⢀⠀⠀⠀⠀⠀⠀⠀⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠓⠤⡀⣆⠈⠒⠤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠏⠀⠀⠀⢡⠀⠈⠒⠒⠢⠤⣠⢛⣁⠀⡎⡜⢀⡀⡇⢸⠀⠀⠀⠀⠀⠀⢰");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠛⠤⡀⠀⠈⠉⠢⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢇⠀⠀⠀⠀⠀⡞⡟⢆⠈⢀⢃⠎⢸⡇⡼⠀⠀⠀⠀⠀⠀⡸⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠒⠤⣀⠀⠀⠈⠑⠢⢄⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢆⠀⠀⠀⠀⠷⠁⠀⠓⢸⠎⠀⢸⡷⣹⠀⠀⠀⠀⠀⢠⠃⠀");
            Console.WriteLine("   ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠑⠢⢄⠀⠀⠀⠈⠉⠲⠤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢢⠀⠈⡆⠀⠀⠀⠀⠀⠀⠀⡰⢡⠃⠀⠀⠀⠀⣳⡎⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠒⠤⣀⠀⠀⠀⠀⠉⠒⠢⢄⡀⠀⠀⠀⠀⠀⠳⡀⠘⡄⠀⠀⠀⠀⢀⡜⠁⠁⠀⠀⠀⠀⢠⠿⢀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠒⠢⢄⡀⠀⠀⠀⠀⠉⠑⠲⠤⣀⡐⢾⢆⢱⡀⠀⠀⢀⠞⠀⠀⠀⠀⠀⠀⣴⠋⠀⠈⡆");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠂⠤⣄⡀⠀⠀⢀⡎⢉⠙⣦⠋⠁⠉⢒⠁⠀⠀⠀⠀⠀⢠⠞⠁⢀⡄⠀⡇⠀⠀⠀⢠⠏⢦⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠑⠒⢼⣀⡇⢀⡇⠀⠀⣄⠀⡇⠀⠀⠀⠀⢠⠚⠒⠊⠁⠀⠀⡞⠀⠀⠀⢸⠀⠈⠆");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠤⠯⣹⢈⣦⡔⣈⢢⣸⣄⡶⠒⠒⠋⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⡆⠀⠀⠸⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠁⠀⠑⠹⠤⢃⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠁⢀⠤⡀⡇⠀⠀⠀⢣⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣯⠋⠀⠀⠀⠀⠀⠀⠀⠀⢀⡎⢀⠃⠀⠹⡡⠀⠀⠀⠸⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠃⠀⠀⠀⠀⠀⠀⠀⠀⢀⠏⡇⡞⠀⠀⠀⢣⣎⠀⠀⠀⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠃⠀⠀⠀⠀⠀⠀⠀⠀⢀⠏⡇⡞⠀⠀⠀⢣⣎⠀⠀⠀⡇⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠃⠀⠀⠀⠀⠀⠀⠀⠀⢀⠎⠀⠻⠀⠀⠀⠀⡎⠸⠀⠀⠀⢰");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠋⠀⠀⠀⠀⠀⠀⠀⠀⢀⠎⠀⠀⠀⠀⠀⠀⡜⠀⠀⢣⠀⠀⢸⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠆⠀⠀⠀⠀⠀⠀⠀⠀⢠⠎⠀⠀⠀⠀⠀⠀⡸⠀⠀⠀⠘⡄⠀⠀⡇⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠎⠀⠀⠀⠀⠀⠀⠀⠀⢀⠻⡄⠀⠀⠀⠀⠀⡜⠀⠀⠀⠀⠀⠱⢄⡰⠃⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡎⠀⠀⠀⠀⠀⠀⠀⠀⢠⠏⠀⢱⠀⠀⠀⢀⠞⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡞⠀⠀⠀⠀⠀⠀⠀⠀⢠⠂⠀⠀⠀⢣⣀⠴⠁⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡼⠀⠀⠀⠀⠀⠀⠀⠀⡠⠃⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⠁⠀⠀⠀⠀⠀⠀⠀⡰⠁⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣀⠀⠀⠀⠀⠀⠀⠀⢠⠃⠀⠀⠀⠀⠀⠀⠀⡴⠁⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡎⠀⠀⠉⠒⠤⡀⠀⠀⠀⠎⠀⠀⠀⠀⠀⠀⢀⠜⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢣⠀⠀⠀⠀⠀⠈⠑⢤⡸⠀⠀⠀⠀⠀⠀⢀⠎⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠣⡀⠀⠀⠀⠀⠀⠀⠯⣢⠄⠀⠀⠀⢠⠃⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢦⡀⠀⠀⠀⠀⠀⠀⠉⠉⠒⡴⠁⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠢⡀⠀⠀⠀⠀⠀⠀⠘⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠑⠤⣀⡀⠀⠀⠀");
            ShowHighlightText("===============================================================================================================");
            Console.WriteLine("");
            Console.WriteLine("                                  \"당장 내 집에서 나가!\"                                             ");
            Console.WriteLine("");
            ShowHighlightText("                                   Warrior의 더블 스트라이크 !!! ");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("                                                                                          Press Any Key >> ");
            ShowHighlightText("===============================================================================================================");
            Console.ReadLine();
        }


        private static void WizardSkillSceneOne() // 법사 스킬씬 1,3   파이어랑 3번째
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠚⢁⠤⠒⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⠢⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠄⠀⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⢀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠎⠀⠀⠀⠀⠀⠀⠀   초능력 맛 좀 볼래?⠀⠀⠀⠀⠀⠀⠀              ⠀⠈⡄");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀                        ⠀⠀⠀       ⢠⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠁⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠢⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠊⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⠂⠤⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣀⡠⠤⠔⠂⠁⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠀⠐⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⢀⠂⠈⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine();
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡠⢿⡏⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠓⠦⣀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡴⠋⠀⠘⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠑⢄⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡰⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⣄");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⡄⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⡀⠀⠀⢀⠔⠉⠑⠦⣤⡀⠀⠀⠀⠀⠀⡀⢀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢣⡀⠀⢸⠀⠀⡀⠀⣿⣯⢠⡀⢠⣾⠞⢡⣾⣿⣯⠳⡀⠀⠀⠀⠀⠀⢠⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠈⠦⡀⢳⣾⣿⣿⠀⡟⢻⡏⢠⣏⣼⣿⣿⢀⡽⠀⠀⠀⠀⢠⠇⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠖⠁⠀⠀⠀⠈⠓⠿⢷⡏⠤⠄⣘⣁⠸⣿⠿⠟⠓⠉⠀⠀⠀⠀⠐⡎⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠎⠀⠀⠀⠀⠀⠀⠀⠀⡠⠋⠀⠀⠀⠀⠈⠙⢆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⢦⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀ ⠀⠀⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⠈⠂⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠱⡀⠀⠀⠀⠀⠀⠀⡼⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠃");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠱⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠢⠀⠀⠤⠔⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠎");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢆⠀⠀⠀⠘⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠦⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠋⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠵⠄⠀⠀⢇⢀⠔⠀⠀⠉⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⠉⠉⠐⠒⠢⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⡤⠔⠊⠉⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⠀⠀⠀⠀⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡟⠑⠦⢀⣠⣀⣠⡤⠴⠚⠉⠉⠀⠑⢄⡀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠐⡀⠀⠀⢰⠀⠀⠀⠀⠀⣠⠐⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡠⠒⠋⡇⠀⠀⡜⠀⢠⠇⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠒⠤⣀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠈⠂⢦⠘⠀⠀⠀⢀⢠⠋⠑⠦⡀⠀⠀⠀⠀⠀⠀⠀⢀⠴⠋⠀⠀⠀⠘⢄⣀⢀⡠⠎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠒⠤⣄⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⠀⠂⡤⠒⠓⠒⢄⠀⠀⡝⠁⠀⠀⠀⠈⠲⣀⠀⢀⣀⠔⠊⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⣄⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⢄⠀⠀⠮⠤⠚⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠢⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⠀⠱⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠳⡀⠀");
            ShowHighlightText("===============================================================================================================");
            Console.WriteLine("");
            Console.WriteLine("                                     \"처신 잘하라고.\"                                             ");
            Console.WriteLine("");
            ShowHighlightText("                                   Wizard가 초능력을 사용합니다! ");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("                                                                                          Press Any Key >> ");
            ShowHighlightText("===============================================================================================================");
            Console.ReadLine();
        }


        private static void WizardSkillSceneTwo() // 법사 스킬씬2 , 아이스 애로우
        {
            Console.Clear();
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣀⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠤⠀⠒⠉⠁⠀⠀⠀⠈⠀⠑⠂⢄⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠔⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠡⡀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⢀⠊⠀⠀⠀⠀⠀⠀호잇!⠀⠀⠀⠀⠀⠁⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠘⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠆⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠤⣌⡂⠀⠄⠀⠀⠐⡆⡤⠤⠄⠀⠠⠤⠠⠒⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⡠⠤⠞⠀⠀⠀⠈⢦⣀⠀⠀⠀⣇⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⢸⡇⠀⠀⠀⠀⠀⠀⠐⠋⠉⢳⡀⠛⣠⠤⠖⠒⠒⠒⠒⠦⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⢀⡷⢀⣶⣠⢄⣀⡖⢦⣈⣷⡈⢳⡎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⢤⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠘⣇⢨⠏⢀⣤⡄⠀⣠⣠⠀⡇⡟⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⢠⡎⢈⡇⠈⠉⣱⣶⣿⠀⠀⠻⡇⠀⠀⠀⠀⡠⢶⣤⠀⠀⠀⣤⣦⠸⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⢧⣾⠀⠀⠤⢬⢽⡭⠄⠀⠀⣿⠀⠀⠀⠀⣇⣿⣾⡇⣀⣄⣿⣿⣸⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠛⢦⡀⠀⠈⠙⠃⠀⣀⠜⠈⢦⠀⠀⠀⠉⠉⠀⠱⡇⠈⢹⠀⠈⠑⣆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢉⡍⠉⠩⣯⣀⠀⠀⠘⣆⠀⠀⠀⠀⠀⠀⠉⠉⠉⠀⠀⠀⣸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⣾⣿⣿⣿⣿⣿⣿⣿⣦⡀⠈⠲⣤⠤⢤⣄⡀⠀⠀⠀⢀⣠⠞⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⢠⠎⣴⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡄⠀⡸⠀⠀⢀⠼⡍⢹⢚⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⡰⠁⣸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⠞⠁⠀⡴⠃⠀⠐⠀⣾⠗⠒⠤⠤⠤⢤⢀⡀⠀⠀⠀⠔⠳⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⡴⠁⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠃⠀⠀⣸⠁⠀⠀⠀⠀⠹⡄⠀⠀⠀⠀⠀⠀⠉⠒⢤⡴⠁⣜⣀⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⢀⠁⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠃⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⢹⢀⡤⠤⠤⢄⣀⣀⠀⢸⠀⢀⠠⢤⠤⠸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⡘⠀⡸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⠀⠀⢰⡾⡅⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠈⠉⠉⠳⣮⠶⠞⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⢠⠃⠸⡀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⠀⠀⡿⠀⡇⠀⠀⠀⠀⠀⠀⠀⠈⠢⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠘⠐⠐⠁⣿⣿⣿⣿⣿⡛⠿⣿⣿⣿⣿⡇⠀⠀⣷⢰⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠹⣿⣿⣿⣿⣷⡀⢈⠙⠛⠿⠿⠶⠶⠾⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡄⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⡇⠉⠛⢿⠿⠛⠫⣆⡀⠀⠀⠀⠀⠀⠘⣤⡀⠀⠀⠀⠀⠀⣀⡀⠴⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("\r\n⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⢸⠀⠀⠀⢸⠑⠲⢴⣀⡄⠀⠀⠉⡟⠉⢳⠒⠒⠋⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("\r\n⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⢸⠀⠀⠀⢸⠀⠀⠀⠏⡇⠀⠀⠀⡇⠀⢸⠀⠀⢀⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀ ⢀⠀⠀⢸⠀⠀⠀⢸⠀⠀⢸⠀⡇⠀⠀⢀⠇⠀⢸⠀⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⡐⠊⠉⠀⠀⡸⠀⠀⠀⡆⠀⠀⠈⢺⡇⠀⠀⣸⣀⠀⢸⠀⠀⣮⣀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠁⠒⠒⠒⠊⠁⠀⠀⠀⠉⠒⠒⠒⠚⢧⡀⠀⠉⣉⡇⢸⡀⠀⠀⢈⡇⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠊⠉⠀⠀⠙⠒⠊⠉⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("");
            Console.WriteLine("");
            ShowHighlightText("===============================================================================================================");
            Console.WriteLine("");
            Console.WriteLine("                                            \" 호잇 !\"                                             ");
            Console.WriteLine("");
            ShowHighlightText("                                   Wizard가 초능력을 사용합니다!! ");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("                                                                                          Press Any Key >> ");
            ShowHighlightText("===============================================================================================================");
            Console.ReadLine();
        }

        private static void MusicionSkillScene() // 음악가 스킬 씬
        {
            Console.Clear();
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⡀⠤⠤⠤⠤⠤⠤⠤⢀⣀⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡀⠤⠒⠂⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠁⠐⠒⠠⠤⢀⡀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⠀⠀⢀⡠⠔⠊⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠒⠤⢄");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⡔⠉⠀⣀⠤⠂⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠒⠤⡀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⢠⠊⠀⠴⠊⠀⠀⠀⠀      ⠀타임 코스모스 ~~⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠑⡄⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⢠⠃⠀⠀⠀         ⡂⡀⡀⠀⠀⠀⠀⠀⠀⠀⠈⡄⠀⠀⠀⠀⠀                               ⡠⠖⣁⠔⠁⠀⠀");
            Console.WriteLine("      ⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀      ⠀⠀    깐따삐야 ~                         ⠀⡇⠀⠀⣀⠔⠋⢀⠔⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠱⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠜⠀⡠⠚⠁⢠⠖⠁");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠑⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡴⠊⡰⠊⠀⣀⠞⠁⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⠤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡠⠒⠁⡠⠊⠀⡠⠚⠁⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠐⠂⠤⠄⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⡠⠤⠐⠊⠁⠀⡠⠊⠀⡠⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠔⠊⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⡰⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠉⡀⠀⠀⠀⠀⠉⠉⠉⠉⠉⠉⠉⠉⠉⠱⡀⠀⡐⠉⠉⠉⠉⠉⠉⠀⠀⠀⠀⠀⠀⢠⠊⠀⠀⠚⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠔⠊⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⢠⠊⠀⠀⠀⠀⠀⠀⠀⢀⠔⠀⠀⠀⡠⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⠀⠀⠀⠀⠀⠀⠀⡇");
            Console.WriteLine("⠀⠀⠀⠀⢀⠔⠁⠀⠀⠀⠀⠀⠀⢀⠔⠁⠀⠀⡠⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡀⠀⠀⠀⠀⠀⡇");
            Console.WriteLine("⠀⠀⠀⢠⠊⠀⠀⠀⠀⠀⠀⠀⡠⠃⠀⠀⢀⠎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡟⢭⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡰⠋⠀⠀⠀⠀⠀⠀⡇");
            Console.WriteLine("\r\n⠀⠀⡔⠁⠀⠀⠀⠀⠀⠀⢀⠎⠀⠀⢀⠔⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠤⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⡋⣉⡺⠆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡰⠊⠀⠀⠀⠀⠀⠀⠀⠀⠁");
            Console.WriteLine("⢀⠜⠀⠀⠀⠀⠀⠀⢀⠔⠁⠀⠀⡰⠁⠀⠀⠀⠀⢀⣀⣀⣀⣀⡀⠀⠀⣠⠎⠀⢀⡜⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣷⠟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠞⠁⠀⠀");
            Console.WriteLine("⠊⠀⠀⠀⠀⠀⠀⢠⠎⠀⠀⢠⠊⠀⠀⠀⡠⠖⠊⠉⠀⠀⠀⠀⠈⠱⠶⠁⠀⠀⠙⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣾⡟⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⢀⠔⠁⠀⠀⡰⠁⠀⠀⢠⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⡔⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⡿⠁⠀⠀⠀⠀⠀♬");
            Console.WriteLine("⠀⠀⠀⠀⡠⠊⠀⠀⢀⠎⠀⠀⠀⢠⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⢴⠃⠀⠀⢀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⢮⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡰⠁⠀⢀⠔");
            Console.WriteLine("⠀⠀⢀⠔⠁⠀⠀⡰⠁⠀⠀⠀⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠴⠋⠀⠣⢤⣾⣿⣿⣦⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⣞⡏⠢⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠊⠀⣀⠔⢁⠔");
            Console.WriteLine("⠀⢠⠊⠀⠀⢀⠞⠀⠀⠀⠀⠀⠀⠈⢆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣇⡤⣢⠅⠀⣿⣿⣿⣿⣿⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡀⢠⣃⢏⡯⠀⠀⡜⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠜⠀⡠⠊⠀⡠⠃⠀");
            Console.WriteLine("⡰⠁⠀⠀⠠⠃⠀⠀⠀⠀⠀⠀⠀⠀⠈⣦⡀⠀⠀⠀⠀⠀⠀⠀⡠⠔⠹⠊⠁⠀⠀⢿⣿⣿⣿⣿⠃⠀⠀⠀⠀⠀⠀⠀⢀⠤⠒⠁⠀⠀⠈⠛⢯⣍⣠⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠔⠁⡠⠊⠀⡠⠊");
            Console.WriteLine("⠀⠀⠀⡔⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⠁⠀⠀⠀⠀⢀⠤⢄⡠⠇⠀⠀⠀⠀⠀⠀⠈⠻⠿⣿⠃⠀⠀⠀⠀⠀⠀⠀⡰⠁⡖⠄⠀⡜⠃⠀⠀⠀⢏⢞⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡰⠃⡠⠊⠀⢀⠎⠀");
            Console.WriteLine("⠀⢀⠌⠀⠀⠀⠀♬⠀⠀⠀⠀⠀⠀⠀⠘⡀⠀⠀⠀⠀⡇⠀⠈⠀⠀⠀⠀⠀⠀⠀⠠⡤⠐⡎⣿⠃⠀⠀⠀⠀⠀⠀⢀⠃⠀⢇⡄⠸⠴⠁⡠⠄⡄⠈⡞⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠊⠀⠊⠀⠀⡴⠁⠀⠀⠀⠀⠔⠁");
            Console.WriteLine("⠠⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⠄⠀⠀⠀⠘⢄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠁⠚⡏⠀⠀⠀⠀⠀⠀⠀⠘⠄⠀⠀⢁⠀⠀⠈⠁⠉⠀⠀⡇⠇⠀⠀⠀⠀⠀⠀⠀⠀⢀⠔⠁⠀⠀⠀⡠⠊⠀⠀⠀⠀⡠⠊⠀");
            Console.WriteLine("⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡸⠀⠀⠀⠀⠀⠀⠉⢻⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⢸⠀⡔⣿⡶⡀⠀⠀⣀⣀⢰⢠⠃⠀⠀⠀⠀⠀⠀⠀⡠⠊⠀⠀⠀⢀⠔⠁⠀⠀⠀⢠⠊⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠃⠀⠀⠀⠀⠀⠀⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⢀⡔⠁⠀⠀⠀⠀⢀⠤⠂⠁⣸⠘⣰⠷⢡⠃⡰⠋⡠⠔⠿⡜⠀⠀⠀⠀⠀⠀⢀⠔⠁⠀⠀⠀⡠⠃⠀⠀⠀⢀⠔⠁⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣎⠀⠀⠀⠀⠀⠀⠀⠀⠈⡇⠀⠀⠀⠄⠄⠰⣖⠉⠀⣀⠤⠄⠂⠉⠀⡠⠉⠉⠀⢰⣿⠗⠁⠠⠁⡜⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠋⠀⠀⠀⢀⠔⠀⠀⠀⠀⡠⠊⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⡩⠋⠉⣆⠀⢀⡠⣄⡰⡁⠀⠀⢀⣀⣤⣤⣷⣴⡉⠀⠀⠀⠀⠀⢀⠃⠀⠀⠀⣾⡏⠀⠀⠀⠣⣳⢤⠀⠀⠀⠀⠀⠀⢀⡜⠁⠀⠀⠀⡠⠋⠀⠀⠀⢠⠊⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡰⠁⠀⠀⠀⠉⠁⠀⠀⢀⣧⣴⣾⣿⣿⣿⣿⣿⣿⣿⣷⣤⡀⡠⠔⢹⠀⠀⠀⢰⣿⠁⠀⢀⣠⠔⠡⠿⣱⠀⠀⠀⠀⠀⠈⠀⠀⠀⢀⠜⠀⠀⠀⠀⡰⠁");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡎⠀⠀⠀⠀⠀⠀⠀⠀⣰⠿⠿⠿⠿⠿⠿⣿⣿⣿⣿⣿⣿⣿⣿⣦⠀⢸⠀⠀⣀⡊⠁⠀⡴⠁⠀⠀⠀⠀⡸⠀⠀⠀⠀⠀⠀⠀⠀⢠⠋⠀⠀⠀⢀⠎");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡰⡝⠀⠀⠀⠀⠀⠀⠀⠀⢰⠉⠀⠀⠀⠀⠀⠀⠀⠈⠉⠛⠻⠿⠿⠿⠟⠗⠊⠉⠁⠀⠀⠀⠀⠁⠀⠀⠀⠀⢀⠇⠀⠀⠀⠀⠀⠀⠀⡔⠁⠀⠀⠀⡴⠁⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⢞⠜⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢆⠀⠀⠀⢀⠜⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠌⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⢠⢊⠎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⠤⠤⠀⠒⠊⠉⠓⠒⠈⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠂");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⡴⢁⠎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢃⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠤⠔⠊⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡔⠁");
            Console.WriteLine("⠀⠀⠀⠀⠀⢀⠜⠀⠊⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠅⠒⠒⠠⠤⠤⠤⠒⠒⠊⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⠜⠀⠀");
            ShowHighlightText("===============================================================================================================");
            Console.WriteLine("");
            Console.WriteLine("                                 Musician 이(가) 느닷 없이 바이올린을 연주합니다 !! ");
            Console.WriteLine("");
            Console.WriteLine("");
            ShowHighlightText("                                                                        Enter를 누르면 시간을 되돌립니다    ...   ");
            ShowHighlightText("===============================================================================================================");
            Console.WriteLine("");
            Console.ReadLine();
        }

        private static void WarriorIntroduce() // 전사 소개
        {
            Console.Clear();
            Console.WriteLine("⠀⣠⠞⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠳⠦⣄⠀");
            Console.WriteLine("⣸⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀   Hey Wizard...     Can you Please Go Home now?");
            Console.WriteLine("⠘⣆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⠞⠁⠀");
            Console.WriteLine("⠀⠈⠳⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⡤⠞⠉⠀");
            Console.WriteLine("⠀⠀⠀⠈⠓⠦⢤⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣠⣤⠴⠚⠉⠁⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠛⠒⠒⠒⠶⠦⠤⠤⠤⠤⣤⣤⣤⣤⣤⣤⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣠⣤⠀⠀⠠⠤⠴⠶⠒⠚⠋⠉⠉⢩⣇⡰⠒⠦⠤⠤⠤⠤⠴⠶⠒⠒⠋⠉⠉⠁");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡟⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣤⠀⠀⠀⠀⢀⣀⣾⣧⣤⣤⣤⣤⣤⣄⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢆⠀⠀⡿⠀⢀⣤⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣦⣄⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣀⣀⣀⣀⠈⢳⡄⡇⣴⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣄");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣴⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣧");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⢰⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡄");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣧⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠈⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠏⠙⠻⡿⠿⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡆");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠃⠀⠀⠀⠀⠀⠀⠈⠉⠛⠻⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠻⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠻⠿⣿⣿⣿⣿⣿⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢿⣿⣿⣿⣿⣿⣿⡿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡄⠀⠀⠀⠀⣄⠀⠀⠀⠀⠀⠀⠘⡟⠿⣿⣿⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣿⣿⣿⣿⣿⡇⠀⠀⠀⢀⣤⠤⠤⠶⠖⠒⠋⠀⠀⠀⠀⠀⠈⠉⠛⠒⠒⠒⠃⠀⡇⠀⠈⠛⠃");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣻⣿⣿⣿⠀⠀⠀⠰⠏⠀⠀⣀⣤⣤⣄⡀⠀⠀⠀⠀⠀⢀⣴⣾⣛⡆⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⠃⠈⠻⢿⡄⠀⠀⠀⠀⠀⠘⠷⣄⣹⣿⡗⠀⠀⠀⠀⠀⠻⠿⠿⠋⠀⠀⠀⠀⡇⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⢾⠗⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡀⠀⠀⠀⠀⠀⢸⠁");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⡀⠀⠀⠈⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡞⢀⠀⠀⣀⡇⠀⠀⠀⠀⠀⠈⠳⡄");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⠚⠑⠚⠁⠀⣀⠀⠀⠀⠀⠀⠀⠘⡄");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⣄⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⣤⣄⣀⣀⡀⢈⣳⠀⠀⠀⠀⠀⠀⢇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠉⢹⠀⠀⠀⠀⠀⠀⠀⠀⢰⠯⠤⠤⢴⣒⣛⠋⣹⠁⠀⠀⠀⠀⠀⠀⢸⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣇⠀⠀⠀⠀⠀⠀⠀⡏⢠⠖⠊⠉⠀⣈⡷⠃⠀⠀⠀⠀⠀⠀⠀⡞");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⡄⠀⠀⠀⠀⠀⠀⠉⠛⠒⠒⠒⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⡼⠁");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⠃⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠓⢤⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡜⠁⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢦⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡞⠁");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⡎⠻⣦⡀⠀⠀⠀⠀⠀⠀⢠⣤⡴⠫⢦⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠖⠒⠛⠛⠛⠒⠲⣧⠀⠘⢿⣦⠀⠀⠀⠀⠀⣸⠁⣠⠀⢸⡇⢠⠤⢤⣀⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡴⠋⠁⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠈⢻⣧⠀⠀⠀⡰⠃⡴⠁⠀⢸⠀⠀⠀⠀⠀⠉⠙⠳⣄");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡴⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠈⢿⠀⠀⢰⠃⣸⠃⠀⠀⡏⠀⠀⠀⠀⠀⠀⠀⠀⠈⢳⡀⠀⠀⣰⠛⡆⠀⠀⠀⣀⡄");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡤⠞⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠀⠀⠀⠀⠈⢧⠀⠀⣰⠃⣠⣄⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢧⠀⢠⠇⠀⢹⢀⡴⠋⠀⡇");
            Console.WriteLine("⠀⢀⡤⣄⠀⠀⠀⠀⠀⠀⠀⠀⣠⡖⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⡇⢠⡞⠓⢦⠘⡆⠀⠁⡼⠁⠸⡇⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡆⡞⠀⢀⣾⡞⠀⢠⡾⠟⠛⠛⣻⠇");
            Console.WriteLine("⣀⠈⢧⠈⠳⣄⠀⠀⠀⠀⢠⠖⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢳⣸⠀⠀⠀⢳⡹⡀⣰⠁⠀⠀⣧⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠇⠀⠘⣏⠁⠀⠈⢀⣴⣖⠋⠁");
            ShowHighlightText("=====================================================================================================");
            Console.WriteLine("");
            ShowHighlightText("                                          전사 (Warrior) ");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("               본래 세상에서는 Warrior가 아닌 30대의 평범한 가장이었으나");
            Console.WriteLine("");
            Console.WriteLine("               \"이상한 초록 도마뱀\"과 \"붉은 코의 외계인\"을 만나 인생이 꼬이기 시작합니다.     ");
            Console.WriteLine("");
            Console.WriteLine("               저 사고뭉치들 때문에 온갖 고통을 겪고 있지만  ");
            Console.WriteLine("");
            Console.WriteLine("               밀린 전세 대출 이자를 갚기 위해 단련 된 타고난 근성과 ");
            Console.WriteLine("");
            Console.WriteLine("               강철 같은 의지력을 바탕으로 오늘도 고군 분투하고 있습니다. ");
            Console.WriteLine("");
            Console.WriteLine("                                                                               다음 직업 보기.. Enter");
            ShowHighlightText("=====================================================================================================");
            Console.ReadLine();
        }

        private static void WizardIntroduce() // 마법사 소개
        {
            Console.Clear();
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⡰⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⢄⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⢰⠁⠀⠀⠀⠀⠀⠀       ⠀⠀⠀⠀   ⠀⠀I ~   C# !⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀    ⠀    ⠈⡆");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡇⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠑⠄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠠⠔⠋⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠑⠒⠤⢄⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣀⠤⠤⠄⠒⠊⠉⠀⠀");
            Console.WriteLine("\r\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠁⠐⠒⠠⠤⢄⣀⣀⣀⣀⣀⡠⠤⠤⡀⢠⠒⠂⠀⠀⠀⠂⠒⠒⠒⠂⠐⠉⠉");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀   ⠀⡇⡎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⡤⠤⠤⠖⠒⠦⠤⠤⣄⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⡴⠚⠉⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠉⠑⠦⣄⡀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡴⠊⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠉⠳⣄⡀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡴⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢦⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢦⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣧");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣷⠀⠀⠀⠀⠀⠀⠀⣰⢦⣀⠀⠀⠀⠰⡆⠀⣰⠟⠛⠀⣀⡶⣆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡾⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⠀⠀⠀⠀⠀⠀⢠⠇⢀⣈⠳⡄⠀⢼⣇⣾⠅⢀⢀⣰⠋⠀⠘⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣴⠃⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⡄⠀⠀⠀⠀⢠⣧⡖⢉⣿⣷⣷⠀⠀⠀⠯⠀⠀⣿⠋⣹⣶⡀⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⡄⠀⠀⠀⠘⣿⣿⣿⣿⡿⠉⢀⠀⣀⠀⠀⠀⣿⣾⣿⣿⠃⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠞⠋⠁⠀⠀⠀⠀⠈⠛⠛⠋⢀⡴⠞⠋⠉⠉⠳⣔⠙⢿⣛⣁⣠⠟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⡇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡞⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡏⠀⠀⠀⠀⠀⠀⠘⢦⠀⠈⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⢠⠇");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢦⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣇⠀⠀⠀⠀⠀⠀⠀⣸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣄⣠⠜⠁");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠢⣄⠀⠀⠀⢀⣰⠋⠀⠀⠀⢀⣀⣤⣶⣶⠖⠛⣿⠉⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠲⢤⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠓⠚⠉⠀⠀⠀⠀⣰⣟⣽⠞⠁⠀⠀⠀⣿");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠙⣟⠛⠛⢫⠟⠉⢓⠶⠶⠦⠤⣤⡤⣴⡶⣾⡿⠋⠀⠀⠀⠀⠀⠀⠘⢦");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⡆⠀⢸⡀⠀⡎⠀⠀⠀⣠⠞⠛⣠⡾⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⡄");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⣆⠀⢷⡀⠁⠀⢀⡔⠋⣀⡾⠛⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢙⣆⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢢⣤⡍⠉⠉⢉⣤⡾⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠙⠒⠶⢤⣀⣀⠀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⠾⠏⡷⠶⡖⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠙⠶⣤⡀");
            Console.WriteLine("⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠞⠁⠀⠀⣼⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠢");
            Console.WriteLine("");
            ShowHighlightText("=====================================================================================================");
            Console.WriteLine("");
            ShowHighlightText("                                         마법사 (Wizard) ");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("               본인이 1억년 전의 공룡이라는 소리를 하는 이상한 녹색 도마뱀.");
            Console.WriteLine("");
            Console.WriteLine("               상당히 뻔뻔한 성격으로, 동료들과 Warrior의 집을 무단 점거하고 있으며     ");
            Console.WriteLine("");
            Console.WriteLine("               \"Ho-It !\" 이라는 외침과 함께 시도 때도 없이 마법을 날려대는 통에 ");
            Console.WriteLine("");
            Console.WriteLine("               하루가 멀다하고 동네방네 크나큰 민폐를 끼치고 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("                                                                          다음 직업 보기.. Enter");
            ShowHighlightText("=====================================================================================================");
            Console.ReadLine();
        }

        private static void MusicianIntroduce() // 음악가 소개
        {
            Console.Clear();
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣀⣀⣀⣀⣀⣀⣤⣤⣄⣀⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⠶⠞⠛⠋⠉⠉⠉⠁⠀⠀⠀⠀⠀⠀⠈⠉⠛⠓⢦⣄⡀⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠾⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠳⣄⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠞⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⣄⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡼⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⣄⠀⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣧");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣧⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⡤⣤⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⡀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⣀⡀⠀⠀⠀⠀⠀⢀⣀⡀⠀⠀⠀⠀⠀⠀⣰⠏⠀⠀⠈⠳⣦⣀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡇");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⡀⣿⠁⠀⠀⠀⢀⡴⠋⠉⠙⠳⠶⠶⠶⠶⠞⠃⠀⠀⠀⠀⠀⠀⠙⠋⠛⢶⡄⠀⠀⠀⠀⠀⠀⠀⣸⠇⠀⠀⠀Yo, Wizard.⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⠋⠀⠘⢷⣴⣤⠞⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢦⣀⣀⡀⠀⠀⠈⣟⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⡏⠀⠀⠀⣸⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⡅⠀⠀⠀⢻⡄⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣇⠀⠀⠀⠈⡇");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⣄⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⣤⣄⡀⠀⠀⠀⣿⠀⠀⠀⠀⣿");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⢹⠀⠀⠀⠀⠀⠀⢠⠟⠁⠀⠀⠈⠉⠳⣄⠀⠀⠀⠀⢀⡴⠋⠀⠀⠀⠇⠀⠀⠀⣿⠀⠀⠀⠀⣿");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡏⠀⠀⠀⠀⠸⣇⠀⠀⠀⠀⠀⠀⠀⢀⣤⣤⣀⡀⠀⠈⠃⠰⠞⡵⠞⢀⣀⣀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⢀⡇");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⢹⡄⠀⠀⠀⠀⠀⠀⠀⠘⢿⣿⠗⠀⠀⠀⠀⠀⠰⢻⣯⣿⡿⠀⠀⠀⠀⠀⠀⡿⠀⠀⠀⣸⠁");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠘⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠀⠀⠀⠀⠉⠉⠀⠀⠀⠀⠀⠀⢸⡇⠀⠀⢀⡟⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⡴⠋⠀⠀⠀⠀⠀⠀⣸⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣠⣴⠏⢹⣿⣿⣿⣶⣄⡀⠀⠀⠀⠀⠀⠀⠘⢳⣄⠀⣸⠃⠀⠀⠀⠀⠀⠀⠀⢠⣤⡀⣠⣄⠀⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⢀⡾⠋⠁⠀⠀⠀⠀⠀⠀⠀⣰⠏⠁⠀⠀⠀⠀⠀⠀⠀⠀⣿⡁⠀⠀⣾⣿⣿⣿⣿⣿⣷⠀⠀⠀⠀⠀⠀⠀⠀⢹⡆⠉⠛⠓⢦⣄⡀⠀⠀⠀⢸⠀⠛⠃⠈⠙⢦⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⢸⡇⠀⠀⠀⠀⠀⠀⠀⠀⣴⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣿⣿⣶⣿⣿⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⡆⠀⠀⠀⠈⢷⠀⠀⠀⡞⠀⠀⠀⠀⠀⠈⡇");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⣸⣧⠀⠀⠀⠀⠀⠀⠀⣸⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢿⣿⣿⣿⣿⣿⣿⣿⣿⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣧⠀⠀⠀⠀⢸⡇⠀⠀⢷⠀⠀⠀⠀⠀⠀⢹");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⢰⠋⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⠿⢿⣿⠿⠿⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⢀⣤⡟⠀⠀⠀⠸⡆⠀⠀⠀⠀⠀⢸");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠸⣦⡀⠀⠀⠀⠀⠀⠀⠀⢹⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠉⢹⡆⠀⠀⡴⠃⠀⠀⠀⠀⠀⢸");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⢨⡿⠀⠀⠀⠀⠀⠀⠀⠀⠻⣄⡀⠀⠀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣼⣃⠀⣀⣄⣰⠟⠀⠀⣰⠃⠀⠀⠀⠀⠀⠀⢸⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠸⣷⣦⣴⡆⠀⠀⠀⢀⣠⣤⡼⠿⠓⠿⢿⣄⣀⣀⣀⣀⣀⡀⢀⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⡤⠖⠋⠉⠉⠛⠉⠉⠁⠀⠀⢰⠇⠀⠀⢸");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠉⠙⠻⠿⣦⡶⠞⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⠉⠙⢳⡖⠦⠤⣴⣄⣀⣀⣴⣤⡶⣯⣁⣀⣀⣀⣀⣀⣀⠀⠀⠀⠀⢀⡏⠀⠀⢸");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠛⠂⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⣿⣿⣿⣿⣷⣤⣤⣼⣷⣤⣼⣿⣿⣿⣿⣿⣿⡌⠋⠀⠀⠀⠀⠀⠀⠈⠉⠉⠙⠛⠋⠀⠀⠀⠀⡏⠀⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣆⠀⠀⠀⠀⠀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡀⠀");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡴⠋⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣄");
            Console.WriteLine(" ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠊⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡄⠀⠀");
            Console.WriteLine("");
            ShowHighlightText("=====================================================================================================");
            Console.WriteLine("");
            ShowHighlightText("                                         음악가 (Musician) ");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("               자신이 'Kan-Ta Pyah'(칸-타 퍄) 라는 곳에서 왔다는 주장을 하는 붉은 코의 외계인..");
            Console.WriteLine("");
            Console.WriteLine("               악명 높은 Wizard 일당 중에서도 가장 교활하고 사나운 성격으로     ");
            Console.WriteLine("");
            Console.WriteLine("               틈만 나면 바이올린을 연주하며 매일 같이 소음 공해를 일으키기 일쑤입니다. ");
            Console.WriteLine("");
            Console.WriteLine("               조금이라도 수틀리면 연주하던 악기로 '시간을 되돌려' 버리는 무서운 능력을 가지고 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("                                                                               직업 선택으로 >> Enter");
            ShowHighlightText("=====================================================================================================");
            Console.ReadLine();
        }

    }
}

















 



                                                                             