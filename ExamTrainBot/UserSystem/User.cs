using ExamTrainBot.Commands;
using ExamTrainBot.Tests;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ExamTrainBot
{
    [Serializable]
    public class User
    {
        public long id;
        public string name;
        public bool subscriber,isadmin, ontest = false;
        public int currentquestion = 0;
        public bool testcreation = false;
        public List<int> points = new List<int>();
        public List<Test> completedtests = new List<Test>();
        public List<bool[]> mistakes = new List<bool[]>();

        public static int currenttest = 0;
        public int currentTest_serializable { get { return currenttest; } set { currenttest = value; } }

        public User(long id, string name)
        {
            this.id = id;
            this.name = name;
            this.subscriber = false;
            this.isadmin = false;
        }
        public User(long id, string name, bool subscriber, bool isadmin, string points, string tests, string mistakes)
        {
            this.id = id;
            this.name = name;
            this.subscriber = subscriber;
            this.isadmin = isadmin;
            if (points != "" || tests != "")
            {
                this.points = points.Replace(" ", "").Split(Program.delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();
                int[] temp = tests.Replace(" ", "").Split(Program.delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
                for (int i = 0; i < temp.Length; i++)
                {
                    completedtests.Add(Program.testlist[temp[i] - 1]);
                }
            }
        }
    }
}
