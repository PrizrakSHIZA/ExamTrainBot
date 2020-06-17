using ExamTrainBot.Tests;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamTrainBot
{
    [Serializable]
    public class User
    {
        public long id;
        public string name;
        public bool subscriber,isadmin, ontest;
        public int currentquestion;
        public bool testcreation;
        public List<int> points = new List<int>();
        public List<Test> completedtests = new List<Test>();

        public static int currenttest = 0;
        public int currentTest_serializable { get { return currenttest; } set { currenttest = value; } }

        public User(long id, string name, bool ispayeduser, bool isadmin)
        {
            this.id = id;
            this.name = name;
            this.subscriber = ispayeduser;
            this.isadmin = isadmin;
            ontest = false;
            currentquestion = 0;
            currenttest = 0;
            testcreation = false;
        }
    }
}
