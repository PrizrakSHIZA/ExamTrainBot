using ExamTrainBot.Commands;
using ExamTrainBot.Tests;
using ExamTrainBot.Tests.Questions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
//using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExamTrainBot
{
    class Program
    {
        public static TelegramBotClient bot;
        public static List<Command> commands = new List<Command>();
        public static List<User> users = new List<User>();
        public static List<Test> testlist = new List<Test>();
        public static List<Question> questions = new List<Question>();
        public static DateTime TestTime = DateTime.Today.AddHours(14);
        public static MySqlConnection con = new MySqlConnection(
                    new MySqlConnectionStringBuilder()
                    {
                        Server = APIKeys.DBServer,
                        Database = APIKeys.DBName,
                        UserID = APIKeys.DBUser,
                        Password = APIKeys.DBPassword
                    }.ConnectionString
                );
        public static string password = APIKeys.password;
        public static bool useTimer = false;
        public static char[] delimiterChars = { ',', '.', '\t', '\n', ';' };

        static Timer timer;

        static void Main(string[] args)
        {
            //Loading data
            LoadFromDB();
            //SaveSystem.Load();

            //Add all commands
            AddAllCommands();

            //add all tests;
            AddAllTests();

            //Initialize timer
            InitializeTimer(TestTime.Hour, TestTime.Minute);

            //Initialize bot client
            bot = new TelegramBotClient(APIKeys.TestBotApi) { Timeout = TimeSpan.FromSeconds(10) };

            //Starting message
            var me = bot.GetMeAsync().Result;
            Console.WriteLine($"Its me, {me}!");

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            bot.StartReceiving();
            bot.OnMessage += Bot_OnMessage;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;

            Console.ReadKey();
        }

        private async static void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            User user = GetCurrentUser(e);
            //if user is on test
            if (user.ontest)
            {
                Question question = testlist[User.currenttest].questions[user.currentquestion];
                //Check answer is right or wrong
                if (e.CallbackQuery.Data == question.answer)
                {
                    await bot.SendTextMessageAsync(user.id, "Правильно!");
                    user.currentquestion++;
                    user.points[^1] += question.points;
                }
                else
                {
                    await bot.SendTextMessageAsync(user.id, $"Неправильно! Правильна відповідь: {question.answer}");
                    user.mistakes[^1][user.currentquestion] = true;
                    user.currentquestion++;
                }
                //Check if its last question in test
                if (user.currentquestion >= testlist[User.currenttest].questions.Count)
                {
                    user.ontest = false;
                    user.currentquestion = 0;
                    await bot.SendTextMessageAsync(user.id, $"Вітаю! Ви закінчили тест. Ви набрали {user.points[^1]} балів!");
                }
                else
                {
                    testlist[User.currenttest].questions[user.currentquestion].Ask(user.id);
                }
            }
            //if admin creating test
            else if (user.testcreation)
            {
                A_TestAdd cmd = (A_TestAdd)commands.Find(c => c.GetType().Equals(typeof(A_TestAdd)));
                cmd.Execute(e.CallbackQuery.Data);
            }
        }

        private async static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            //check if new user
            if (users.Find(u => u.id == e.Message.Chat.Id) == null)
            {
                users.Add(new User(e.Message.Chat.Id, e.Message.Chat.FirstName + " " + e.Message.Chat.LastName, false, false));
                SaveSystem.Save();
            }
            //Add user in temp var
            User user = users.Find(u => u.id == e.Message.Chat.Id);

            var text = e?.Message?.Text;
            if (text == null) return;

            //if admin creating test
            if (user.testcreation)
            {
                commands.Find(c => c.GetType().Equals(typeof(A_TestAdd))).Execute(e);
            }
            //If user completing test
            else if (user.ontest)
            {
                string answer = e.Message.Text;
                Question question = testlist[User.currenttest].questions[user.currentquestion];
                //Check conformity question
                if (testlist[User.currenttest].questions[user.currentquestion] is ConformityQuestion)
                {
                    ConformityQuestion q = (ConformityQuestion)testlist[User.currenttest].questions[user.currentquestion];
                    //Delete spaces
                    if (q.IsRight(answer))
                    {
                        await bot.SendTextMessageAsync(user.id, "Правильно!");
                        user.currentquestion++;
                        user.points[^1] += question.points;
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(user.id, $"Неправильно! Правильна відповідь: {question.answer}");
                        user.mistakes[^1][user.currentquestion] = true;
                        user.currentquestion++;
                    }
                }
                //Check other type
                else if (answer.ToLower() == question.answer.ToLower())
                {
                    await bot.SendTextMessageAsync(user.id, "Правильно!");
                    user.currentquestion++;
                    user.points[^1] += question.points;
                }
                else
                {
                    await bot.SendTextMessageAsync(user.id, $"Неправильно! Правильна відповідь: {question.answer}");
                    user.mistakes[^1][user.currentquestion] = true;
                    user.currentquestion++;
                }
                //Check if its last question in test
                if (user.currentquestion >= testlist[User.currenttest].questions.Count)
                {
                    user.ontest = false;
                    user.currentquestion = 0;
                    await bot.SendTextMessageAsync(user.id, $"Вітаю! Ви закінчили тест. Ви набрали {user.points[^1]} балів!");
                }
                else
                {
                    testlist[User.currenttest].questions[user.currentquestion].Ask(user.id);
                }
            }
            else
            {
                //Check commands
                text += " ";
                int index = text.IndexOf(" ");
                text = text.Substring(0, index);

                if (commands.Find(c => c.Name == text) != null)
                {
                    Command cmd = commands.Find(c => c.Name == text);
                    if (!cmd.forAdmin || (cmd.forAdmin && user.isadmin))
                    {
                        cmd.Execute(e);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(user.id, "У вас немає доступу до цієї команди");
                    }
                }
                else
                {
                    await bot.SendTextMessageAsync(user.id, "Такої команди не уснує. Для списку усіх команд введіть: '/help'");
                }
            }
        }

        private static void AddAllTests()
        {
            /*
            testlist.Add(new Test
            {
                Text = "Відсотком (процентом) називають число одну соту. Отже, 1:100 = 1%; 100% = 100:100 = 1; 50% = 50:100 = 1:2; 25% = 25:100 = 1:4\n" +
                        "Тоді 100% числа а дорівнюють а, 50% числа а дорівнюють 1:2*а, 25% числа а ставновлять 1:4*а.\n" +
                        "Щоб перетворити десятковий дріб у відсотки, потрібно помножити його на 100.\n" +
                        "Щоб перетворити відсотки у десятковий дріб, потрібно число відсотків поділити на 100.\n" +
                        "Наприклад: а) 0,002 = 0,002 * 100% = 0,2%; 0,07 = 0,07 * 100% = 7%; 1,34 = 1,34 * 100% = 134%;\n" +
                        "б) 2,3% = 2,3% : 100% = 0,023; 40% = 40% : 100% = 0,4; 263% = 263% : 100% = 2,63",
                questions = new List<Question>()
                {
                    new TestQuestion("Як знайти 52% від числа 960?", 2, new string[] {"960 * 100 : 52","52 * 100 : 96","960 * 52 : 100","960 : 52", "960 * 52"}, 2, "960 * 52 : 100"),
                    new TestQuestion("Як знайти число, 60% від якого дорівнюють 360?", 2, new string[] {"360 * 60","360 : 60","60 * 100 : 360","360 * 60 : 100","360 * 100 : 60"}, 2, "360 * 100 : 60"),
                    new TestQuestion("(ЗНО-2010) За видачу свідоцтва про право на спадщину стягується державне мито в розмірі\n"
                                    +"0,5% від вартості майна, що успадковується. Скільки державного мита повинен сплатити спадкоємець,\n"
                                    +"якщо вартість майна, що успадковується, становить 32 000 грн?",
                                    3, new string[] {"16 грн","64 грн","160 грн","320 грн","1600 грн"}, 2, "160 грн"),
                    new TestQuestion("2 кг сплаву міді з оловом містить 40% міді. Скільки потрібно додати до цього сплаву олова,\nщоб отриманий сплав містив 16% міді?", 4, new string[] {"3 кг","2,5 кг","2 кг","4 кг","3,5 кг"}, 2, "3 кг"),
                    new FreeQuestion("Яка тварина є самою вірною людині?",2,"Собака"),
                    new ConformityQuestion("Об'єднайте правильно: \nА)Техніка\nБ)Тварина\nВ)Комаха\nГ)Людина\n\n1)Собака\n2)Міша\n3)Літак\n4)Павук",4,"А-3,Б-1,В-4,Г-2"),
                },
            });*/
            SaveSystem.SaveTests();
        }

        private static void AddAllCommands()
        {
            commands.Add(new A_TestAdd());
            commands.Add(new A_TestAll());
            commands.Add(new A_TestDelete());
            commands.Add(new A_TestId());
            commands.Add(new A_TestList());
            commands.Add(new A_TimerSet());
            commands.Add(new A_TimerTurn());
            commands.Add(new A_UserAdd());
            commands.Add(new A_UserDelete());
            commands.Add(new A_UserList());
            commands.Add(new A_UserResault());
            commands.Add(new AdminCmd());
            commands.Add(new HelpCommand());
            commands.Add(new RecordsCmd());
            commands.Add(new TestCmd());
            commands.Add(new TimerCmd());
            commands.Sort((x, y) => string.Compare(x.Name, y.Name));
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            SaveSystem.Save();
        }

        public static User GetCurrentUser(long id)
        {
            return users.Find(u => u.id == id);
        }
        public static User GetCurrentUser(MessageEventArgs e)
        {
            long id = e.Message.Chat.Id;
            return users.Find(u => u.id == id);
        }
        public static User GetCurrentUser(CallbackQueryEventArgs e)
        {
            long id = e.CallbackQuery.From.Id;
            return users.Find(u => u.id == id);
        }


        public static InlineKeyboardMarkup GetInlineKeyboard(string[] array, int column)
        {
            int steps = (int)Math.Round((double)array.Length / column);
            var keyboardInline = new InlineKeyboardButton[steps][];
            for (int y = 0; y < steps; y++)
            {
                var keyboardButtons = new InlineKeyboardButton[column];
                for (int i = 0; i < column; i++)
                {
                    keyboardButtons[i] = new InlineKeyboardButton
                    {
                        Text = array[(y * column) + i],
                        CallbackData = array[(y * column) + i],
                    };
                }
                keyboardInline[y] = keyboardButtons;
            }

            return keyboardInline;
        }

        public static void InitializeTimer(int hour, int minute)
        {
            TestTime = DateTime.Today.AddHours(hour).AddMinutes(minute);
            if (useTimer)
            {
                if (timer != null)
                    timer.Dispose();
                timer = new Timer(new TimerCallback(TestAll));

                // Figure how much time until seted time
                DateTime now = DateTime.Now;

                // If it's already past setted time, wait until setted time tomorrow    
                if (now > TestTime)
                {
                    TestTime = TestTime.AddDays(1.0);
                }

                int msUntilTime = (int)((TestTime - now).TotalMilliseconds);

                // Set the timer to elapse only once, at setted teme.
                timer.Change(msUntilTime, Timeout.Infinite);
            }
            else
            {
                if(timer != null)
                    timer.Dispose();
                timer = null;
            }
        }

        public async static void TestAll(object state)
        {
            //Check if program have test to send
            if (User.currenttest + 1 > testlist.Count)
            {
                //Send msg to admins if no
                foreach (User u in Program.users)
                {
                    if (u.isadmin)
                    {
                        await bot.SendTextMessageAsync(u.id, $"Неможливо відправити тести користувачам оскільки немає тесту за індексом {User.currenttest}!");
                    }
                }
            }
            else
            {
                foreach (User u in Program.users)
                {
                    if (u.isadmin)
                    {
                        //create bool array filled with false value
                        bool[] tempbool = Enumerable.Repeat(false, testlist[User.currenttest].questions.Count + 1).ToArray();
                        u.mistakes.Add(tempbool);
                        u.points.Add(0);
                        u.completedtests.Add(Program.testlist[User.currenttest]);
                        u.ontest = true;
                        u.currentquestion = 0;
                        await Program.bot.SendTextMessageAsync(u.id, Program.testlist[User.currenttest].Text);
                        Program.testlist[User.currenttest].questions[0].Ask(u.id);
                    }
                }
                //timer to 0:00
                Timer t = new Timer(new TimerCallback(StopTest));
                DateTime temptime = DateTime.Today.AddHours(23).AddMinutes(59);

                int msUntilTime = (int)((temptime - DateTime.Now).TotalMilliseconds);
                t.Change(msUntilTime, Timeout.Infinite);

                InitializeTimer(TestTime.Hour, TestTime.Minute);
            }
        }

        public async static void StopTest(object state)
        {
            foreach (User u in Program.users)
            {
                if (u.isadmin && u.ontest)
                {
                    u.ontest = false;
                    u.currentquestion = 0;
                    await Program.bot.SendTextMessageAsync(u.id, $"Час на тест закінчився. Ви набрали: {u.points[^1]} балів!");
                }
            }
            SaveSystem.Save();
        }

        public static void LoadFromDB()
        {
            try
            {
                con.Open();

                //Load Questions
                string command = "SELECT * FROM questions";
                MySqlCommand cmd = new MySqlCommand(command, con);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    switch (reader.GetInt32("type"))
                    {
                        case 1: questions.Add(new TestQuestion(
                            reader.GetString("text"), 
                            reader.GetInt32("points"),
                            reader.GetString("variants").Replace(" ","").Split(delimiterChars), 
                            reader.GetInt32("columns"), 
                            reader.GetString("answer"))); break;
                        case 2: questions.Add(new FreeQuestion(
                            reader.GetString("text"),
                            reader.GetInt32("points"),
                            reader.GetString("answer")
                            )); break;
                        case 3: questions.Add(new ConformityQuestion(
                            reader.GetString("text"),
                            reader.GetInt32("points"),
                            reader.GetString("answer")
                            )); break;
                        default: break;
                    }
                }
                reader.Close();

                //Load Tests
                command = "SELECT * FROM tests";
                cmd = new MySqlCommand(command, con);

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string[] ids = reader.GetString("questions").Replace(" ", "").Split(delimiterChars);
                    List<Question> q = new List<Question>();
                    for (int i = 0; i < ids.Length; i++)
                    {
                        q.Add(questions[ Int32.Parse(ids[i]) - 1 ]);
                    }
                    testlist.Add(new Test(reader.GetString("rule"), q));
                }
                reader.Close();

                //Load Users
                command = "SELECT * FROM users";
                cmd = new MySqlCommand(command, con);

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User(reader.GetInt32("ID"), reader.GetString("Name") + " " + reader.GetString("Soname"), Convert.ToBoolean(reader.GetUInt32("Subscriber")), Convert.ToBoolean(reader.GetUInt32("Admin"))));
                }
                reader.Close();

                con.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine("Потрібен перезапуск");
            }
        }
    }
}
